using System;
using System.Windows.Input;
using VideIO.FFmpeg;
using VideIO.FFmpeg.Enums;

namespace VideIO.App.Command
{
    public class SelectOptionCommand<T> : ICommand
    {
        private readonly Ffmpeg ffmpeg;
        private readonly T option;
        private readonly Action<T> callback;

        public event EventHandler CanExecuteChanged;

        public SelectOptionCommand(Ffmpeg ffmpeg, T codec, Action<T> callback)
        {
            this.ffmpeg = ffmpeg;
            this.option = codec;
            this.callback = callback;
        }

        public bool CanExecute(object parameter) => true;

        public void Execute(object parameter)
        {
            switch (this.option)
            {
                case AudioCodec audioCodec:
                    this.ffmpeg.UsingAudioCodec(audioCodec);
                    break;

                case VideoCodec videoCodec:
                    this.ffmpeg.UsingVideoCodec(videoCodec);
                    break;

                default:
                    throw new NotImplementedException();
            }

            this.callback(this.option);
        }
    }
}
