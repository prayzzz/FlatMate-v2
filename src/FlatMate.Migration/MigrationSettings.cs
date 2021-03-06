﻿namespace FlatMate.Migration
{
    public class MigrationSettings
    {
        public string BackupFolder { get; set; }

        public string ConnectionString { get; set; }

        public bool CreateMissingSchema { get; set; } = false;

        public string DbSchemaAndTableEscaped => IsSchemaSet ? $"{DbSchemaEscaped}.{DbTableEscaped}" : DbTableEscaped;

        public string DbSchemaEscaped => $"[{Schema}]";

        public string DbTableEscaped => $"[{Table}]";

        public bool IsSchemaSet => !string.IsNullOrWhiteSpace(Schema);

        public string MigrationsFolder { get; set; } = ".";

        public string Schema { get; set; }

        public string Table { get; } = "Migrations";
    }
}