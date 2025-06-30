using System.Windows;
using System.Windows.Media.Imaging;

namespace Fruit_ninja.Models
{
    public class Splat
    {
        public Point Position { get; set; }
        public BitmapImage Image { get; set; }
        public int Lifetime { get; set; } = 100; 
    }
}