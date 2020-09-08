using Creobe.VoiceMemos.Models;
using Creobe.VoiceMemos.Models.Legacy;
using System;
using System.Data.Linq;
using System.Linq;

namespace Creobe.VoiceMemos.Data.Legacy.Repositories
{
    public class FavoriteRepository : Repository<Favorite>
    {
        public FavoriteRepository(DataContext context) :
            base(context) { }

        public Favorite FindByMemo(int id)
        {
            var favorite = dbContext.GetTable<Favorite>()
                .Where(f => f.Memo.Id == id)
                .FirstOrDefault();

            return favorite;
        }

        public Favorite FindByMemo(Memo memo)
        {
            return FindByMemo(memo.Id);
        }

        public void DeleteByMemo(int id)
        {
            var favorite = FindByMemo(id);

            if (favorite != null)
                Delete(favorite);
        }

        public void DeleteByMemo(Memo memo)
        {
            DeleteByMemo(memo.Id);
        }

        public bool IsMemoInFavorite(int id)
        {
            return FindByMemo(id) != null;
        }

        public bool IsMemoInFavorite(Memo memo)
        {
            return IsMemoInFavorite(memo.Id);
        }
    }
}
