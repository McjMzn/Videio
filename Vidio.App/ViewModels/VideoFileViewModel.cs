using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Input;
using Avalonia.Threading;
using ReactiveUI;
using Vidio.App.Command;
using Vidio.FFmpeg;
using Vidio.FFmpeg.Enums;

namespace Vidio.App.ViewModels
{
    public class VideoFileViewModel : ViewModelBase
    {
        private List<StreamViewModel> streams;
        private StringBuilder outputBuilder;

        public VideoFileViewModel() { }

        public VideoFileViewModel(string videoFilePath, string ffmpegExecutablePath, Action<VideoFileViewModel> onClose)
        {
            this.Ffmpeg = Ffmpeg.LoadFrom(videoFilePath, ffmpegExecutablePath);
            this.SetOutputFile();
            
            this.Ffmpeg.ArgumentsChanged += (arguments) => this.RaisePropertyChanged(nameof(this.FfmpegArguments));
            this.Ffmpeg.UsingAudioCodec(Vidio.FFmpeg.Enums.AudioCodec.Copy).UsingVideoCodec(Vidio.FFmpeg.Enums.VideoCodec.Copy).PreservingMetadata();
            this.AudioCodec = new OptionViewModel<AudioCodec>(audioCodec => Ffmpeg.UsingAudioCodec(audioCodec), Enum.GetValues(typeof(AudioCodec)).Cast<AudioCodec>());
            this.VideoCodec = new OptionViewModel<VideoCodec>(videoCodec => Ffmpeg.UsingVideoCodec(videoCodec), Enum.GetValues(typeof(VideoCodec)).Cast<VideoCodec>());
            this.AudioChannels = new OptionViewModel<uint>(audioChannels => Ffmpeg.UsingAudioChannels(audioChannels), new Dictionary<uint, string> { [0] = "Copy", [1] = "Mono", [2] = "Stereo", [6] = "5.1" });

            this.CloseCommand = new ActionCommand(() => onClose(this));
            
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
                        if (args.Data is null)
                        {
                            return;
                        }

                        var timeText = Regex.Match(args.Data, @"time=(?<time>\S*)");
                        if (timeText.Success)
                        {
                            var time = TimeSpan.Parse(timeText.Groups["time"].Value);
                            var duration = this.Ffmpeg.Streams.First(s => s.StreamType == StreamType.Video).Duration;
                            this.Progress = 100 * time / duration.Value;
                            this.RaisePropertyChanged(nameof(this.Progress));
                        }

                        this.outputBuilder.AppendLine(args.Data);
                        this.RaisePropertyChanged(nameof(this.ProcessOutput));
                    });
                };

                process.BeginErrorReadLine();
                process.BeginOutputReadLine();
            });

            this.ToggleMetadataCommand = new ActionCommand(() =>
            {
                this.Ffmpeg.PreservingMetadata(!this.Ffmpeg.PreserveMetadata);
                this.RaisePropertyChanged(nameof(this.PreserveMetadata));
            });
        }
        
        public ICommand ConvertCommand { get; }
        public ICommand ToggleMetadataCommand { get; }
        public ICommand CloseCommand { get; }
        public Ffmpeg Ffmpeg { get; }
        public string FfmpegArguments => this.FormatArguments(this.Ffmpeg.CurrentArguments);
        public OptionViewModel<AudioCodec> AudioCodec { get; set; }
        public OptionViewModel<uint> AudioChannels { get; set; }
        public OptionViewModel<VideoCodec> VideoCodec { get; set; }
        public List<StreamViewModel> Streams => this.GetStreams();
        public string ProcessOutput => this.outputBuilder?.ToString();
        public bool PreserveMetadata => this.Ffmpeg.PreserveMetadata;
        public double Progress { get; set; } = 0;

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

        private void SetOutputFile(string outputFilePath = null)
        {
            if (outputFilePath is not null)
            {
                this.Ffmpeg.SaveTo(outputFilePath);
                return;
            }

            var inputFile = this.Ffmpeg.InputFilePath;
            var inputFileDirectory = Path.GetDirectoryName(inputFile);
            var inputFileName = Path.GetFileName(inputFile);

            var outputDirecory = Path.Combine(inputFileDirectory, "Vidio");
            Directory.CreateDirectory(outputDirecory);
            var outputFile = Path.Combine(outputDirecory, inputFileName);

            this.Ffmpeg.SaveTo(outputFile);
        }

        private string FormatArguments(string arguments)
        {
            var toProcess = arguments;
            var builder = new StringBuilder(this.Ffmpeg.ExecutablePath);
            while (toProcess.Count() > 0)
            {
                var index = toProcess.IndexOf(" -");
                builder.Append($"{Environment.NewLine}  {(index >= 0 ? toProcess.Substring(0, index) : toProcess)}");


                toProcess = index < 0 ? string.Empty : toProcess.Substring(index).TrimStart();
            }

            return builder.ToString().Replace($"{this.Ffmpeg.OutputFilePath}", $"{Environment.NewLine}  {this.Ffmpeg.OutputFilePath}");
        }
    }
}
