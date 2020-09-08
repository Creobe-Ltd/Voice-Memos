using Creobe.VoiceMemos.Models;
using Creobe.VoiceMemos.Models.Legacy;
using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Creobe.VoiceMemos.Data.Legacy.Repositories
{
    public class MakerRepository : Repository<Marker>
    {
        public MakerRepository(DataContext context) :
            base(context) { }

        public void DeleteByMemo(int id)
        {
            var markers = dbContext.GetTable<Marker>()
                .Where(t => t.Memo.Id == id);

            dbContext.GetTable<Marker>().DeleteAllOnSubmit(markers);
        }

        public void DeleteByMemo(Memo memo)
        {
            dbContext.GetTable<Marker>().DeleteAllOnSubmit(memo.Markers);
        }
    }
}
