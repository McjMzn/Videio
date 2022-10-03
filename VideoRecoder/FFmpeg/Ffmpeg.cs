namespace VideIO.FFmpeg;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using VideIO.FFmpeg.Enums;

public class Ffmpeg : IFluentFfmpeg
{
    private string executablePath;
    private string inputFilePath;
    private VideoCodec? videoCodec;
    private AudioCodec? audioCodec;
    private uint? audioChannels;
    private AudioBitrate? audioBitrate;
    private string outputFile;
    private FfmpegMap map;
    private bool preserveMetadata;

    public event Action<string> ArgumentsChanged;

    private Ffmpeg() { }

    public static List<StreamInformation> IdentifyStreams(string inputFilePath, string ffmpegExecutablePath = "ffmpeg.exe")
    {
        var processInfo = new ProcessStartInfo(ffmpegExecutablePath, $"-i \"{inputFilePath}\"")
        {
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true,
            StandardErrorEncoding = Encoding.UTF8,
        };

        var process = Process.Start(processInfo);
        var error = process.StandardError.ReadToEnd();
        var output = process.StandardOutput.ReadToEnd();

        var streamInformations = new List<StreamInformation>();

        var streams = Regex.Matches(error, @"Stream #").Cast<Match>().ToList();
        for (var i = 0; i < streams.Count - 1; i++)
        {
            var stream = error.Substring(streams[i].Index, streams[i + 1].Index - streams[i].Index);
            var match = Regex.Match(stream, @"Stream #(?<inputIndex>\d+):(?<streamIndex>\d+)(\((?<languageCode>\w{3})\))?: (?<streamType>\w+): (?<format>[^,\n]+)");
            var title = Regex.Match(stream, @"title\s+: (?<title>.*)");

            streamInformations.Add(new StreamInformation
            {
                InputIndex = uint.Parse(match.Groups["inputIndex"].Value.Trim()),
                StreamIndex = uint.Parse(match.Groups["streamIndex"].Value.Trim()),
                LanguageCode = match.Groups["languageCode"].Value.Trim(),
                StreamType = Enum.Parse<StreamType>(match.Groups["streamType"].Value.Trim()),
                Format = match.Groups["format"].Value.Trim(),
                Title = title.Success ? title.Groups["title"].Value.Trim() : null
            });
        }

        return streamInformations;
    }

    public string CurrentArguments => GetFfmpegArguments();

    public string InputFilePath => this.inputFilePath;

    public IReadOnlyList<StreamInformation> Streams { get; private set; }

    public static Ffmpeg LoadFrom(string inputFile, string ffmpegExecutablePath = "ffmpeg.exe")
    {
        var streams = Ffmpeg.IdentifyStreams(inputFile, ffmpegExecutablePath);
        var ffmpeg = new Ffmpeg
        {
            Streams = streams,
            executablePath = ffmpegExecutablePath,
            inputFilePath = inputFile
        };

        return ffmpeg;
    }

    public IFluentFfmpeg UsingAudioCodec(AudioCodec audioCodec)
    {
        this.audioCodec = audioCodec;
        return InvokeArgumentsChangedAndReturn();
    }

    public IFluentFfmpeg UsingVideoCodec(VideoCodec videoCodec)
    {
        this.videoCodec = videoCodec;
        return InvokeArgumentsChangedAndReturn();
    }

    public IFluentFfmpeg UsingAudioBitrate(AudioBitrate audioBitrate)
    {
        this.audioBitrate = audioBitrate;
        return InvokeArgumentsChangedAndReturn();
    }

    public IFluentFfmpeg UsingAudioChannels(uint audioChannels)
    {
        this.audioChannels = audioChannels;
        return InvokeArgumentsChangedAndReturn();
    }

    public IFluentFfmpeg SelectStreams(Action<FfmpegMap> mapAction)
    {
        if (map is null)
        {
            map = new FfmpegMap(0);
        }

        mapAction(map);
        return InvokeArgumentsChangedAndReturn();
    }

    public IFluentFfmpeg SaveTo(string outputFile)
    {
        this.outputFile = $"\"{outputFile}\"";
        return InvokeArgumentsChangedAndReturn();
    }

    public IFluentFfmpeg PreserveMetadata()
    {
        this.preserveMetadata = true;
        return this;
    }

    public void Run(Action<Process> processAction = null)
    {
        var arguments = GetFfmpegArguments();
        var processStartInfo = new ProcessStartInfo(this.executablePath, arguments)
        {
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true
        };

        var process = Process.Start(processStartInfo);
        processAction?.Invoke(process);

        process.WaitForExit();
    }

    private Ffmpeg InvokeArgumentsChangedAndReturn()
    {
        ArgumentsChanged?.Invoke(GetFfmpegArguments());
        return this;
    }

    private void AddSpaceAndAppendIfNotEmpty(StringBuilder stringBuilder, string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return;
        }

        stringBuilder.Append($" {text}");
    }

    private string GetFfmpegArguments()
    {
        var argumentsBuilder = new StringBuilder();

        argumentsBuilder.Append($"-i \"{inputFilePath}\"");

        AddSpaceAndAppendIfNotEmpty(argumentsBuilder, this.videoCodec is not null ? $"-vcodec {videoCodec.ToString().ToLower()}" : null);
        AddSpaceAndAppendIfNotEmpty(argumentsBuilder, this.audioCodec is not null ? $"-acodec {this.audioCodec.ToString().ToLower()}" : null);
        AddSpaceAndAppendIfNotEmpty(argumentsBuilder, this.audioChannels is not null ? $"-ac {audioChannels}" : null);
        AddSpaceAndAppendIfNotEmpty(argumentsBuilder, this.audioBitrate is not null ? $"-b:a {audioBitrate.ToString().Replace("Bitrate", string.Empty)}" : null);
        AddSpaceAndAppendIfNotEmpty(argumentsBuilder, this.preserveMetadata ? "-map_metadata 0 -write_id3v2 1" : null);
        AddSpaceAndAppendIfNotEmpty(argumentsBuilder, map?.Build());
        AddSpaceAndAppendIfNotEmpty(argumentsBuilder, outputFile);

        return argumentsBuilder.ToString();
    }
}
