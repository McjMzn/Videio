using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using VideIO.App.Command;
using VideIO.FFmpeg;
using VideIO.FFmpeg.Enums;

namespace VideIO.App.ViewModels
{
    public class StreamViewModel : ViewModelBase
    {
        public StreamViewModel() { }

        public StreamViewModel(StreamInformation streamInformation, Ffmpeg ffmpeg)
        {
            this.StreamInformation = streamInformation;
            this.Select = new ToggleStreamCommand((uint)this.StreamInformation.StreamIndex, ffmpeg);
            this.Select.Toggled += () =>
            {
                this.IsSelected = !this.IsSelected;
                this.RaisePropertyChanged(nameof(IsSelected));
            };
        }

        public StreamInformation StreamInformation { get; init; }
        public ToggleStreamCommand Select { get; init; }
        public bool IsSelected { get; set; }

        public string Icon => this.StreamInformation.StreamType switch
        {
            StreamType.Video => "mdi-filmstrip",
            StreamType.Audio => "mdi-volume-high",
            StreamType.Subtitle => "mdi-message-text",
            StreamType.Attachements => "mdi-paperclip",
            StreamType.Data => "mdi-database",
            StreamType.Any => "mdi-infinity"
        };
    }
}
