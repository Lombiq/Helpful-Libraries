using YesSql.Sql.Schema;

namespace Lombiq.HelpfulLibraries.OrchardCore.Data;

public static class ColumnCommandExtensions
{
    /// <summary>
    /// Sets the created column length to the commonly used unique ID length which is 26.
    /// </summary>
    private static IColumnCommand WithCommonUniqueIdLength(this IColumnCommand command) =>
        command.WithLength(26);

    /// <summary>
    /// Sets the created column length to the commonly used unique ID length which is 26.
    /// </summary>
    public static ICreateColumnCommand WithCommonUniqueIdLength(this ICreateColumnCommand command) =>
        (ICreateColumnCommand)((IColumnCommand)command).WithCommonUniqueIdLength();

    /// <summary>
    /// Alters the column length to the commonly used unique ID length which is 26.
    /// </summary>
    public static IAlterColumnCommand WithCommonUniqueIdLength(this IAlterColumnCommand command) =>
        (IAlterColumnCommand)((IColumnCommand)command).WithCommonUniqueIdLength();
}
