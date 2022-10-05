using Avalonia.Input;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace Videio.App.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private string ffmpegPath = @"ffmpeg.exe";
        public MainWindowViewModel()
        {
            this.VideoFiles = new ObservableCollection<VideoFileViewModel>();
            this.FfmpegFound = this.CheckFfmpeg();
        }

        public ObservableCollection<VideoFileViewModel> VideoFiles { get; set; }

        public bool FfmpegFound { get; }
        
        // Driectly handled in MainWindow's code behind.
        public void OnDrop(object sender, DragEventArgs args)
        {
            foreach (var file in args.Data.GetFileNames())
            {
                this.VideoFiles.Add(new VideoFileViewModel(file, this.ffmpegPath));
            }
        }

        public bool CheckFfmpeg()
        {
            try
            {
                Process.Start(this.ffmpegPath);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
