using Fruit_ninja.Managers;
using Fruit_ninja.Models;
using Fruit_ninja.ViewModels;

namespace Fruit_ninja.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private ViewModelBase _currentViewModel;
        public ViewModelBase CurrentViewModel { get => _currentViewModel; set { _currentViewModel = value; OnPropertyChanged(); } }

        public MainViewModel()
        {
            CurrentViewModel = new MenuViewModel(this);
            SoundManager.PlayMenuMusic();
        }

        public void NavigateToGame(GameMode mode)
        {
            CurrentViewModel = new GameViewModel(this, mode);
           // SoundManager.PlayGameMusic();
        }

        public void NavigateToSettings()
        {
            CurrentViewModel = new SettingsViewModel(this);
            SoundManager.PlayMenuMusic();
        }
    }
}