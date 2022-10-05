using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Videio.App.Command;

namespace Videio.App.ViewModels
{
    public class OptionViewModel<T> : ViewModelBase
    {
        private readonly Action<T> action;

        public OptionViewModel(Action<T> action, IEnumerable<T> options)
            : this(action, options.ToDictionary(x => x, x => (string)null))
        {
        }

        public OptionViewModel(Action<T> action, IDictionary<T, string> options)
        {
            this.Buttons = new();
            foreach (var kvp in options)
            {
                var option = kvp.Key;
                var label = kvp.Value;

                var command = new ActionCommand(() => ChangeOption(option));
                this.Buttons.Add(new SelectableButtonViewModel<T>(option, command, option.Equals(this.Selected), label));
            }

            this.action = action;
        }

        public T Selected { get; private set; }

        public ObservableCollection<SelectableButtonViewModel<T>> Buttons { get; }

        private void ChangeOption(T option)
        {
            this.action(option);

            this.Selected = option;
            foreach (var button in this.Buttons)
            {
                button.IsSelected = button.Content.Equals(this.Selected);
            }

            this.RaisePropertyChanged(nameof(this.Selected));
            this.RaisePropertyChanged(nameof(this.Buttons));
        }
    }
}
