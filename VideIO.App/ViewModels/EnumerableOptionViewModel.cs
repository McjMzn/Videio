using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using VideIO.App.Command;
using VideIO.FFmpeg;

namespace VideIO.App.ViewModels
{
    public class EnumerableOptionViewModel<T> : ViewModelBase
        where T : Enum 
    {
        public EnumerableOptionViewModel(Ffmpeg ffmpeg)
        {
            this.Buttons = new ObservableCollection<SelectableButtonViewModel<T>>();
            foreach (T option in Enum.GetValues(typeof(T)))
            {
                var command = new SelectOptionCommand<T>(ffmpeg, option, this.OnOptionChanged);
                this.Buttons.Add(new SelectableButtonViewModel<T>(option, command, Enum.Equals(option, this.Selected)));
            }
        }

        public ObservableCollection<SelectableButtonViewModel<T>> Buttons { get; }
        public T Selected { get; private set; }

        private void OnOptionChanged(T option)
        {
            this.Selected = option;
            foreach (var button in this.Buttons)
            {
                button.IsSelected = Enum.Equals(button.Content, this.Selected);
            }

            this.RaisePropertyChanged(nameof(this.Selected));
            this.RaisePropertyChanged(nameof(this.Buttons));
        }
    }
}
