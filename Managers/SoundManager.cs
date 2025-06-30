using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Media;

namespace Fruit_ninja.Managers
{
    public static class SoundManager
    {
        private static readonly MediaPlayer MusicPlayer = new MediaPlayer();
        private static readonly Dictionary<string, string> TempMusicFiles = new Dictionary<string, string>();

       
        private static readonly SoundPlayer SlicePlayer = new SoundPlayer();
        private static readonly SoundPlayer ExplosionPlayer = new SoundPlayer();

        public static void LoadSounds()
        {
            
            MusicPlayer.MediaEnded += (sender, e) =>
            {
                MusicPlayer.Position = TimeSpan.Zero;
                MusicPlayer.Play();
            };
            MusicPlayer.MediaFailed += (sender, e) =>
            {
                Debug.WriteLine($"ПОМИЛКА MusicPlayer: {e.ErrorException.Message}");
            };
            Application.Current.Exit += (s, e) => CleanupTempFiles();

          
            LoadWavResource(SlicePlayer, "pack://application:,,,/Resources/Sounds/slice.wav");
            LoadWavResource(ExplosionPlayer, "pack://application:,,,/Resources/Sounds/explosion.wav");
        }

        
        private static void LoadWavResource(SoundPlayer player, string resourcePath)
        {
            try
            {
                var resourceUri = new Uri(resourcePath, UriKind.Absolute);
                
                var streamInfo = Application.GetResourceStream(resourceUri);

                if (streamInfo != null)
                {
                    player.Stream = streamInfo.Stream;
                    
                    player.LoadAsync();
                }
                else
                {
                    Debug.WriteLine($"КРИТИЧНА ПОМИЛКА: Вбудований WAV ресурс НЕ ЗНАЙДЕНО: {resourcePath}");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Помилка при завантаженні WAV з ресурсу {resourcePath}: {ex.Message}");
            }
        }

        
        private static async void PlayMusic(string resourcePath)
        {
            try
            {
                string tempFilePath;
                if (!TempMusicFiles.TryGetValue(resourcePath, out tempFilePath))
                {
                    var resourceUri = new Uri(resourcePath, UriKind.Absolute);
                    var streamInfo = Application.GetResourceStream(resourceUri);
                    if (streamInfo == null)
                    {
                        Debug.WriteLine($"КРИТИЧНА ПОМИЛКА: Вбудований ресурс не знайдено: {resourcePath}");
                        return;
                    }
                    tempFilePath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".mp3");
                    using (var resourceStream = streamInfo.Stream)
                    using (var fileStream = new FileStream(tempFilePath, FileMode.Create, FileAccess.Write))
                    {
                        await resourceStream.CopyToAsync(fileStream);
                    }
                    TempMusicFiles[resourcePath] = tempFilePath;
                }
                var fileUri = new Uri(tempFilePath, UriKind.Absolute);
                Application.Current.Dispatcher.Invoke(() =>
                {
                    if (MusicPlayer.Source != fileUri)
                    {
                        MusicPlayer.Open(fileUri);
                        MusicPlayer.Play();
                    }
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Помилка при відтворенні музики з ресурсу {resourcePath}: {ex.Message}");
            }
        }

       
        public static void PlaySliceSound()
        {
           
           SlicePlayer?.Play();
        }

        public static void PlayBombSound()
        {
            ExplosionPlayer?.Play();
        }

        public static void PlayMenuMusic()
        {
            PlayMusic("pack://application:,,,/Resources/Sounds/menu_music.mp3");
        }

        public static void PlayGameMusic()
        {
            PlayMusic("pack://application:,,,/Resources/Sounds/game_music.mp3");
        }

        private static void CleanupTempFiles()
        {
            foreach (var file in TempMusicFiles.Values)
            {
                try
                {
                    if (File.Exists(file)) File.Delete(file);
                }
                catch { }
            }
        }
    }
}