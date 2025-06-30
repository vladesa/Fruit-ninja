using Fruit_ninja.ViewModels;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation; 

namespace Fruit_ninja.Views
{
    public partial class GameView : UserControl
    {
        private bool _isSlicing = false;
       
        private readonly PointCollection _slicePoints = new PointCollection();

        public GameView()
        {
            InitializeComponent();
            
            SliceTrail.Points = _slicePoints;
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _isSlicing = true;
           
            _slicePoints.Clear();
            SliceTrail.Opacity = 1; 
            _slicePoints.Add(e.GetPosition(this));

            
            Slice(e.GetPosition(this));
        }

        private void Grid_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _isSlicing = false;
           
            AnimateSliceFadeOut();
        }

        private void Grid_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isSlicing)
            {
                
                _slicePoints.Add(e.GetPosition(this));

               
                if (_slicePoints.Count > 15)
                {
                    _slicePoints.RemoveAt(0);
                }

                
                Slice(e.GetPosition(this));
            }
        }

        
        private void AnimateSliceFadeOut()
        {
            var fadeOutAnimation = new DoubleAnimation
            {
                From = 1,
                To = 0,
                Duration = TimeSpan.FromMilliseconds(400) 
            };

           
            fadeOutAnimation.Completed += (s, a) => _slicePoints.Clear();

            SliceTrail.BeginAnimation(OpacityProperty, fadeOutAnimation);
        }

        private void Slice(Point position)
        {
            var viewModel = DataContext as GameViewModel;
            if (viewModel != null)
            {
                viewModel.CheckForSlice(position);
            }
        }
    }
}