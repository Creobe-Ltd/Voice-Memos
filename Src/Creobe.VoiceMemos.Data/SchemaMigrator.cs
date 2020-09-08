using Creobe.VoiceMemos.Core.Data;
using Microsoft.Phone.Data.Linq;
using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Reflection;

namespace Creobe.VoiceMemos.Data
{
    public class SchemaMigrator
    {
        public static void ApplyMigrations(DataContext dbContext)
        {
            var migrations = GetMigrations();

            foreach (var m in migrations)
            {
                var migration = CreateMigration(m);
                migration.Apply(dbContext);
            }
        }

        public static void SetSchemaVersion(DataContext dbContext)
        {
            int version = 1;
            var lastMigration = GetMigrations().LastOrDefault();

            if (lastMigration != null)
            {
                var migration = CreateMigration(lastMigration);
                version = migration.Version;
            }

            var dbUpdater = dbContext.CreateDatabaseSchemaUpdater();
            dbUpdater.DatabaseSchemaVersion = version;
            dbUpdater.Execute();
        }

        private static IEnumerable<Type> GetMigrations()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var ns = "Creobe.VoiceMemos.Data.Migrations";

            return assembly.GetTypes()
                .Where(t => string.Equals(t.Namespace, ns, StringComparison.Ordinal))
                .Where(t => t.GetInterfaces().Contains(typeof(IMigration)))
                .OrderBy(t => t.Name);
        }

        private static IMigration CreateMigration(Type migration)
        {
            return (IMigration)Activator.CreateInstance(migration);
        }
    }
}
