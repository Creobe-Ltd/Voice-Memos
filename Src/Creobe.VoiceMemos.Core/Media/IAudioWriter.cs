using System.Threading.Tasks;

namespace Creobe.VoiceMemos.Core.Media
{
    public interface IAudioWriter
    {
        bool CanWrite { get; }
        int Write(byte[] buffer, int offset, int count);
        Task<int> WriteAsync(byte[] buffer, int offset, int count);
        void Flush();
        Task FlushAsync();
    }
}
