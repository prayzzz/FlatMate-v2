using System;

namespace FlatMate.Migration
{
    public class MigrationSettings
    {
        public string ConnectionString { get; set; }

        public string MigrationsFolder { get; set; }
    }
}