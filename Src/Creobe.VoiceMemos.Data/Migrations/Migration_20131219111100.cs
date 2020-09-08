using Creobe.VoiceMemos.Core.Data;
using Creobe.VoiceMemos.Models;
using Creobe.VoiceMemos.Models.Legacy;
using Microsoft.Phone.Data.Linq;
using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Creobe.VoiceMemos.Data.Legacy.Migrations
{
    public class Migration_20131219111100 : IMigration
    {
        public int Version
        {
            get { return 3; }
        }

        public void Apply(DataContext dbContext)
        {
            var dbUpdater = dbContext.CreateDatabaseSchemaUpdater();

            if (dbUpdater.DatabaseSchemaVersion < Version)
            {
                Update(dbUpdater);
                dbUpdater.DatabaseSchemaVersion = Version;
                dbUpdater.Execute();
            }
        }

        private void Update(DatabaseSchemaUpdater dbUpdater)
        {
            dbUpdater.AddTable<Favorite>();
        }

    }
}
