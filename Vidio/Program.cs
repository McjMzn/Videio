using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Videio.FFmpeg;
using Videio.FFmpeg.Enums;

internal class Program
{
    private static async Task Main(string[] args)
    {
        var s = Ffmpeg.IdentifyStreams(@"E:\Downloads\Oglądanie\Obi Wan Kenobi\Obi-Wan.Kenobi.S01E01.1080p.WEB.h264-KOGi.mkv", @"C:\Users\mazan\source\repos\VideoRecoder\VideoRecoder\bin\Debug\net6.0\ffmpeg.exe");

        const string OutputDirectoryName = "vidio";
        var inputDirectory = args[0];
        
        Directory.CreateDirectory(Path.Combine(inputDirectory, OutputDirectoryName));

        var supportedFormats = new[] { "mkv", "mp4" };
        var files = new List<string>();

        Console.WriteLine($"Loading videos from \"{inputDirectory}\".");
        foreach(var format in supportedFormats)
        {
            files.AddRange(Directory.GetFiles(inputDirectory, $"*{format}"));
        }

        foreach (var file in files)
        {
            Console.WriteLine($"  - {file}");
        }

        foreach (var file in files)
        {
            var fileName = Path.GetFileName(file);
            var outputFilePath = Path.Combine(inputDirectory, OutputDirectoryName, fileName);
            if (File.Exists(outputFilePath))
            {
                continue;
            }

            Ffmpeg
                .LoadFrom(file)
                .UsingAudioCodec(AudioCodec.Aac)
                .UsingAudioChannels(2)
                .UsingAudioBitrate(AudioBitrate.Bitrate192k)
                .UsingVideoCodec(VideoCodec.Copy)
                .SelectStreams(streams => streams
                    .SelectAll(StreamType.Any)
                    .DeselectAll(StreamType.Subtitle)
                    .SelectSubtitles("pol")
                )
                .SaveTo(outputFilePath)
                .Run();
        }
    }
}