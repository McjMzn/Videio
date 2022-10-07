using System.Collections.Generic;
using Vidio.FFmpeg;

namespace Vidio.App.Models
{
    public class VideoFileInformation
    {
        public VideoFileInformation()
        {
        }

        public VideoFileInformation(string path, string ffmpegPath = "ffmpeg.exe")
        {
            Path = path;
            Streams = Ffmpeg.IdentifyStreams(path, ffmpegPath);
        }

        public string Path { get; set; }
        public List<StreamInformation> Streams { get; set; }
    }
}
