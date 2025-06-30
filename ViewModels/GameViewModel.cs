using Fruit_ninja.Commands;
using Fruit_ninja.Managers;
using Fruit_ninja.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Fruit_ninja.ViewModels
{
    public class GameViewModel : ViewModelBase
    {
        
        private readonly MainViewModel _mainViewModel;
        private readonly GameMode _gameMode;
        private readonly Random _random = new Random();
        private const double Gravity = 0.15;
        private const int GameAreaWidth = 800;
        private const int GameAreaHeight = 600;

        public ObservableCollection<GameItem> GameItems { get; set; }
        public ObservableCollection<Splat> Splats { get; set; }

        private DispatcherTimer _gameTimer;
        private DispatcherTimer _arcadeSecondTimer;

        private Dictionary<string, BitmapImage> _fruitImages = new Dictionary<string, BitmapImage>();
        private Dictionary<string, (BitmapImage, BitmapImage)> _fruitHalfImages = new Dictionary<string, (BitmapImage, BitmapImage)>();
        private BitmapImage _bombImage;
        private List<BitmapImage> _splatImages = new List<BitmapImage>();
        private List<string> _fruitImagePaths = new List<string>();

       
        private int _score;
        public int Score { get => _score; set { _score = value; OnPropertyChanged(); } }

        private int _lives;
        public int Lives { get => _lives; set { _lives = value; OnPropertyChanged(); } }

        private int _timeLeft;
        public int TimeLeft { get => _timeLeft; set { _timeLeft = value; OnPropertyChanged(); } }

        public bool IsArcadeMode => _gameMode == GameMode.Arcade;
        public bool ShowLives => _gameMode == GameMode.Classic;
        public bool IsZenMode => _gameMode == GameMode.Zen; 

        private bool _isGameOver;
        public bool IsGameOver { get => _isGameOver; set { _isGameOver = value; OnPropertyChanged(); } }

        private Brush _sliceTrailBrush;
        public Brush SliceTrailBrush
        {
            get => _sliceTrailBrush;
            private set { _sliceTrailBrush = value; OnPropertyChanged(); }
        }

        private BitmapImage _backgroundSkin;
        public BitmapImage BackgroundSkin { get => _backgroundSkin; set { _backgroundSkin = value; OnPropertyChanged(); } }

        private bool _isLoading = true;
        public bool IsLoading { get => _isLoading; set { _isLoading = value; OnPropertyChanged(); } }

        public ICommand ReturnToMenuCommand { get; }
        public ICommand PlayAgainCommand { get; }

        private int _spawnCooldown = 0;

        public GameViewModel(MainViewModel mainViewModel, GameMode mode)
        {
            _mainViewModel = mainViewModel;
            _gameMode = mode;

            GameItems = new ObservableCollection<GameItem>();
            Splats = new ObservableCollection<Splat>();

            ReturnToMenuCommand = new RelayCommand(o => StopGameAndReturnToMenu());
            PlayAgainCommand = new RelayCommand(o => ResetGame());

            _gameTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(16) };
            _gameTimer.Tick += GameLoop;

            InitializeGameAsync();
        }

        private async void InitializeGameAsync()
        {
            IsLoading = true;
            await LoadAllAssetsAsync();
            LoadBlade();
            ResetGame();
            IsLoading = false;
        }

        private Task LoadAllAssetsAsync()
        {
            return Task.Run(() =>
            {
                var backgroundPath = GetBackgroundPath();
                var backgroundBitmap = LoadBitmap(backgroundPath);

                var fruitPaths = new Dictionary<string, (string, string)>
                {
                    ["pack://application:,,,/Resources/Images/fruit1.png"] = ("pack://application:,,,/Resources/Images/fruit1_half1.png", "pack://application:,,,/Resources/Images/fruit1_half2.png"),
                    ["pack://application:,,,/Resources/Images/fruit2.png"] = ("pack://application:,,,/Resources/Images/fruit2_half1.png", "pack://application:,,,/Resources/Images/fruit2_half2.png"),
                    ["pack://application:,,,/Resources/Images/fruit3.png"] = ("pack://application:,,,/Resources/Images/fruit3_half1.png", "pack://application:,,,/Resources/Images/fruit3_half2.png"),
                    ["pack://application:,,,/Resources/Images/fruit4.png"] = ("pack://application:,,,/Resources/Images/fruit4_half1.png", "pack://application:,,,/Resources/Images/fruit4_half2.png"),
                    ["pack://application:,,,/Resources/Images/fruit5.png"] = ("pack://application:,,,/Resources/Images/fruit5_half1.png", "pack://application:,,,/Resources/Images/fruit5_half2.png"),
                    ["pack://application:,,,/Resources/Images/fruit6.png"] = ("pack://application:,,,/Resources/Images/fruit6_half1.png", "pack://application:,,,/Resources/Images/fruit6_half2.png"),
                    ["pack://application:,,,/Resources/Images/fruit7.png"] = ("pack://application:,,,/Resources/Images/fruit7_half1.png", "pack://application:,,,/Resources/Images/fruit7_half2.png"),
                    ["pack://application:,,,/Resources/Images/fruit8.png"] = ("pack://application:,,,/Resources/Images/fruit8_half1.png", "pack://application:,,,/Resources/Images/fruit8_half2.png"),
                    ["pack://application:,,,/Resources/Images/fruit9.png"] = ("pack://application:,,,/Resources/Images/fruit9_half1.png", "pack://application:,,,/Resources/Images/fruit9_half2.png"),
                    ["pack://application:,,,/Resources/Images/fruit10.png"] = ("pack://application:,,,/Resources/Images/fruit10_half1.png", "pack://application:,,,/Resources/Images/fruit10_half2.png"),
                    ["pack://application:,,,/Resources/Images/fruit11.png"] = ("pack://application:,,,/Resources/Images/fruit11_half1.png", "pack://application:,,,/Resources/Images/fruit11_half2.png")
                };

                foreach (var path in fruitPaths)
                {
                    _fruitImages[path.Key] = LoadBitmap(path.Key);
                    _fruitHalfImages[path.Key] = (LoadBitmap(path.Value.Item1), LoadBitmap(path.Value.Item2));
                }
                _fruitImagePaths = fruitPaths.Keys.ToList();

                _bombImage = LoadBitmap("pack://application:,,,/Resources/Images/bomb.png");
                _splatImages = new List<BitmapImage>
                {
                    LoadBitmap("pack://application:,,,/Resources/Images/splat_red.png"),
                    LoadBitmap("pack://application:,,,/Resources/Images/splat_green.png"),
                    LoadBitmap("pack://application:,,,/Resources/Images/splat_yellow.png")
                };

                Application.Current.Dispatcher.Invoke(() => { BackgroundSkin = backgroundBitmap; });
            });
        }

        private string GetBackgroundPath()
        {
            string selectedSkinName = Properties.Settings.Default.SelectedSkin;
            switch (selectedSkinName)
            {
                case "Bamboo": return "pack://application:,,,/Resources/Images/background_bamboo.jpg";
                case "Hello Kitty": return "pack://application:,,,/Resources/Images/background_kitty.jpg";
                case "Матриця": return "pack://application:,,,/Resources/Images/background_matrix.jpg";
                case "Космос": return "pack://application:,,,/Resources/Images/background_space.jpg";
                case "Synthwave": return "pack://application:,,,/Resources/Images/background_synthwave.jpg";
                case "Підводний світ": return "pack://application:,,,/Resources/Images/background_underwater.jpg";
                default: return "pack://application:,,,/Resources/Images/background_classic.jpg";
            }
        }

        private BitmapImage LoadBitmap(string path)
        {
            try
            {
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(path, UriKind.Absolute);
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();
                bitmap.Freeze();
                return bitmap;
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    MessageBox.Show($"Не вдалося завантажити зображення: {path}\n{ex.Message}");
                });
                return null;
            }
        }

        private void LoadBlade()
        {
            string selectedBladeName = Properties.Settings.Default.SelectedBlade;
            switch (selectedBladeName)
            {
                case "Вогняний": SliceTrailBrush = new LinearGradientBrush(Colors.OrangeRed, Colors.Yellow, 90); break;
                case "Неоновий": SliceTrailBrush = new LinearGradientBrush(Colors.Aqua, Colors.Fuchsia, 90); break;
                case "Крижаний": SliceTrailBrush = new SolidColorBrush(Colors.LightCyan); break;
                case "Кристальний": SliceTrailBrush = new LinearGradientBrush(Colors.LightGreen, Colors.SpringGreen, 90); break;
                case "Тіньовий": SliceTrailBrush = new SolidColorBrush(Color.FromRgb(40, 40, 40)); break;
                default: SliceTrailBrush = new SolidColorBrush(Colors.White); break;
            }
        }

        private void ResetGame()
        {
            Splats.Clear();
            GameItems.Clear();
            Score = 0;
            IsGameOver = false;
            switch (_gameMode)
            {
                case GameMode.Classic: Lives = 3; _arcadeSecondTimer?.Stop(); break;
                case GameMode.Arcade: Lives = int.MaxValue; TimeLeft = 60; _arcadeSecondTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) }; _arcadeSecondTimer.Tick += ArcadeTimer_Tick; _arcadeSecondTimer.Start(); break;
                case GameMode.Zen: Lives = int.MaxValue; _arcadeSecondTimer?.Stop(); break;
            }
            OnPropertyChanged(nameof(Lives));
            OnPropertyChanged(nameof(TimeLeft));
            OnPropertyChanged(nameof(IsArcadeMode));
            OnPropertyChanged(nameof(ShowLives));
            OnPropertyChanged(nameof(IsZenMode)); 
            _gameTimer.Start();
        }

        private void GameLoop(object sender, EventArgs e)
        {
            if (IsGameOver || IsLoading) return;

            if (--_spawnCooldown <= 0)
            {
                _spawnCooldown = _random.Next(25, 40);
                if (_gameMode != GameMode.Zen && _random.Next(0, 100) < 20)
                {
                    var newBomb = new GameItem { Type = ItemType.Bomb, Image = _bombImage, Position = new Point(_random.Next(100, GameAreaWidth - 100), GameAreaHeight), Velocity = new Vector(_random.Next(-2, 3), -_random.Next(16, 20)) };
                    GameItems.Add(newBomb);
                }
                else
                {
                    string fruitPath = _fruitImagePaths[_random.Next(0, _fruitImagePaths.Count)];
                    var newFruit = new GameItem { Type = ItemType.Fruit, ImagePath = fruitPath, Image = _fruitImages[fruitPath], Position = new Point(_random.Next(100, GameAreaWidth - 100), GameAreaHeight), Velocity = new Vector(_random.Next(-2, 3), -_random.Next(15, 20)) };
                    GameItems.Add(newFruit);
                }
            }

            for (int i = GameItems.Count - 1; i >= 0; i--)
            {
                var item = GameItems[i];
                item.Velocity = new Vector(item.Velocity.X, item.Velocity.Y + Gravity);
                item.Position = new Point(item.Position.X + item.Velocity.X, item.Position.Y + item.Velocity.Y);
                if (item.Type == ItemType.FruitHalf)
                {
                    item.RotationAngle += item.Velocity.X * 1.5;
                }
                if (item.Position.Y > GameAreaHeight + 100)
                {
                    if (_gameMode == GameMode.Classic && item.Type == ItemType.Fruit)
                    {
                        if (--Lives <= 0) TriggerGameOver("Ви пропустили 3 фрукти!");
                    }
                    GameItems.RemoveAt(i);
                }
            }

            for (int i = Splats.Count - 1; i >= 0; i--)
            {
                if (--Splats[i].Lifetime <= 0)
                {
                    Splats.RemoveAt(i);
                }
            }
        }

        public void CheckForSlice(Point mousePosition)
        {
            if (IsGameOver) return;
            var slicedItems = new List<GameItem>();
            foreach (var item in GameItems)
            {
                if ((item.Type == ItemType.Fruit || item.Type == ItemType.Bomb) &&
                    new Rect(item.Position.X - 50, item.Position.Y - 50, 100, 100).Contains(mousePosition))
                {
                    slicedItems.Add(item);
                }
            }

            if (slicedItems.Any(item => item.Type == ItemType.Bomb) && _gameMode != GameMode.Zen)
            {
                GameItems.Remove(slicedItems.First(item => item.Type == ItemType.Bomb));
                SoundManager.PlayBombSound();
                TriggerGameOver("Ви розрізали бомбу!");
                return;
            }

            int slicedFruitsCount = slicedItems.Count(item => item.Type == ItemType.Fruit);
            if (slicedFruitsCount > 0)
            {
                SoundManager.PlaySliceSound();
                Score += (slicedFruitsCount == 1) ? 1 : (slicedFruitsCount == 2) ? 10 : slicedFruitsCount * 10;
                foreach (var item in slicedItems.Where(i => i.Type == ItemType.Fruit).ToList())
                {
                    GameItems.Remove(item);
                    SplitFruit(item);
                }
            }
        }

        private void SplitFruit(GameItem originalFruit)
        {
            if (!_fruitHalfImages.TryGetValue(originalFruit.ImagePath, out var halves)) return;

            var half1 = new GameItem { Type = ItemType.FruitHalf, Image = halves.Item1, Position = originalFruit.Position, Velocity = new Vector(originalFruit.Velocity.X - 2, originalFruit.Velocity.Y - 2) };
            var half2 = new GameItem { Type = ItemType.FruitHalf, Image = halves.Item2, Position = originalFruit.Position, Velocity = new Vector(originalFruit.Velocity.X + 2, originalFruit.Velocity.Y - 2) };
            GameItems.Add(half1);
            GameItems.Add(half2);

            var splat = new Splat { Position = new Point(originalFruit.Position.X, originalFruit.Position.Y), Image = _splatImages[_random.Next(0, _splatImages.Count)] };
            Splats.Add(splat);
        }

        private void TriggerGameOver(string message)
        {
            _gameTimer.Stop();
            _arcadeSecondTimer?.Stop();
            SaveTotalScore();
            MessageBox.Show($"{message}\nВаш рахунок: {Score}", "Гру завершено");
            IsGameOver = true;
        }

        private void StopGameAndReturnToMenu()
        {
            _gameTimer.Stop();
            _arcadeSecondTimer?.Stop();
            SaveTotalScore();
            SoundManager.PlayMenuMusic();
            _mainViewModel.CurrentViewModel = new MenuViewModel(_mainViewModel);
        }

        public void SaveTotalScore()
        {
            Properties.Settings.Default.PlayerScore += Score;
            Properties.Settings.Default.Save();
        }

        private void ArcadeTimer_Tick(object sender, EventArgs e)
        {
            if (--TimeLeft <= 0) TriggerGameOver("Час вийшов!");
        }
    }
}