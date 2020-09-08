using Creobe.VoiceMemos.Models;
using Creobe.VoiceMemos.Models.Legacy;
using System.Data.Linq;
using System.Linq;

namespace Creobe.VoiceMemos.Data.Legacy.Repositories
{
    public class MemoTagRepository : Repository<MemoTag>
    {
        public MemoTagRepository(DataContext context) :
            base(context) { }

        public void DeleteByMemo(int id)
        {
            var tags = dbContext.GetTable<MemoTag>()
                .Where(t => t.Memo.Id == id);

            dbContext.GetTable<MemoTag>().DeleteAllOnSubmit(tags);
        }

        public void DeleteByMemo(Memo memo)
        {
            dbContext.GetTable<MemoTag>().DeleteAllOnSubmit(memo.Tags);
        }

        public void DeleteByTag(int id)
        {
            var tags = dbContext.GetTable<MemoTag>()
                .Where(t => t.Tag.Id == id);

            dbContext.GetTable<MemoTag>().DeleteAllOnSubmit(tags);
        }
    }
}
