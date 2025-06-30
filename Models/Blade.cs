using Fruit_ninja.ViewModels;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Fruit_ninja.Models
{
    public class Blade : ViewModelBase
    {
        public string Name { get; set; }
        public int Cost { get; set; }

        //залишає слід від руху леза
        public Brush TrailBrush { get; set; }

        
        public BitmapImage PreviewImage { get; set; }

        private bool _isUnlocked;
        public bool IsUnlocked
        {
            get => _isUnlocked;
            set { _isUnlocked = value; OnPropertyChanged(); }
        }
    }
}