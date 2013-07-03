using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orchard.Data.Migration;
using Orchard.Environment.Extensions;
using Piedone.HelpfulLibraries.Models;

namespace Piedone.HelpfulLibraries.KeyValueStore
{
    [OrchardFeature("Piedone.HelpfulLibraries.KeyValueStore")]
    public class Migrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(KeyValueRecord).Name,
                table => table
                    .Column<int>("Id", column => column.PrimaryKey().Identity())
                    .Column<string>("StringKey", column => column.WithLength(2048).Unique())
                    .Column<string>("Value", column => column.Unlimited())
                )
            .AlterTable(typeof(KeyValueRecord).Name,
                table => table
                    .CreateIndex("StringKey", new string[] { "StringKey" })
            );

            return 1;
        }
    }
}
