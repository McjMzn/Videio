using ReactiveUI;
using System;
using System.Windows.Input;

namespace VideIO.App.ViewModels
{
    public class SelectableButtonViewModel<T> : ViewModelBase
                where T : Enum
    {
        private T content;
        private ICommand command;
        private bool isSelected;

        public SelectableButtonViewModel(T content, ICommand command, bool isSelected)
        {
            this.content = content;
            this.command = command;
            this.isSelected = isSelected;
        }

        public bool IsSelected
        {
            get => this.isSelected;
            set => this.RaiseAndSetIfChanged(ref this.isSelected, value);
        }

        public T Content
        {
            get => this.content;
            set => this.RaiseAndSetIfChanged(ref this.content, value);
        }

        public ICommand Command
        {
            get => this.command;
            set => this.RaiseAndSetIfChanged(ref this.command, value);
        }
    }
}
