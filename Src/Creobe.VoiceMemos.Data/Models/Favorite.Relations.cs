using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Creobe.VoiceMemos.Data.Models
{
    public partial class Favorite
    {
        #region Memo Relational Property

        private Memo _memo;

        [XmlIgnore]
        public Memo Memo
        {
            get
            {
                return _memo ?? (_memo = VoiceMemosDatabase.Memos.LoadByKey(MemoFK));
            }
            set
            {
                _memo = value;
                SyncMemoFK();

            }
        }


        void SyncMemoFK()
        {
            MemoFK = _memo.Id;
        }

        #endregion

    }
}
