#pragma once

#include <string>
#include <algorithm>

#include "lame.h"

using namespace Platform;
using namespace Platform::Metadata;

namespace Creobe
{
	namespace VoiceMemos
	{
		namespace Media
		{
			namespace Lame
			{
				public enum class LAMEPreset : int
				{
					ABR_8 = 8,
					ABR_16 = 16,
					ABR_32 = 32,
					ABR_48 = 48,
					ABR_64 = 64,
					ABR_96 = 96,
					ABR_128 = 128,
					ABR_160 = 160,
					ABR_256 = 256,
					ABR_320 = 320,				

					V9 = 410,
					VBR_10 = 410,
					V8 = 420,
					VBR_20 = 420,
					V7 = 430,
					VBR_30 = 430,
					V6 = 440,
					VBR_40 = 440,
					V5 = 450,
					VBR_50 = 450,
					V4 = 460,
					VBR_60 = 460,
					V3 = 470,
					VBR_70 = 470,
					V2 = 480,
					VBR_80 = 480,
					V1 = 490,
					VBR_90 = 490,
					V0 = 500,
					VBR_100 = 500,					

					R3MIX = 1000,
					STANDARD = 1001,
					EXTREME = 1002,
					INSANE = 1003,
					STANDARD_FAST = 1004,
					EXTREME_FAST = 1005,
					MEDIUM = 1006,
					MEDIUM_FAST = 1007
				};

				[Flags]
				public enum class MPEGMode: uint32
				{
					Stereo = 0,
					DualChannel = 2,
					Mono = 3,
					NotSet = 4,
				};

				[Flags]
				public enum class ASMOptimizations : uint32
				{
					MMX = 1,
					AMD_3DNow = 2,
					SSE = 3
				};

				[Flags]
				public enum class VBRMode : uint32
				{
					Off = 0,
					MT,
					RH,
					ABR,
					MTRH,
					Default = MTRH
				};

				public enum class MPEGVersion : int
				{
					MPEG2 = 0,
					MPEG1 = 1,
					MPEG2_5 = 2
				};

