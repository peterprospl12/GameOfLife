using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GameOfLife.Core.Models;
using GameOfLife.Core.Services;
using System.Windows.Threading;

namespace GameOfLife.WPF.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        [ObservableProperty]
        private Board _board;

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

        public MainViewModel(SimulationService simulationService)
        {
            this._simulationService = simulationService;

            Board = new Board();
            Board.Initialize(1000, 1000);

            Rules = Rules.DefaultConway();

            Statistics = new Statistics();
            BoardViewModel = new BoardViewModel(Board);

            IsEditing = true;
            IsAnimating = false;

            _animationTimer = new DispatcherTimer();
            _animationTimer.Tick += OnAnimationTick;
            UpdateTimerInterval();

            Board.Randomize(0.3);
        }

        [RelayCommand]
        private void Step()
        {
            _simulationService.NextGeneration(Board, Rules, Statistics);
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
                Board.Randomize(0.5);
                Statistics.Reset();
            }
        }

        [RelayCommand]
        private void Clear()
        {
            if (IsEditing)
            {
                Board.Clear();
                Statistics.Reset();
            }
        }

        [RelayCommand]
        private void Save(string filePath)
        {
            if (!string.IsNullOrEmpty(filePath))
            {
                Board.SaveToFile(filePath, Rules);
            }
        }

        [RelayCommand]
        private void Load(string filePath)
        {
            if (!string.IsNullOrEmpty(filePath))
            {
                Board.LoadFromFile(filePath, Rules);
                Statistics.Reset();
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
        }

        partial void OnIsAnimatingChanged(bool value)
        {
            StepManualCommand.NotifyCanExecuteChanged();
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