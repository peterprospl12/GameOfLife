This project is a desktop implementation of Conway's Game of Life, built using modern .NET and WPF. It follows the MVVM (Model-View-ViewModel) architecture to ensure a clean separation between the simulation logic and the user interface.

## Features

- **Dynamic Simulation**: Start, stop, and control the speed of the simulation.
- **Step-by-Step Execution**: Advance the simulation one generation at a time to observe changes.
- **Interactive Board**: Manually toggle the state of cells by clicking on them when the simulation is paused.
- **Customizable Setup**:
    - Define the board's width and height.
    - Specify custom rules for cell survival and birth (e.g., "B3/S23").
    - Choose custom colors for alive and dead cells.
    - Initialize the board with predefined patterns.
- **State Persistence**: Save the current game state (board and rules) to a file and load it later.
- **Real-time Statistics**: View statistics such as the current generation number.

## Architecture

The solution is structured into three distinct projects, adhering to the principles of clean architecture and the MVVM design pattern.

- **`GameOfLife.Core`**: A .NET class library that contains all the core business logic.
  - **Models**: `Board`, `Cell`, `Rules`, and `Statistics`.
  - **Services**: `SimulationService` for running the simulation logic and `FileService` for saving/loading the game state.
  - This project is completely independent of the UI, making the core logic portable and easy to test.

- **`GameOfLife.WPF`**: The main WPF application that provides the user interface.
  - **Views**: XAML files that define the structure and appearance of the UI (`MainWindow`, `InitializationWindow`, `BoardControl`).
  - **ViewModels**: Classes that manage the application's state and logic, acting as a bridge between the Views and the Models (`MainViewModel`, `InitializationViewModel`).
  - **MVVM Pattern**: Utilizes `CommunityToolkit.Mvvm` for implementing `ObservableObject` and `RelayCommand`.
  - **Dependency Injection**: Services from the `Core` project are injected into the ViewModels.

- **`GameOfLife.Tests`**: A test project for unit testing the logic within `GameOfLife.Core` and the ViewModels in `GameOfLife.WPF`.

## Technologies Used

- .NET
- C#
- WPF (Windows Presentation Foundation)
- MVVM Design Pattern
- CommunityToolkit.Mvvm
- Microsoft Extensions for Dependency Injection

<img width="286" height="363" alt="image" src="https://github.com/user-attachments/assets/27b4830f-ca22-4596-a58f-143fe52f4bd5" />
<img width="1917" height="1024" alt="image" src="https://github.com/user-attachments/assets/a007fc9d-8019-4e3b-8704-48b8c37b45d1" />
