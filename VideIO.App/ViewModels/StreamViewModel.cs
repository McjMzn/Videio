using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Videio.App.Command;
using Videio.FFmpeg;
using Videio.FFmpeg.Enums;

namespace Videio.App.ViewModels
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
        public string Details => this.StreamInformation.StreamType == StreamType.Video ? this.StreamInformation.Duration.ToString().TrimEnd('0') + '0' : this.StreamInformation.LanguageCode;

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
