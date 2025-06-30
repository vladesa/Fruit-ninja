using Fruit_ninja.Commands;
using Fruit_ninja.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Fruit_ninja.ViewModels
{
    public class SettingsViewModel : ViewModelBase
    {
        private readonly MainViewModel _mainViewModel;
        private int _playerScore;

        public int PlayerScore { get => _playerScore; set { _playerScore = value; OnPropertyChanged(); } }
        public ObservableCollection<Skin> AllSkins { get; set; }
        public ObservableCollection<Blade> AllBlades { get; set; }

        public ICommand NavigateBackCommand { get; }
        public ICommand BuyCommand { get; }
        public ICommand SelectCommand { get; }

        public SettingsViewModel(MainViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel;
            AllSkins = new ObservableCollection<Skin>();
            AllBlades = new ObservableCollection<Blade>();

            LoadSkins();
            LoadBlades();
            LoadPlayerData();

            NavigateBackCommand = new RelayCommand(o => _mainViewModel.CurrentViewModel = new MenuViewModel(_mainViewModel));
            BuyCommand = new RelayCommand(BuyItem, CanBuyItem);
            SelectCommand = new RelayCommand(SelectItem);
        }

        private void BuyItem(object parameter)
        {
            dynamic item = parameter;
            if (PlayerScore >= item.Cost)
            {
                PlayerScore -= item.Cost;
                item.IsUnlocked = true;
                SavePlayerData();
            }
            else
            {
                MessageBox.Show("Недостатньо очок!", "Помилка");
            }
        }

        private bool CanBuyItem(object parameter)
        {
            if (parameter is Skin skin) return PlayerScore >= skin.Cost;
            if (parameter is Blade blade) return PlayerScore >= blade.Cost;
            return false;
        }

        private void SelectItem(object parameter)
        {
            if (parameter is Skin skin)
            {
                if (!skin.IsUnlocked) return;
                Properties.Settings.Default.SelectedSkin = skin.Name;
                MessageBox.Show($"Скін '{skin.Name}' обрано!", "Інформація");
            }
            else if (parameter is Blade blade)
            {
                if (!blade.IsUnlocked) return;
                Properties.Settings.Default.SelectedBlade = blade.Name;
                MessageBox.Show($"Клинок '{blade.Name}' обрано!", "Інформація");
            }
            SavePlayerData();
        }

        private void LoadSkins()
        {
            try
            {
                AllSkins.Add(new Skin { Name = "Classic", Cost = 0, PreviewImage = new BitmapImage(new Uri("pack://application:,,,/Resources/Images/preview_classic.jpg")), BackgroundImage = new BitmapImage(new Uri("pack://application:,,,/Resources/Images/background_classic.jpg")) });
                AllSkins.Add(new Skin { Name = "Bamboo", Cost = 100, PreviewImage = new BitmapImage(new Uri("pack://application:,,,/Resources/Images/preview_bamboo.jpg")), BackgroundImage = new BitmapImage(new Uri("pack://application:,,,/Resources/Images/background_bamboo.jpg")) });
                AllSkins.Add(new Skin { Name = "Hello Kitty", Cost = 250, PreviewImage = new BitmapImage(new Uri("pack://application:,,,/Resources/Images/preview_kitty.jpg")), BackgroundImage = new BitmapImage(new Uri("pack://application:,,,/Resources/Images/background_kitty.jpg")) });
                AllSkins.Add(new Skin { Name = "Матриця", Cost = 500, PreviewImage = new BitmapImage(new Uri("pack://application:,,,/Resources/Images/preview_matrix.jpg")), BackgroundImage = new BitmapImage(new Uri("pack://application:,,,/Resources/Images/background_matrix.jpg")) });

                
                AllSkins.Add(new Skin { Name = "Космос", Cost = 500, PreviewImage = new BitmapImage(new Uri("pack://application:,,,/Resources/Images/preview_space.jpg")), BackgroundImage = new BitmapImage(new Uri("pack://application:,,,/Resources/Images/background_space.jpg")) });
                AllSkins.Add(new Skin { Name = "Synthwave", Cost = 500, PreviewImage = new BitmapImage(new Uri("pack://application:,,,/Resources/Images/preview_synthwave.jpg")), BackgroundImage = new BitmapImage(new Uri("pack://application:,,,/Resources/Images/background_synthwave.jpg")) });
                AllSkins.Add(new Skin { Name = "Підводний світ", Cost = 600, PreviewImage = new BitmapImage(new Uri("pack://application:,,,/Resources/Images/preview_underwater.jpg")), BackgroundImage = new BitmapImage(new Uri("pack://application:,,,/Resources/Images/background_underwater.jpg")) });
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Не вдалося завантажити картинки для скінів. Помилка: {ex.Message}");
            }
        }

        private void LoadBlades()
        {
            try
            {
                AllBlades.Add(new Blade { Name = "Класичний", Cost = 0, PreviewImage = new BitmapImage(new Uri("pack://application:,,,/Resources/Images/blade_classic.png")), TrailBrush = new SolidColorBrush(Colors.White) });
                AllBlades.Add(new Blade { Name = "Вогняний", Cost = 200, PreviewImage = new BitmapImage(new Uri("pack://application:,,,/Resources/Images/blade_fire.png")), TrailBrush = new LinearGradientBrush(Colors.OrangeRed, Colors.Yellow, 90) });
                AllBlades.Add(new Blade { Name = "Неоновий", Cost = 350, PreviewImage = new BitmapImage(new Uri("pack://application:,,,/Resources/Images/blade_neon.png")), TrailBrush = new LinearGradientBrush(Colors.Aqua, Colors.Fuchsia, 90) });
                AllBlades.Add(new Blade { Name = "Крижаний", Cost = 500, PreviewImage = new BitmapImage(new Uri("pack://application:,,,/Resources/Images/blade_ice.png")), TrailBrush = new SolidColorBrush(Colors.LightCyan) });
                AllBlades.Add(new Blade { Name = "Кристальний", Cost = 750, PreviewImage = new BitmapImage(new Uri("pack://application:,,,/Resources/Images/blade_crystal.png")), TrailBrush = new LinearGradientBrush(Colors.LightGreen, Colors.SpringGreen, 90) });
                AllBlades.Add(new Blade { Name = "Тіньовий", Cost = 1000, PreviewImage = new BitmapImage(new Uri("pack://application:,,,/Resources/Images/blade_shadow.png")), TrailBrush = new SolidColorBrush(Color.FromRgb(40, 40, 40)) });
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Не вдалося завантажити картинки для клинків. Помилка: {ex.Message}");
            }
        }

        private void LoadPlayerData()
        {
            PlayerScore = Properties.Settings.Default.PlayerScore;
            var unlockedSkins = Properties.Settings.Default.UnlockedSkins?.Split(',') ?? Array.Empty<string>();
            foreach (var skin in AllSkins) { if (skin.Cost == 0 || unlockedSkins.Contains(skin.Name)) skin.IsUnlocked = true; }
            var unlockedBlades = Properties.Settings.Default.UnlockedBlades?.Split(',') ?? Array.Empty<string>();
            foreach (var blade in AllBlades) { if (blade.Cost == 0 || unlockedBlades.Contains(blade.Name)) blade.IsUnlocked = true; }
        }

        private void SavePlayerData()
        {
            Properties.Settings.Default.PlayerScore = this.PlayerScore;
            var unlockedSkins = AllSkins.Where(s => s.IsUnlocked).Select(s => s.Name);
            Properties.Settings.Default.UnlockedSkins = string.Join(",", unlockedSkins);
            var unlockedBlades = AllBlades.Where(b => b.IsUnlocked).Select(b => b.Name);
            Properties.Settings.Default.UnlockedBlades = string.Join(",", unlockedBlades);
            Properties.Settings.Default.Save();
        }
    }
}