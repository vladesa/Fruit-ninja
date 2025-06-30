using Fruit_ninja.ViewModels;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Fruit_ninja.Models
{
    public class Skin : ViewModelBase
    {
        public string Name { get; set; }
        public int Cost { get; set; }

        public BitmapImage BackgroundImage { get; set; }
        public BitmapImage PreviewImage { get; set; }

        private bool _isUnlocked;
        public bool IsUnlocked
        {
            get => _isUnlocked;
            set
            {
                _isUnlocked = value;
                OnPropertyChanged();
            }
        }
    }
}