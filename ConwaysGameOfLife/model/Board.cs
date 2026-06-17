using System;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using System.Text;

namespace ConwaysGameOfLife.model
{
    internal class Game
    {
        private bool[,] currentGrid { get; set;  }

        public int Width { get; }
        public int Height { get; }
        public ITopology Topology { get; }
        public int BirthThreshold { get; set; } = 3;
        public int MinSurvivalThreshold { get; set; } = 2;
        public int MaxSurvivalThreshold { get; set; } = 3;
       
        public Game(int width, int height, ITopology topology, int birthThreshold, int minSurvivalThreshold, int maxSurvivalThreshold)
        {
            Width = width;
            Height = height;
            Topology = topology;

            BirthThreshold = birthThreshold;
            MinSurvivalThreshold = minSurvivalThreshold;
            MaxSurvivalThreshold = maxSurvivalThreshold;

            currentGrid = new bool[width, height];
        }

        public Game(int width, int height, ITopology topology, int birthThreshold, int minSurvivalThreshold, int maxSurvivalThreshold, bool[,] storedGrid)
        {
            if (storedGrid.GetLength(0) != width || storedGrid.GetLength(1) != height)
            {
                throw new ArgumentException("The dimensions of the stored grid do not match the chosen width and height.");
            }

            Width = width;
            Height = height;
            Topology = topology;

            BirthThreshold = birthThreshold;
            MinSurvivalThreshold = minSurvivalThreshold;
            MaxSurvivalThreshold = maxSurvivalThreshold;

            currentGrid = (bool[,])storedGrid.Clone();  //performing deep copy
        }

        public bool[,] ExportGrid() => (bool[,]) currentGrid.Clone();

        private bool IsValidCoordinate(int x, int y) =>
            x >= 0 && x < Width && y >= 0 && y < Height;

        public bool GetCell(int x, int y)
        {
            if (!IsValidCoordinate(x, y))
            {
                return false;
            }
            return currentGrid[x, y];
        }

        public void SetCell(int x, int y, bool value)
        {
            if (IsValidCoordinate(x, y))
            {
                currentGrid[x, y] = value;
            }
        }

        private int CountLiveNeighbors(int x, int y)
        {
            int[] dx = { -1, -1, -1, 0, 0, 1, 1, 1 };
            int[] dy = { -1, 0, 1, 1, -1, -1, 0, 1 };

            int count = 0;
            int size = dx.Length;

            for (int i = 0; i < size; ++i)
            {
                int neighborX = x + dx[i];
                int neighborY = y + dy[i];

                var (wrappedX, wrappedY) = Topology.Wrap(neighborX, neighborY, Width, Height);

                if (GetCell(wrappedX, wrappedY))
                {
                    count++;
                }
            }
            return count;
        }

        public void AdvanceGeneration()
        {
            bool[,] nextGrid = new bool[Width, Height];
            for (int x = 0; x < Width; ++x)
            {
                for (int y = 0; y < Height; ++y)
                {
                    int neighbors = CountLiveNeighbors(x, y);
                    bool toLive = 
                        (GetCell(x, y) && neighbors >= MinSurvivalThreshold && neighbors <= MaxSurvivalThreshold) ||
                        (!GetCell(x, y) && neighbors == BirthThreshold);
                    nextGrid[x, y] = toLive;
                }
            }

            currentGrid = nextGrid;
        }

        public void Clear() => currentGrid = new bool[Width, Height];

        public void Randomize(double density)
        {
            for (int x = 0; x < Width; ++x)
            {
                for (int y = 0; y < Height; ++y)
                {
                    //coming from the expected value
                    currentGrid[x, y] = Random.Shared.NextDouble() < density; 
                }
            }
        }
    }
}