				public ref class LibMp3Lame sealed
				{
				public:
					LibMp3Lame();

					virtual ~LibMp3Lame();

#pragma region Properties

#pragma region Library version data

					property String^ LameVersion 
					{ 
						String^ get();
					}

					property String^ LameShortVersion 
					{ 
						String^ get();
					}

					property String^ LameVeryShortVersion 
					{ 
						String^ get();
					}

					property String^ LamePsychoacousticVersion 
					{ 
						String^ get();
					}

					property String^ LameURL 
					{ 
						String^ get();
					}

					property String^ LameOSBitness 
					{ 
						String^ get();
					}

#pragma endregion

#pragma region Input Stream Descriptions

					property uint64 NumSamples
					{
						uint64 get();
						void set(uint64);
					}

					property int InputSampleRate
					{
						int get();
						void set(int);
					}

					property int NumChannels
					{
						int get();
						void set(int);
					}

					property float Scale
					{
						float get();
						void set(float);
					}

					property float ScaleLeft
					{
						float get();
						void set(float);
					}

					property float ScaleRight
					{
						float get();
						void set(float);
					}

					property int OutputSampleRate
					{
						int get();
						void set(int);
					}

#pragma endregion

#pragma region General Control Parameters

					property bool Anaylysis
					{
						bool get();
						void set(bool);
					}

					property bool WriteVBRTag
					{
						bool get();
						void set(bool);
					}

					property bool DecodeOnly
					{
						bool get();
						void set(bool);
					}

					property int Quality
					{
						int get();
						void set(int);
					}

					property MPEGMode Mode
					{
						MPEGMode get();
						void set(MPEGMode);
					}

					property bool ForceMS
					{
						bool get();
						void set(bool);
					}

					property bool UseFreeFormat
					{
						bool get();
						void set(bool);
					}

					property bool FindReplayGain
					{
						bool get();
						void set(bool);
					}

					property bool DecodeOnTheFly
					{
						bool get();
						void set(bool);
					}

					property int NoGapTotal
					{
						int get();
						void set(int);
					}

					property int NoGapCurrentIndex
					{
						int get();
						void set(int);
					}

					property int BitRate
					{
						int get();
						void set(int);
					}

					property float CompressionRatio
					{
						float get();
						void set(float);
					}

					bool SetPreset(LAMEPreset preset);

					bool SetOptimization(ASMOptimizations opt, bool enabled);

#pragma endregion

#pragma region Frame parameters

					property bool Copyright
					{
						bool get();
						void set(bool);
					}

					property bool Original
					{
						bool get();
						void set(bool);
					}

					property bool ErrorProtection
					{
						bool get();
						void set(bool);
					}

					property bool Extension
					{
						bool get();
						void set(bool);
					}

					property bool StrictISO
					{
						bool get();
						void set(bool);
					}
#pragma endregion

#pragma region Quantization/Noise Shaping

					property bool DisableReservoir 
					{
						bool get();
						void set(bool);
					}

					property int QuantComp 
					{
						int get();
						void set(int);
					}

					property int QuantCompShort 
					{
						int get();
						void set(int);
					}

					property int ExperimentalX 
					{
						int get();
						void set(int);
					}

					property int ExperimentalY 
					{
						int get();
						void set(int);
					}

					property int ExperimentalZ 
					{
						int get();
						void set(int);
					}

					property int ExperimentalNSPsyTune 
					{
						int get();
						void set(int);
					}

					property int MSFix 
					{
						int get();
						void set(int);
					}

#pragma endregion

#pragma region VBR Control

					property VBRMode VBR 
					{
						VBRMode get();
						void set(VBRMode);
					}

					property int VBRQualityLevel 
					{
						int get();
						void set(int);
					}

					property float VBRQuality 
					{
						float get();
						void set(float);
					}

					property int VBRMeanBitrateKbps 
					{
						int get();
						void set(int);
					}

					property int VBRMinBitrateKbps 
					{
						int get();
						void set(int);
					}

					property int VBRMaxBitrateKbps 
					{
						int get();
						void set(int);
					}

					property bool VBRHardMin 
					{
						bool get();
						void set(bool);
					}
#pragma endregion

#pragma region Filtering control

					property int LowPassFreq 
					{
						int get();
						void set(int);
					}

					property int LowPassWidth 
					{ 
						int get();
						void set(int);
					}

					property int HighPassFreq 
					{ 
						int get();
						void set(int);
					}

					property int HighPassWidth 
					{ 
						int get();
						void set(int);
					}

#pragma endregion

#pragma region Internal state variables, read only

					property MPEGVersion Version 
					{ 
						MPEGVersion get();
					}

					property int EncoderDelay 
					{ 
						int get();
					}

					property int EncoderPadding 
					{ 
						int get();
					}

					property int MFSamplesToEncode 
					{ 
						int get();
					}

					property int MP3BufferSize 
					{ 
						int get();
					}

					property int FrameNumber 
					{ 
						int get();
					}

					property int TotalFrames 
					{ 
						int get();
					}

					property int RadioGain 
					{ 
						int get();
					}

					property int AudiophileGain 
					{ 
						int get();
					}

					property float PeakSample 
					{ 
						float get();
					}

					property int NoClipGainChange 
					{ 
						int get();
					}

					property float NoClipScale 
					{ 
						float get();
					}

#pragma endregion

#pragma endregion

#pragma region Methods

					bool InitParams();

					int Write(const Array<short>^ samples, int nSamples, WriteOnlyArray<unsigned char>^ output, int outputSize, bool mono);

					int Write(const Array<float>^ samples, int nSamples, WriteOnlyArray<unsigned char>^ output, int outputSize, bool mono);

					int Flush(WriteOnlyArray<unsigned char>^ output, int outputSize);

#pragma endregion

				private:
					lame_t lame;
				};
			}
		}
	}
}