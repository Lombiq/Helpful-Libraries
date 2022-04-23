using System;
using System.Data.Common;

namespace YesSql;

public static class TransactionSqlDialectFactory
{
    /// <summary>
    /// Retrieves the <see cref="ISqlDialect"/> for the given <see cref="DbTransaction"/>.
    /// </summary>
    [Obsolete("This is no longer supported since YesSql 3. Use ISession.Value.Store.Configuration.SqlDialect.")]
    public static ISqlDialect For(DbTransaction transaction) => throw new NotSupportedException();
}
