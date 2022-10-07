using System;
using System.Windows.Input;
using Videio.FFmpeg;

namespace Videio.App.Command
{
    public class ToggleStreamCommand : ICommand
    {
        private readonly Ffmpeg ffmpeg;

        public uint StreamIndex { get; }

        public event EventHandler CanExecuteChanged;

        public event Action Toggled;

        public ToggleStreamCommand(uint streamIndex, Ffmpeg ffmpeg)
        {
            this.StreamIndex = streamIndex;
            this.ffmpeg = ffmpeg;
        }

        public bool CanExecute(object parameter) => true;

        public void Execute(object parameter)
        {
            this.ffmpeg.SelectStreams(map =>
                map.Toggle(new FfmpegMapSelection { IsPositive = true, StreamIndex = this.StreamIndex })
            );

            this.Toggled?.Invoke();
        }
    }
}
