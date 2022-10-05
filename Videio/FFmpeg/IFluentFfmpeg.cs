namespace Videio.FFmpeg;

using System;
using System.Diagnostics;
using Videio.FFmpeg.Enums;

public interface IFluentFfmpeg
{
    IFluentFfmpeg UsingAudioCodec(AudioCodec audioCodec);
    IFluentFfmpeg UsingVideoCodec(VideoCodec videoCodec);
    IFluentFfmpeg UsingAudioBitrate(AudioBitrate audioBitrate);
    IFluentFfmpeg UsingAudioChannels(uint audioChannels);
    IFluentFfmpeg SelectStreams(Action<FfmpegMap> mapAction);
    IFluentFfmpeg SaveTo(string outputFile);
    IFluentFfmpeg PreservingMetadata(bool preserveMetadata = true);
    void Run(Action<Process> processAction = null);
}
