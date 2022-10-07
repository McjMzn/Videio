using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia.Threading;
using Vidio.FFmpeg;

namespace Vidio.App.Command
{
    class ConvertCommand : ICommand
    {
        private readonly Ffmpeg ffmpeg;
        private readonly Action<Process> processAction;
        private Task conversionTask;

        public event EventHandler CanExecuteChanged;

        public ConvertCommand(Ffmpeg ffmpeg, Action<Process> processAction)
        {
            this.ffmpeg = ffmpeg;
            this.processAction = processAction;
        }

        public bool CanExecute(object parameter) => conversionTask is null || conversionTask.IsCompleted;

        public void Execute(object parameter)
        {
            this.conversionTask = Task.Run(() =>
            {
                Dispatcher.UIThread.Post(() => this.CanExecuteChanged?.Invoke(this, EventArgs.Empty));
                                
                this.ffmpeg.Run(this.processAction);

                Dispatcher.UIThread.Post(() => this.CanExecuteChanged?.Invoke(this, EventArgs.Empty));
            });
        }
    }
}
