using System;
using System.Collections.Generic;

namespace OrchardCore.ContentFields.Fields;

#nullable enable

public static class ContentPickerFieldExtensions
{
    /// <summary>
    /// Compares two content pickers by their <see cref="ContentPickerField.ContentItemIds"/> as if they were sets,
    /// meaning that their count and contents must match regardless of item order.
    /// </summary>
    public static bool ContentItemIdsEqual(this ContentPickerField field1, ContentPickerField? field2)
    {
        ArgumentNullException.ThrowIfNull(field1);
        if (field2 is null) return false;

        if (field1.ContentItemIds.Length != field2.ContentItemIds.Length) return false;

        if (field1.ContentItemIds.Length == 1) return field1.ContentItemIds[0] == field2.ContentItemIds[0];

        return new HashSet<string>(field1.ContentItemIds).SetEquals(field2.ContentItemIds);
    }
}
