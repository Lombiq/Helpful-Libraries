using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orchard.Data.Migration;
using Orchard.Environment.Extensions;
using Piedone.HelpfulLibraries.Models;

namespace Piedone.HelpfulLibraries.Tasks.Locking
{
    [OrchardFeature("Piedone.HelpfulLibraries.Tasks.Locking.Database")]
    public class DatabaseLockMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(DatabaseLockRecord).Name,
                table => table
                    .Column<int>("Id", column => column.PrimaryKey().Identity())
                    .Column<string>("Name", column => column.NotNull().WithLength(255))
                    .Column<DateTime>("AcquiredUtc")
                )
            .AlterTable(typeof(DatabaseLockRecord).Name,
                table =>
                {
                    table.CreateIndex("Name", "Name");
                    table.CreateIndex("AcquiredUtc", "AcquiredUtc");
                });


            return 1;
        }
    }
}
