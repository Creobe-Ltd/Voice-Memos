using System;

namespace Creobe.VoiceMemos.Core.Media
{
    public class SampleReceivedEventArgs : EventArgs
    {
        public virtual float Level { get; set; }
        public virtual int Duration { get; set; }
    }
}
