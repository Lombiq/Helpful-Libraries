using Dapper;
using Lombiq.HelpfulLibraries.OrchardCore.Data;
using Lombiq.HelpfulLibraries.Tests.Models;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using OrchardCore.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using YesSql;
using YesSql.Provider.Sqlite;
using YesSql.Sql;

namespace Lombiq.HelpfulLibraries.Tests.UnitTests.Services;

public sealed class ManualConnectingIndexServiceFixture : IDisposable
{
    private const string NamePrefix = ManualConnectingIndexServiceTests.NamePrefix;
    private const string FileName = nameof(ManualConnectingIndexServiceTests) + ".db";

    private readonly IConfiguration _configuration;

    public IReadOnlyList<TestDocument> Documents { get; }
    public IStore Store { get; set; }

    public ManualConnectingIndexServiceFixture()
    {
        _configuration = new Configuration()
            .UseSqLite($"Data Source={FileName};Cache=Shared", IsolationLevel.ReadUncommitted)
            .UseDefaultIdGenerator();

        Documents = Enumerable
            .Range(0, 10)
            .Select(n => new TestDocument { Name = NamePrefix + n.ToTechnicalString() })
            .ToArray();

        if (File.Exists(FileName)) File.Delete(FileName);
        CreateDatabaseAsync().Wait();
    }

    public async Task SessionAsync(Func<ISession, Task> action)
    {
        if (Store == null) await CreateDatabaseAsync();

        await using var session = Store.CreateSession();
        await action(session);
        await session.SaveChangesAsync();
    }

    // We could have a
    //// if (File.Exists(FileName)) File.Delete(FileName);
    // here but on .NET 6 the file remains locked despite the disposal. It doesn't really matter if it remains there
    // after a test execution.
    public void Dispose() => Store?.Dispose();

    private async Task CreateDatabaseAsync()
    {
        Store = (Store)await StoreFactory.CreateAndInitializeAsync(_configuration);
        var dbAccessorMock = new Mock<IDbConnectionAccessor>();
        dbAccessorMock.Setup(x => x.CreateConnection())
            .Returns(() => _configuration.ConnectionFactory.CreateConnection());
        var dbAccessor = dbAccessorMock.Object;

        await using (var connection = dbAccessor.CreateConnection())
        {
            await connection.OpenAsync();
            await using (var transaction = await connection.BeginTransactionAsync())
            {
                var schemaBuilder = new SchemaBuilder(_configuration, transaction);

                if (transaction.Connection.Query<string>(
                        $"SELECT name FROM sqlite_master WHERE type='table' AND name='{nameof(TestDocumentIndex)}';")
                    .FirstOrDefault() != null)
                {
                    schemaBuilder.DropTable(nameof(TestDocumentIndex));
                }

                schemaBuilder.CreateMapIndexTable<TestDocumentIndex>(
                    table => table.Column<int>(nameof(TestDocumentIndex.Number)));
                await transaction.CommitAsync();
            }

            await connection.CloseAsync();
        }

        await SessionAsync(session =>
        {
            foreach (var document in Documents) session.Save(document);
            return Task.CompletedTask;
        });

        var manualConnectingIndexService = new ManualConnectingIndexService<TestDocumentIndex>(
            dbAccessor,
            new NullLogger<ManualConnectingIndexService<TestDocumentIndex>>());
        for (var i = 0; i < Documents.Count; i++)
        {
            if (i == 3) continue;

            var index = new TestDocumentIndex
            {
                Number = int.Parse(
                    Documents[i].Name.Replace(NamePrefix, string.Empty, StringComparison.InvariantCulture),
                    CultureInfo.InvariantCulture),
            };

            await SessionAsync(session => manualConnectingIndexService.AddAsync(index, session, i + 1));
        }

        await SessionAsync(session =>
            manualConnectingIndexService.RemoveAsync(nameof(TestDocumentIndex.Number), 6, session));
    }
}
