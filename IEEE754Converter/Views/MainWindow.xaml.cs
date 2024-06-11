using IEEE754Converter.ViewModels;

namespace IEEE754Converter.Views;

public partial class MainWindow 
{
    public MainWindow()
    {
        InitializeComponent();

        DataContext = new MainViewModel();
    }
}