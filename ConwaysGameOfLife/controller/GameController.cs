using System;
using System.IO;
using System.Collections.Generic;
using ConwaysGameOfLife.model;
using System.Text;
using System.Text.Json;

namespace ConwaysGameOfLife.controller
{
    public class GameController
    {
        private Game _game;
        private readonly System.Windows.Forms.Timer _timer;

        public event Action OnBoardUpdate;

        public GameController()
        {
            _timer = new System.Windows.Forms.Timer() { Interval = 100 };
            _timer.Tick += (s, e) =>
            {
                _game.AdvanceGeneration();
                OnBoardUpdate?.Invoke();
            };
        }

        public void InitializeGame(int width, int height, ITopology topology, int birth, int survivalMin, int survivalMax)
        {
            _game = new Game(width, height, topology, birth, survivalMin, survivalMax);
            OnBoardUpdate?.Invoke();
        }

        public void ToggleSimulation(bool run)
        {
            if (run) _timer.Start();
            else _timer.Stop();
        }
        public void Reset()
        {
            _timer.Stop();
            _game.Clear();
            OnBoardUpdate?.Invoke();
        }

        public void Dispose()
        {
            _timer.Stop();
            _timer.Dispose();
        }
        public void SaveGame(string filePath)
        {
            bool[,] grid = _game.ExportGrid();
            int width = grid.GetLength(0);
            int height = grid.GetLength(1);

            bool[][] jaggedGrid = new bool[width][];
            for (int i = 0; i < width; i++)
            {
                jaggedGrid[i] = new bool[height];
                for (int j = 0; j < height; j++)
                {
                    jaggedGrid[i][j] = grid[i, j];
                }
            }

            string json = JsonSerializer.Serialize(jaggedGrid);
            File.WriteAllText(filePath, json);
        }

        public void LoadGame(string filePath, ITopology topology, int birth, int survivalMin, int survivalMax)
        {
            if (!File.Exists(filePath)) throw new FileNotFoundException("Save file not found.");

            string json = File.ReadAllText(filePath);
            bool[][] jaggedGrid = JsonSerializer.Deserialize<bool[][]>(json)!;

            int width = jaggedGrid.Length;
            int height = jaggedGrid[0].Length;
            bool[,] grid = new bool[width, height];

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    grid[i, j] = jaggedGrid[i][j];
                }
            }

            _game = new Game(width, height, topology, birth, survivalMin, survivalMax, grid);
            OnBoardUpdate?.Invoke();
        }

        public bool[,] GetCurrentGrid() => _game.ExportGrid();

        public void UpdateRules(int birth, int minSurvival, int maxSurvival)
        {
            if (_game == null) return;
            _game.BirthThreshold = birth;
            _game.MinSurvivalThreshold = minSurvival;
            _game.MaxSurvivalThreshold = maxSurvival;
        }

        public void ToggleCell(int x, int y, bool state)
        {
            if (_game == null) return;
            _game.SetCell(x, y, state);
            OnBoardUpdate?.Invoke();
        }
    }
}
