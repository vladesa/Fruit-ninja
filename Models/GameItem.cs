using Fruit_ninja.ViewModels;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Fruit_ninja.Models
{
    public enum ItemType
    {
        Fruit,
        FruitHalf,
        Bomb
    }

    public class GameItem : ViewModelBase
    {
        private Point _position;
        public Point Position { get => _position; set { _position = value; OnPropertyChanged(); } }

        private double _rotationAngle;
        public double RotationAngle { get => _rotationAngle; set { _rotationAngle = value; OnPropertyChanged(); } }

        public Vector Velocity { get; set; }
        public ItemType Type { get; set; }
        public BitmapImage Image { get; set; }
        public string ImagePath { get; set; }
    }
}