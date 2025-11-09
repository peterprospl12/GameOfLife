using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GameOfLife.Core.Enums;
using GameOfLife.Core.Models;
using GameOfLife.Core.Services;
using Microsoft.Win32;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace GameOfLife.WPF.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        [ObservableProperty]
        private Rules _rules;

        [ObservableProperty]
        private Statistics _statistics;

        [ObservableProperty]
        private bool _isAnimating;

        [ObservableProperty]
        private bool _isEditing;

        [ObservableProperty]
        private double _speed = 1.0;

        [ObservableProperty]
        private BoardViewModel _boardViewModel;

        private DispatcherTimer _animationTimer;

        private readonly SimulationService _simulationService;
        private readonly FileService _fileService;

        public MainViewModel(SimulationService simulationService, FileService fileService)
        {
            _simulationService = simulationService;
            _fileService = fileService;
        }

        public void Initialize(int width, int height, string ruleString, Color aliveColor, Color deadColor, CellShape shape)
        {
            Rules = new Rules(ruleString);

            Statistics = new Statistics();
            BoardViewModel = new BoardViewModel(width, height)
            {
                AliveColor = aliveColor,
                DeadColor = deadColor,
                CellShape = shape,
                IsEditing = true
            };
            IsEditing = true;
            IsAnimating = false;

            _animationTimer = new DispatcherTimer();
            _animationTimer.Tick += OnAnimationTick;
            UpdateTimerInterval();
        }

        [RelayCommand]
        private void Step()
        {
            _simulationService.NextGeneration(BoardViewModel.GetBoard, Rules, Statistics);
        }

        [RelayCommand(CanExecute = nameof(CanStep))]
        private void StepManual()
        {
            Step();
        }

        private bool CanStep()
        {
            return !IsAnimating;
        }

        [RelayCommand(CanExecute = nameof(CanStep))]
        private async Task SaveAsync()
        {
            var saveFileDialog = new SaveFileDialog
            {
                Filter = "JSON Files (*.json)|*.json|All files (*.*)|*.*",
                DefaultExt = ".json"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    await _fileService.SaveStateAsync(saveFileDialog.FileName, BoardViewModel.GetBoard, Rules, Statistics);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error saving file: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        [RelayCommand(CanExecute = nameof(CanStep))]
        private async Task LoadAsync()
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "JSON Files (*.json)|*.json|All files (*.*)|*.*",
                DefaultExt = ".json"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    var (board, rules, statistics) = await _fileService.LoadStateAsync(openFileDialog.FileName);
                    Rules = rules;
                    Statistics = statistics;
                    BoardViewModel.Board = board;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading file: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        [RelayCommand]
        private void StartAnimation()
        {
            if (!IsAnimating)
            {
                IsAnimating = true;
                IsEditing = false;
                _animationTimer.Start();
            }
        }

        [RelayCommand]
        private void StopAnimation()
        {
            if (IsAnimating)
            {
                _animationTimer.Stop();
                IsAnimating = false;
                IsEditing = true;
            }
        }

        [RelayCommand]
        private void Randomize()
        {
            if (IsEditing)
            {
                BoardViewModel.Randomize(0.5);
                Statistics.Reset();
            }
        }

        [RelayCommand]
        private void Clear()
        {
            if (IsEditing)
            {
                BoardViewModel.Clear();
                Statistics.Reset();
            }
        }

        [RelayCommand]
        private void SaveImage()
        {
            var saveFileDialog = new SaveFileDialog
            {
                Filter = "PNG Image|*.png",
                Title = "Save an Image File",
                FileName = "GameOfLife_Board.png"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                if (!string.IsNullOrEmpty(saveFileDialog.FileName))
                {
                    // Używamy stałego rozmiaru komórki dla obrazu, np. 10x10 pikseli
                    var bitmap = BoardViewModel.CreateBitmap(10);

                    var encoder = new PngBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(bitmap));

                    using (var stream = new FileStream(saveFileDialog.FileName, FileMode.Create))
                    {
                        encoder.Save(stream);
                    }
                }
            }
        }

        partial void OnSpeedChanged(double value)
        {
            UpdateTimerInterval();
        }

        partial void OnIsEditingChanged(bool value)
        {
            BoardViewModel.IsEditing = value;
            StepManualCommand.NotifyCanExecuteChanged();
            SaveCommand.NotifyCanExecuteChanged();
            LoadCommand.NotifyCanExecuteChanged();
        }

        partial void OnIsAnimatingChanged(bool value)
        {
            StepManualCommand.NotifyCanExecuteChanged();
            SaveCommand.NotifyCanExecuteChanged();
            LoadCommand.NotifyCanExecuteChanged();
        }

        private void UpdateTimerInterval()
        {
            if (Speed > 0)
            {
                _animationTimer.Interval = TimeSpan.FromMilliseconds(1000 / Speed);
            }
        }

        private void OnAnimationTick(object? sender, EventArgs e)
        {
            Step();
        }
    }
}