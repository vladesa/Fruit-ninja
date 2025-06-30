using Fruit_ninja.Managers;
using System.Windows;

namespace Fruit_ninja
{
    public partial class App : Application
    {
        public App()
        {
            SoundManager.LoadSounds();
        }
    }
}