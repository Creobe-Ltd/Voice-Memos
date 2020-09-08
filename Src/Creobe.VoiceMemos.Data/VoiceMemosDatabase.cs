using Creobe.VoiceMemos.Data.Models;
using Lex.Db;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Creobe.VoiceMemos.Data
{
    public static class VoiceMemosDatabase
    {
        public static DbInstance Instance { get { return _db ?? (_db = CreateDb()); } }
        static DbInstance _db;

        private static DbTable<Memo> _memos;

        public static DbTable<Memo> Memos
        {
            get { return _memos ?? (_memos = Instance.Table<Memo>()); }
        }

        private static DbTable<Tag> _tags;

        public static DbTable<Tag> Tags
        {
            get { return _tags ?? (_tags = Instance.Table<Tag>()); }
        }

        private static DbTable<Marker> _markers;

        public static DbTable<Marker> Markers
        {
            get { return _markers ?? (_markers = Instance.Table<Marker>()); }
        }

        private static DbTable<Favorite> _favorites;

        public static DbTable<Favorite> Favorites
        {
            get { return _favorites ?? (_favorites = Instance.Table<Favorite>()); }
        }

        static DbInstance CreateDb(bool purge = false)
        {
            var result = new DbInstance("Database");

            result.Map<Memo>().Automap(i => i.Id, true);
            result.Map<Tag>().Automap(i => i.Id, true);
            result.Map<Marker>().Automap(i => i.Id, true);
            result.Map<Favorite>().Automap(i => i.Id, true);

            result.Initialize();

            if (purge)
                result.Purge();

            return result;
        }
    }
}
