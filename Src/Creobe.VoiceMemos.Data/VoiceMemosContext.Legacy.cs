using Creobe.VoiceMemos.Models;
using Creobe.VoiceMemos.Models.Legacy;
using System.Data.Linq;

namespace Creobe.VoiceMemos.Data.Legacy
{
    public class VoiceMemosContext : DataContext
    {
        public Table<Favorite> Favorites;
        public Table<Marker> Markers;
        public Table<Memo> Memos;
        public Table<MemoTag> MemoTags;
        public Table<Tag> Tags;

        public VoiceMemosContext() : base(string.Empty) { }

        public VoiceMemosContext(string fileOrConnection) : base(fileOrConnection) { }

        public VoiceMemosContext(string fileOrConnection, System.Data.Linq.Mapping.MappingSource mapping) : base(fileOrConnection, mapping) { }

        public static void Initialize(string fileOrConnection)
        {
            using (VoiceMemosContext dbContext = new VoiceMemosContext(fileOrConnection))
            {
                if (dbContext.DatabaseExists() == false)
                {
                    // Create the local database.
                    dbContext.CreateDatabase();

                    // Seed data.
                    Seed(dbContext);

                    // Save changes to database
                    dbContext.SubmitChanges();

                    // Set the database version.
                    SchemaMigrator.SetSchemaVersion(dbContext);
                }
                else
                {
                    SchemaMigrator.ApplyMigrations(dbContext);
                }
            }
        }

        private static void Seed(VoiceMemosContext context)
        {
            // TODO: Add data seed logic if needed.
        }
    }
}
