#include "pch.h"

#include "lame.h"
#include "LibMp3Lame.h"

using namespace Platform;
using namespace Creobe::VoiceMemos::Media::Lame;

LibMp3Lame::LibMp3Lame()
{
	lame = lame_init();
}

LibMp3Lame::~LibMp3Lame()
{
	if(!NULL(lame))
	{
		lame_close(lame);
		lame = NULL;
	}
}

#pragma region Properties

#pragma region Library version data

String^ LibMp3Lame::LameVersion::get()
{
	std::string s_str = std::string(get_lame_version());
	std::wstring w_str = std::wstring(s_str.begin(), s_str.end());
	
	return ref new String(w_str.c_str());
}

String^ LibMp3Lame::LameShortVersion::get()
{
	std::string s_str = std::string(get_lame_short_version());
	std::wstring w_str = std::wstring(s_str.begin(), s_str.end());
	return ref new String(w_str.c_str());
}

String^ LibMp3Lame::LameVeryShortVersion::get()
{
	std::string s_str = std::string(get_lame_very_short_version());
	std::wstring w_str = std::wstring(s_str.begin(), s_str.end());
	return ref new String(w_str.c_str());
}

String^ LibMp3Lame::LamePsychoacousticVersion::get()
{
	std::string s_str = std::string(get_psy_version());
	std::wstring w_str = std::wstring(s_str.begin(), s_str.end());
	return ref new String(w_str.c_str());
}

String^ LibMp3Lame::LameURL::get()
{
	std::string s_str = std::string(get_lame_url());
	std::wstring w_str = std::wstring(s_str.begin(), s_str.end());
	return ref new String(w_str.c_str());
}

String^ LibMp3Lame::LameOSBitness::get()
{
	std::string s_str = std::string(get_lame_os_bitness());
	std::wstring w_str = std::wstring(s_str.begin(), s_str.end());
	return ref new String(w_str.c_str());
}

#pragma endregion

#pragma region Input Stream Descriptions

uint64 LibMp3Lame::NumSamples::get()
{
	return lame_get_num_samples(lame);
}

void LibMp3Lame::NumSamples::set(uint64 value)
{
	lame_set_num_samples(lame, value);
}

int LibMp3Lame::InputSampleRate::get()
{
	return lame_get_in_samplerate(lame);
}

void LibMp3Lame::InputSampleRate::set(int value)
{
	lame_set_in_samplerate(lame, value);
}

int LibMp3Lame::NumChannels::get()
{
	return lame_get_num_channels(lame);
}

void LibMp3Lame::NumChannels::set(int value)
{
	lame_set_num_channels(lame, value);
}

float LibMp3Lame::Scale::get()
{
	return lame_get_scale(lame);
}

void LibMp3Lame::Scale::set(float value)
{
	lame_set_scale(lame, value);
}

float LibMp3Lame::ScaleLeft::get()
{
	return lame_get_scale_left(lame);
}

void LibMp3Lame::ScaleLeft::set(float value)
{
	lame_set_scale_left(lame, value);
}

float LibMp3Lame::ScaleRight::get()
{
	return lame_get_scale_right(lame);
}

void LibMp3Lame::ScaleRight::set(float value)
{
	lame_set_scale_right(lame, value);
}

int LibMp3Lame::OutputSampleRate::get()
{
	return lame_get_out_samplerate(lame);
}

void LibMp3Lame::OutputSampleRate::set(int value)
{
	lame_set_out_samplerate(lame, value);
}

#pragma endregion

#pragma region General Control Parameters

bool LibMp3Lame::Anaylysis::get()
{
	return lame_get_analysis(lame);
}

void LibMp3Lame::Anaylysis::set(bool value)
{
	lame_set_analysis(lame, value);
}

bool LibMp3Lame::WriteVBRTag::get()
{
	return lame_get_bWriteVbrTag(lame) != 0;
}

