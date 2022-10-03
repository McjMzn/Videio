using Avalonia.Controls;
using Avalonia.Input;
using VideIO.App.ViewModels;

namespace VideIO.App.Views
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
