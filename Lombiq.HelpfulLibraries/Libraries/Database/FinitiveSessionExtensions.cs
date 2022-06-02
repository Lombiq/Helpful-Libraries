using Dapper;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace YesSql
{
    public static class FinitiveSessionExtensions
    {
        public static async Task<IEnumerable<T>> QueryAsync<T>(this ISession session, string sql, object param = null)
        {
            var conn = await session.CreateConnectionAsync();
            var tran = session.CurrentTransaction;
            return await conn.QueryAsync<T>(sql, param, tran);
        }

        public static async Task<int> ExecuteAsync(this ISession session, string sql, object param = null)
        {
            var conn = await session.CreateConnectionAsync();
            var tran = session.CurrentTransaction;
            return await conn.ExecuteAsync(sql, param, tran);
        }

        public static async Task<T> QueryFirstAsync<T>(this ISession session, string sql, object param = null)
        {
            var conn = await session.CreateConnectionAsync();
            var tran = session.CurrentTransaction;
            return await conn.QueryFirstAsync<T>(sql, param, tran);
        }

    }
}