void LibMp3Lame::WriteVBRTag::set(bool value)
{
	lame_set_bWriteVbrTag(lame, value ? 1 : 0);
}

bool LibMp3Lame::DecodeOnly::get()
{
	return lame_get_decode_only(lame) != 0;
}

void LibMp3Lame::DecodeOnly::set(bool value)
{
	lame_set_decode_only(lame, value ? 1 : 0);
}

int LibMp3Lame::Quality::get()
{
	return lame_get_quality(lame);
}

void LibMp3Lame::Quality::set(int value)
{
	lame_set_quality(lame, value);
}

MPEGMode LibMp3Lame::Mode::get()
{
	return static_cast<MPEGMode>(lame_get_mode(lame));
}

void LibMp3Lame::Mode::set(MPEGMode value)
{
	lame_set_mode(lame, static_cast<MPEG_mode>(value));
}

bool LibMp3Lame::ForceMS::get()
{
	return lame_get_force_ms(lame);
}

void LibMp3Lame::ForceMS::set(bool value)
{
	lame_set_force_ms(lame, value);
}

bool LibMp3Lame::UseFreeFormat::get()
{
	return lame_get_free_format(lame);
}

void LibMp3Lame::UseFreeFormat::set(bool value)
{
	lame_set_free_format(lame, value);
}

bool LibMp3Lame::FindReplayGain::get()
{
	return lame_get_findReplayGain(lame);
}

void LibMp3Lame::FindReplayGain::set(bool value)
{
	lame_set_findReplayGain(lame, value);
}

bool LibMp3Lame::DecodeOnTheFly::get()
{
	return lame_get_decode_on_the_fly(lame);
}

void LibMp3Lame::DecodeOnTheFly::set(bool value)
{
	lame_set_decode_on_the_fly(lame, value);
}

int LibMp3Lame::NoGapTotal::get()
{
	return lame_get_nogap_total(lame);
}

void LibMp3Lame::NoGapTotal::set(int value)
{
	lame_set_nogap_total(lame, value);
}

int LibMp3Lame::NoGapCurrentIndex::get()
{
	return lame_get_nogap_currentindex(lame);
}

void LibMp3Lame::NoGapCurrentIndex::set(int value)
{
	lame_set_nogap_currentindex(lame, value);
}

int LibMp3Lame::BitRate::get()
{
	return lame_get_brate(lame);
}

void LibMp3Lame::BitRate::set(int value)
{
	lame_set_brate(lame, value);
}

float LibMp3Lame::CompressionRatio::get()
{
	return lame_get_compression_ratio(lame);
}

void LibMp3Lame::CompressionRatio::set(float value)
{
	lame_set_compression_ratio(lame, value);
}

bool LibMp3Lame::SetPreset(LAMEPreset preset)
{
	int res = lame_set_preset(lame, (int)preset);
	return res == 0;
}

bool LibMp3Lame::SetOptimization(ASMOptimizations opt, bool enabled)
{
	int res = lame_set_asm_optimizations(lame, (int)opt, enabled);
	return res == 0;
}

#pragma endregion

#pragma region Frame parameters

bool LibMp3Lame::Copyright::get()
{
	return lame_get_copyright(lame);
}

void LibMp3Lame::Copyright::set(bool value)
{
	lame_set_copyright(lame, value);
}

bool LibMp3Lame::Original::get()
{
	return lame_get_original(lame);
}

void LibMp3Lame::Original::set(bool value)
{
	lame_set_original(lame, value);
}

bool LibMp3Lame::ErrorProtection::get()
{
	return lame_get_error_protection(lame);
}

void LibMp3Lame::ErrorProtection::set(bool value)
{
	lame_set_error_protection(lame, value);
}

bool LibMp3Lame::Extension::get()
{
	return lame_get_extension(lame);
}

void LibMp3Lame::Extension::set(bool value)
{
	lame_set_extension(lame, value);
}

