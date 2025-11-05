using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GameOfLife.Core.Models;
using GameOfLife.Core.Services;
using System;
using System.Windows.Threading;

namespace GameOfLife.WPF.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        [ObservableProperty]
        private Board board;

        [ObservableProperty]
        private Rules rules;

        [ObservableProperty]
        private Statistics statistics;

        [ObservableProperty]
        private bool isAnimating;

        [ObservableProperty]
        private bool isEditing;

        [ObservableProperty]
        private double speed = 1.0; 

        [ObservableProperty]
        private BoardViewModel boardViewModel;

        private DispatcherTimer animationTimer;

        private readonly SimulationService simulationService;

        public MainViewModel(SimulationService simulationService)
        {
            this.simulationService = simulationService;

            Board = new Board();
            Board.Initialize(100, 100);

            Rules = Rules.DefaultConway();

            Statistics = new Statistics();
            BoardViewModel = new BoardViewModel(Board);

            IsEditing = true;
            IsAnimating = false;

            animationTimer = new DispatcherTimer();
            animationTimer.Tick += OnAnimationTick;
            UpdateTimerInterval();

            Board.Randomize(0.3);
            BoardViewModel.UpdateVisibleCells();
        }

        [RelayCommand]
        private void Step()
        {
            simulationService.NextGeneration(Board,Rules, Statistics);
            BoardViewModel.UpdateVisibleCells();
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
                animationTimer.Start();
            }
        }

        [RelayCommand]
        private void StopAnimation()
        {
            if (IsAnimating)
            {
                animationTimer.Stop();
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
                BoardViewModel.UpdateVisibleCells();
            }
        }

        [RelayCommand]
        private void Clear()
        {
            if (IsEditing)
            {
                Board.Clear();
                Statistics.Reset();
                BoardViewModel.UpdateVisibleCells(); 
            }
        }

        [RelayCommand]
        private void Save(string filePath)
        {
            if (!string.IsNullOrEmpty(filePath))
            {
                Board.SaveToFile(filePath, Rules);
                BoardViewModel.UpdateVisibleCells();
            }
        }

        [RelayCommand]
        private void Load(string filePath)
        {
            if (!string.IsNullOrEmpty(filePath))
            {
                Board.LoadFromFile(filePath, Rules);
                Statistics.Reset();
                BoardViewModel.UpdateVisibleCells(); 
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
                animationTimer.Interval = TimeSpan.FromMilliseconds(1000 / Speed);
            }
        }

        private void OnAnimationTick(object? sender, EventArgs e)
        {
            Step();
        }
    }
}