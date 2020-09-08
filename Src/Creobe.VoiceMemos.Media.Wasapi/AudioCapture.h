#pragma once

#include <windows.h>

#include <synchapi.h>
#include <audioclient.h>
#include <phoneaudioclient.h>

namespace Creobe
{
	namespace VoiceMemos
	{
		namespace Media
		{
			namespace Wasapi
			{
				public ref class AudioCapture sealed
				{
				public:
					AudioCapture();
					virtual ~AudioCapture();

					bool StartCapture();
					bool StopCapture();
					int ReadBuffer(Platform::Array<byte>^* buffer);

					void SetCaptureFormat(int sampleRate, WORD channels, WORD bits);
				private:
					HRESULT InitCapture();

					bool started;
					int m_sourceFrameSizeInBytes;

					WAVEFORMATEX* m_waveFormatEx;
					int m_sampleRate;
					WORD m_channels;
					WORD m_bits;

					IAudioClient2* m_pDefaultCaptureDevice;

					IAudioCaptureClient* m_pCaptureClient;
				};
			}
		}
	}
}