bool LibMp3Lame::StrictISO::get()
{
	return lame_get_strict_ISO(lame);
}

void LibMp3Lame::StrictISO::set(bool value)
{
	lame_set_strict_ISO(lame, value);
}

#pragma endregion

#pragma region Quantization/Noise Shaping

bool LibMp3Lame::DisableReservoir::get()
{
	return lame_get_disable_reservoir(lame);
}

void LibMp3Lame::DisableReservoir::set(bool value)
{
	lame_set_disable_reservoir(lame, value);
}

int LibMp3Lame::QuantComp::get()
{
	return lame_get_quant_comp(lame);
}

void LibMp3Lame::QuantComp::set(int value)
{
	lame_set_quant_comp(lame, value);
}

int LibMp3Lame::QuantCompShort::get()
{
	return lame_get_quant_comp_short(lame);
}

void LibMp3Lame::QuantCompShort::set(int value)
{
	lame_set_quant_comp_short(lame, value);
}

int LibMp3Lame::ExperimentalX::get()
{
	return lame_get_experimentalX(lame);
}

void LibMp3Lame::ExperimentalX::set(int value)
{
	lame_set_experimentalX(lame, value);
}

int LibMp3Lame::ExperimentalY::get()
{
	return lame_get_experimentalY(lame);
}

void LibMp3Lame::ExperimentalY::set(int value)
{
	lame_set_experimentalY(lame, value);
}

int LibMp3Lame::ExperimentalZ::get()
{
	return lame_get_experimentalZ(lame);
}

void LibMp3Lame::ExperimentalZ::set(int value)
{
	lame_set_experimentalZ(lame, value);
}

int LibMp3Lame::ExperimentalNSPsyTune::get()
{
	return lame_get_exp_nspsytune(lame);
}

void LibMp3Lame::ExperimentalNSPsyTune::set(int value)
{
	lame_set_exp_nspsytune(lame, value);
}

int LibMp3Lame::MSFix::get()
{
	return lame_get_msfix(lame);
}

void LibMp3Lame::MSFix::set(int value)
{
	lame_set_msfix(lame, value);
}

#pragma endregion

#pragma region VBR Control

VBRMode LibMp3Lame::VBR::get()
{
	return static_cast<VBRMode>(lame_get_VBR(lame));
}

void LibMp3Lame::VBR::set(VBRMode value)
{
	lame_set_VBR(lame, static_cast<vbr_mode>(value));
}

int LibMp3Lame::VBRQualityLevel::get()
{
	return lame_get_VBR_q(lame);
}

void LibMp3Lame::VBRQualityLevel::set(int value)
{
	lame_set_VBR_q(lame, value);
}

float LibMp3Lame::VBRQuality::get()
{
	return lame_get_VBR_quality(lame);
}

void LibMp3Lame::VBRQuality::set(float value)
{
	lame_set_VBR_quality(lame, value);
}

int LibMp3Lame::VBRMeanBitrateKbps::get()
{
	return lame_get_VBR_mean_bitrate_kbps(lame);
}

void LibMp3Lame::VBRMeanBitrateKbps::set(int value)
{
	lame_set_VBR_mean_bitrate_kbps(lame, value);
}

int LibMp3Lame::VBRMinBitrateKbps::get()
{
	return lame_get_VBR_min_bitrate_kbps(lame);
}

void LibMp3Lame::VBRMinBitrateKbps::set(int value)
{
	lame_set_VBR_min_bitrate_kbps(lame, value);
}

int LibMp3Lame::VBRMaxBitrateKbps::get()
{
	return lame_get_VBR_max_bitrate_kbps(lame);
}

void LibMp3Lame::VBRMaxBitrateKbps::set(int value)
{
	lame_set_VBR_max_bitrate_kbps(lame, value);
}

bool LibMp3Lame::VBRHardMin::get()
{
	return lame_get_VBR_hard_min(lame);
}

