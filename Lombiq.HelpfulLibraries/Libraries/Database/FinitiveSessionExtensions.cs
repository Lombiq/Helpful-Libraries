using System.Data.Common;

namespace YesSql
{
    public static class FinitiveSessionExtensions
    {

        public static DbConnection Connection(this ISession session) =>
#pragma warning disable VSTHRD104
            session.CreateConnectionAsync().Result;
#pragma warning restore VSTHRD104

    }
}
