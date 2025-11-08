using GameOfLife.Core.Models;
using GameOfLife.Core.Models.DTO;
using System.IO;
using System.Text.Json;

namespace GameOfLife.Core.Services
{
    public class FileService
    {
        private static readonly JsonSerializerOptions Options = new() { WriteIndented = true };

        public async Task SaveStateAsync(string filePath, Board board, Rules rules, Statistics statistics)
        {
            var gameState = new GameStateDto
            {
                RuleString = rules.RuleString,
                Generation = statistics.Generation,
                TotalBorn = statistics.TotalBorn,
                TotalDied = statistics.TotalDied,
                Width = board.Width,
                Height = board.Height,
                AliveCells = board.Cells
            };

            await using var stream = File.Create(filePath);
            await JsonSerializer.SerializeAsync(stream, gameState, Options);
        }

        public async Task<(Board board, Rules rules, Statistics statistics)> LoadStateAsync(string filePath)
        {
            await using var stream = File.OpenRead(filePath);
            var gameState = await JsonSerializer.DeserializeAsync<GameStateDto>(stream);

            if (gameState is null)
            {
                throw new InvalidDataException("Failed to deserialize game state from file.");
            }

            var board = new Board();
            board.Initialize(gameState.Width, gameState.Height);
            board.Update(gameState.AliveCells);

            var rules = new Rules(gameState.RuleString);

            var statistics = new Statistics();
            statistics.Reset();

            typeof(Statistics).GetProperty(nameof(Statistics.Generation))?.SetValue(statistics, gameState.Generation);
            typeof(Statistics).GetProperty(nameof(Statistics.TotalBorn))?.SetValue(statistics, gameState.TotalBorn);
            typeof(Statistics).GetProperty(nameof(Statistics.TotalDied))?.SetValue(statistics, gameState.TotalDied);

            return (board, rules, statistics);
        }
    }
}