void LibMp3Lame::VBRHardMin::set(bool value)
{
	lame_set_VBR_hard_min(lame, value);
}

#pragma endregion

#pragma region Filtering control

int LibMp3Lame::LowPassFreq::get()
{
	return lame_get_lowpassfreq(lame);
}

void LibMp3Lame::LowPassFreq::set(int value)
{
	lame_set_lowpassfreq(lame, value);
}

int LibMp3Lame::LowPassWidth::get()
{
	return lame_get_lowpasswidth(lame);
}

void LibMp3Lame::LowPassWidth::set(int value)
{
	lame_set_lowpasswidth(lame, value);
}

int LibMp3Lame::HighPassFreq::get()
{
	return lame_get_highpassfreq(lame);
}

void LibMp3Lame::HighPassFreq::set(int value)
{
	lame_set_highpassfreq(lame, value);
}

int LibMp3Lame::HighPassWidth::get()
{
	return lame_get_highpasswidth(lame);
}

void LibMp3Lame::HighPassWidth::set(int value)
{
	lame_set_highpasswidth(lame, value);
}

#pragma endregion

#pragma region Internal state variables, read only

MPEGVersion LibMp3Lame::Version::get()
{
	return static_cast<MPEGVersion>(lame_get_version(lame));
}

int LibMp3Lame::EncoderDelay::get()
{
	return lame_get_encoder_delay(lame);
}

int LibMp3Lame::EncoderPadding::get()
{
	return lame_get_encoder_padding(lame);
}

int LibMp3Lame::MFSamplesToEncode::get()
{
	return lame_get_mf_samples_to_encode(lame);
}

int LibMp3Lame::MP3BufferSize::get()
{
	return lame_get_size_mp3buffer(lame);
}

int LibMp3Lame::FrameNumber::get()
{
	return lame_get_frameNum(lame);
}

int LibMp3Lame::TotalFrames::get()
{
	return lame_get_totalframes(lame);
}

int LibMp3Lame::RadioGain::get()
{
	return lame_get_RadioGain(lame);
}

int LibMp3Lame::AudiophileGain::get()
{
	return lame_get_AudiophileGain(lame);
}

float LibMp3Lame::PeakSample::get()
{
	return lame_get_PeakSample(lame);
}

int LibMp3Lame::NoClipGainChange::get()
{
	return lame_get_noclipGainChange(lame);
}

float LibMp3Lame::NoClipScale::get()
{
	return lame_get_noclipScale(lame);
}

#pragma endregion

#pragma endregion

#pragma region Methods

bool LibMp3Lame::InitParams()
{
	if(NULL(lame))
		throw ref new NullReferenceException("InitParams called without initializing lame");

	int res = lame_init_params(lame);
	return res == 0;
}

int LibMp3Lame::Write(const Array<short>^ samples, int nSamples, WriteOnlyArray<unsigned char>^ output, int outputSize, bool mono)
{
	int rc = -1;

	if (mono)
		rc = lame_encode_buffer(lame, samples->Data, samples->Data, nSamples, output->Data, outputSize);
	else
		rc = lame_encode_buffer_interleaved(lame, samples->Data, nSamples / 2, output->Data, outputSize);

	return rc;
}

int LibMp3Lame::Write(const Array<float>^ samples, int nSamples, WriteOnlyArray<unsigned char>^ output, int outputSize, bool mono)
{
	int rc = -1;

	if (mono)
		rc = lame_encode_buffer_ieee_float(lame, samples->Data, samples->Data, nSamples, output->Data, outputSize);
	else
		rc = lame_encode_buffer_interleaved_ieee_float(lame, samples->Data, nSamples / 2, output->Data, outputSize);

	return rc;
}

int LibMp3Lame::Flush(WriteOnlyArray<unsigned char>^ output, int outputSize)
{
	int res = lame_encode_flush(lame, output->Data, outputSize);
	return std::max(0, res);
}

#pragma endregion