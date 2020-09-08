using System;

namespace Creobe.VoiceMemos.Core.Data
{
    public interface IEntity
    {
        int Id { get; set; }
        DateTime? CreatedDate { get; set; }
        DateTime? ModifiedDate { get; set; }
    }
}
