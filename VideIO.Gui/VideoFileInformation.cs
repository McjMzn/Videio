using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideIO.Gui
{
    public class VideoFileInformation
    {
        public VideoFileInformation(string path)
        {
            this.Path = path;
            this.Streams = Ffmpeg.IdentifyStreams(path, @"C:\Users\mazan\source\repos\VideoRecoder\VideoRecoder\bin\Debug\net6.0\ffmpeg.exe");
        }

        public string Path { get; set; }
        public List<StreamInformation> Streams { get; set; }
    }
}
