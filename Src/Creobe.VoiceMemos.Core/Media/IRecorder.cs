using System;

namespace Creobe.VoiceMemos.Core.Media
{
    public interface IRecorder
    {
        int SampleRate { get; }
        int Channels { get; }
        int BitsRate { get; }
        EncodingFormat EncodingFormat { get; }
        int EncodingPreset { get; }
        int Size { get; }
        int Duration { get; }
        RecorderState State { get; }
        bool IsCapturing { get; }
        bool SkipSilence { get; set; }

        void Start();
        void Stop();
        void Pause();
        void PauseCapture();
        void ResumeCapture();
        void Resume();

        event EventHandler<SampleReceivedEventArgs> SampleReceived;
    }
}
