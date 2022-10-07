using Avalonia.Controls;
using Avalonia.Input;
using Vidio.App.ViewModels;

namespace Vidio.App.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();
            this.AddHandler(DragDrop.DropEvent, this.OnDrop);
        }

        public void OnDrop(object sender, DragEventArgs args)
        {
            if (this.DataContext is not MainWindowViewModel viewModel)
            {
                return;
            }

            viewModel.OnDrop(sender, args);
        }
    }
}
