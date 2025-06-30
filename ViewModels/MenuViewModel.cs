using Fruit_ninja.Commands;
using Fruit_ninja.Models; 
using System.Windows;
using System.Windows.Input;

namespace Fruit_ninja.ViewModels
{
    public class MenuViewModel : ViewModelBase
    {
        private readonly MainViewModel _mainViewModel;

       
        public ICommand StartClassicCommand { get; }
        public ICommand StartArcadeCommand { get; }
        public ICommand StartZenCommand { get; }

        public ICommand NavigateToSettingsCommand { get; }
        public ICommand ExitCommand { get; }

        public MenuViewModel(MainViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel;

           
            StartClassicCommand = new RelayCommand(o => _mainViewModel.NavigateToGame(GameMode.Classic));
            StartArcadeCommand = new RelayCommand(o => _mainViewModel.NavigateToGame(GameMode.Arcade));
            StartZenCommand = new RelayCommand(o => _mainViewModel.NavigateToGame(GameMode.Zen));

            NavigateToSettingsCommand = new RelayCommand(o => _mainViewModel.NavigateToSettings());
            ExitCommand = new RelayCommand(o => Application.Current.Shutdown());
        }
    }
}