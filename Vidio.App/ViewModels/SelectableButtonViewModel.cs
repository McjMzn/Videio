using ReactiveUI;
using System;
using System.Windows.Input;

namespace Videio.App.ViewModels
{
    public class SelectableButtonViewModel<T> : ViewModelBase
    {
        private T content;
        private ICommand command;
        private bool isSelected;
        private readonly string label;

        public SelectableButtonViewModel(T content, ICommand command, bool isSelected, string label = null)
        {
            this.content = content;
            this.command = command;
            this.isSelected = isSelected;
            this.label = label;
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

        public string Label => this.label ?? this.content.ToString();

        public ICommand Command
        {
            get => this.command;
        }
    }
}
