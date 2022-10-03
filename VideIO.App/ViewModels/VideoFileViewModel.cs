using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Avalonia.Threading;
using ReactiveUI;
using VideIO.App.Command;
using VideIO.FFmpeg;
using VideIO.FFmpeg.Enums;

namespace VideIO.App.ViewModels
{
    public class VideoFileViewModel : ViewModelBase
    {
        private List<StreamViewModel> streams;
        private StringBuilder outputBuilder;

        public VideoFileViewModel() { }

        public VideoFileViewModel(string videoFilePath, string ffmpegExecutablePath)
        {
            this.Ffmpeg = Ffmpeg.LoadFrom(videoFilePath, ffmpegExecutablePath);
            this.Ffmpeg.ArgumentsChanged += (arguments) => this.RaisePropertyChanged(nameof(this.FfmpegArguments));
            this.Ffmpeg.UsingAudioCodec(FFmpeg.Enums.AudioCodec.Copy).UsingVideoCodec(FFmpeg.Enums.VideoCodec.Copy).PreserveMetadata();
            this.AudioCodec = new EnumerableOptionViewModel<AudioCodec>(Ffmpeg);
            this.VideoCodec = new EnumerableOptionViewModel<VideoCodec>(Ffmpeg);

            this.ConvertCommand = new ConvertCommand(this.Ffmpeg, process =>
            {
                this.outputBuilder = new StringBuilder();

                process.OutputDataReceived += (sender, args) =>
                {
                    Dispatcher.UIThread.Post(() =>
                    {
                        this.outputBuilder.AppendLine(args.Data);
                        this.RaisePropertyChanged(nameof(this.ProcessOutput));
                    });
                };

                process.ErrorDataReceived += (sender, args) =>
                {
                    Dispatcher.UIThread.Post(() =>
                    {
                        this.outputBuilder.AppendLine(args.Data);
                        this.RaisePropertyChanged(nameof(this.ProcessOutput));
                    });
                };

                process.BeginErrorReadLine();
                process.BeginOutputReadLine();
            });
        }
        
        public ICommand ConvertCommand { get; }
        public Ffmpeg Ffmpeg { get; }
        public string FfmpegArguments => this.Ffmpeg.CurrentArguments;
        public EnumerableOptionViewModel<AudioCodec> AudioCodec { get; set; }
        public EnumerableOptionViewModel<VideoCodec> VideoCodec { get; set; }
        public List<StreamViewModel> Streams => this.GetStreams();
        public string ProcessOutput => this.outputBuilder?.ToString();

        private List<StreamViewModel> GetStreams()
        {
            if (this.streams is null)
            {
                this.streams = new List<StreamViewModel>();
                foreach (var stream in this.Ffmpeg.Streams)
                {
                    this.streams.Add(new StreamViewModel(stream, this.Ffmpeg));
                }
            }

            return this.streams;
        }

    }
}
