#include "pch.h"
#include "AudioCapture.h"

using namespace Creobe::VoiceMemos::Media::Wasapi;
using namespace Platform;
using namespace Windows::System::Threading;

#define MAX_RAW_BUFFER_SIZE 1024*128

void FillPcmFormat(WAVEFORMATEX& format, WORD channels, int sampleRate, WORD bits)
{
	format.wFormatTag = WAVE_FORMAT_PCM;
	format.nChannels = channels;
	format.nSamplesPerSec = sampleRate;
	format.wBitsPerSample = bits;
	format.nBlockAlign       = format.nChannels * (format.wBitsPerSample / 8);
    format.nAvgBytesPerSec   = format.nSamplesPerSec * format.nBlockAlign;
    format.cbSize            = 0;
}

AudioCapture::AudioCapture():
	m_waveFormatEx(NULL),
	m_sampleRate(44100),
	m_channels(2),
	m_bits(16),
	m_pDefaultCaptureDevice(NULL),
	m_pCaptureClient(NULL),
	m_sourceFrameSizeInBytes(0),
	started(false)
{
}

AudioCapture::~AudioCapture()
{
}

bool AudioCapture::StartCapture()
{
    bool ret = false;

    if (!started)
    {
        HRESULT hr = InitCapture();
        if (SUCCEEDED(hr))
        {
            ret = started = true;
        }
    }

    return ret;
}

bool AudioCapture::StopCapture()
{
    bool ret = false;

    if (started)
    {
        HRESULT hr = S_OK;

        if (m_pDefaultCaptureDevice)
        {
            hr = m_pDefaultCaptureDevice->Stop();
        }

        if (m_pCaptureClient)
        {
            m_pCaptureClient->Release();
            m_pCaptureClient = NULL;
        }

        if (m_pDefaultCaptureDevice)
        {
            m_pDefaultCaptureDevice->Release();
            m_pDefaultCaptureDevice = NULL;
        }

        if (m_waveFormatEx)
        {
            CoTaskMemFree((LPVOID)m_waveFormatEx);
            m_waveFormatEx = NULL;
        }

        if (SUCCEEDED(hr))
        {
            started = false;
            ret = true;
        }
    }

    return ret;
}

int AudioCapture::ReadBuffer(Platform::Array<byte>^* byteArray)
{
    int ret = 0;
    if (!started) return ret;

    BYTE *tempBuffer = new BYTE[MAX_RAW_BUFFER_SIZE];
    UINT32 packetSize = 0;
    HRESULT hr = S_OK;
    long accumulatedBytes = 0;

    if (tempBuffer)
    {
        hr = m_pCaptureClient->GetNextPacketSize(&packetSize);

        while (SUCCEEDED(hr) && packetSize > 0)
        {
            BYTE* packetData = nullptr;
            UINT32 frameCount = 0;
            DWORD flags = 0;
            if (SUCCEEDED(hr))
            {
                hr = m_pCaptureClient->GetBuffer(&packetData, &frameCount, &flags, nullptr, nullptr);
                unsigned int incomingBufferSize = frameCount * m_sourceFrameSizeInBytes;

                memcpy(tempBuffer + accumulatedBytes, packetData, incomingBufferSize);
                accumulatedBytes += incomingBufferSize;
            }

            if (SUCCEEDED(hr))
            {
                hr = m_pCaptureClient->ReleaseBuffer(frameCount);
            }

            if (SUCCEEDED(hr))
            {
                hr = m_pCaptureClient->GetNextPacketSize(&packetSize);
            }
        }

        // Copy the available capture data to the array.
        auto temp = ref new Platform::Array<byte>(accumulatedBytes);
        for(long i = 0; i < accumulatedBytes; i++)
        {
                temp[i] = tempBuffer[i];
        }
        *byteArray = temp;
        ret = accumulatedBytes;

        // Reset byte counter
        accumulatedBytes = 0;
    }

    delete[] tempBuffer;

    return ret;
}

void AudioCapture::SetCaptureFormat(int sampleRate, WORD channels, WORD bits)
{
	m_sampleRate = sampleRate;
	m_channels = channels;
	m_bits = bits;
}

HRESULT AudioCapture::InitCapture()
{
    HRESULT hr = E_FAIL;

    LPCWSTR captureId = GetDefaultAudioCaptureId(AudioDeviceRole::Communications);
	
    if (NULL == captureId)
    {
        hr = E_FAIL;
    }
    else
    {
        hr = ActivateAudioInterface(captureId, __uuidof(IAudioClient2), (void**)&m_pDefaultCaptureDevice);
    }

    if (SUCCEEDED(hr))
    {
        hr = m_pDefaultCaptureDevice->GetMixFormat(&m_waveFormatEx);
		
		
    }

    // Set the category through SetClientProperties
    AudioClientProperties properties = {};
    if (SUCCEEDED(hr))
    {
        properties.cbSize = sizeof AudioClientProperties;
        properties.eCategory = AudioCategory_Other;
        // Note that AudioCategory_Other is the only valid category for capture and loopback streams.
        // From: http://msdn.microsoft.com/en-us/library/windows/desktop/hh404178(v=vs.85).aspx
        hr = m_pDefaultCaptureDevice->SetClientProperties(&properties);
    }

    if (SUCCEEDED(hr))
    {
        WAVEFORMATEX temp;
        FillPcmFormat(temp, m_channels, m_sampleRate, m_bits); // stereo, 44100 Hz, 16 bit

        *m_waveFormatEx = temp;
        m_sourceFrameSizeInBytes = (m_waveFormatEx->wBitsPerSample / 8) * m_waveFormatEx->nChannels;

        // using device to capture stereo requires the flag 0x8800000, or at least some part of it
        hr = m_pDefaultCaptureDevice->Initialize(AUDCLNT_SHAREMODE_SHARED, 0x88000000, 1000 * 10000, 0, m_waveFormatEx, NULL);
    }

    if (SUCCEEDED(hr))
    {
        hr = m_pDefaultCaptureDevice->GetService(__uuidof(IAudioCaptureClient), (void**)&m_pCaptureClient);
    }

    if (SUCCEEDED(hr))
    {
        hr = m_pDefaultCaptureDevice->Start();
    }

    if (captureId)
    {
        CoTaskMemFree((LPVOID)captureId);
    }

    return hr;
}