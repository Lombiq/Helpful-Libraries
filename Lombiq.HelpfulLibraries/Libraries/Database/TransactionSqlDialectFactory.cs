using System.Data.Common;

namespace YesSql
{
    public static class TransactionSqlDialectFactory
    {
        /// <summary>
        /// Retrieves the <see cref="ISqlDialect"/> for the given <see cref="DbTransaction"/>.
        /// </summary>
        public static ISqlDialect For(DbTransaction transaction) => TransactionSqlDialectFactory.For(transaction);
    }
}
