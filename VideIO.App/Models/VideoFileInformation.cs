using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Videio.FFmpeg;

namespace Videio.App.Models
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
