using Fruit_ninja.ViewModels;
using System.Windows;

namespace Fruit_ninja
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            
            this.DataContext = new MainViewModel();
        }
    }
}