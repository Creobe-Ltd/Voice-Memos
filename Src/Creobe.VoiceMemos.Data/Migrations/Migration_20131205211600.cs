using Creobe.VoiceMemos.Core.Data;
using Creobe.VoiceMemos.Models;
using Creobe.VoiceMemos.Models.Legacy;
using Microsoft.Phone.Data.Linq;
using System.Data.Linq;

namespace Creobe.VoiceMemos.Data.Legacy.Migrations
{
    public class Migration_20131205211600 : IMigration
    {
        public int Version
        {
            get { return 2; }
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
            dbUpdater.AddColumn<Memo>("AudioFormat");
            dbUpdater.AddColumn<Memo>("SampleRate");
            dbUpdater.AddColumn<Memo>("BitRate");
            dbUpdater.AddColumn<Memo>("Channels");
        }
    }
}
