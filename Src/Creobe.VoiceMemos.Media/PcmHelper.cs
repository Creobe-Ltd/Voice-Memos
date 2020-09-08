using System;

namespace Creobe.VoiceMemos.Media
{
    public class PcmHelper
    {
        public static float GetPeak(byte[] buffer, int length)
        {
            float level = 0;

            for (int index = 0; index < length; index += 2)
            {
                float sample = BitConverter.ToInt16(buffer, index) / 32768f;

                level = Math.Max(level, sample);
            }

            return level;
        }

        public static int GetDuration(int sizeInBytes, int sampleRate, int audioChannels = 1, int bitsPerSample = 16)
        {
            int duration = (sizeInBytes / (sampleRate * (bitsPerSample / 8))) / audioChannels;

            return duration;
        }
    }
}
