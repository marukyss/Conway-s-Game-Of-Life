# User documentation

An implementation of a Conway's Game Of Life in C#.

## Introduction

---

Conway's Game of Life is a zero-player game (cellular automaton) created by the British mathematician John Horton Conway in 1970.  The game is based on the set configuration of live and dead cells on a grid and fixed rules. After triggering the start of the simulation, the game evolves based on the predefined parameters.

## Key features

---

-  **Custom Topologies** (Change the space on which the simulation runs. Supported spaces are: Euclidean plane, Torus, Klein Bottle, Mobius Strip)
-  **Predefined Stamps** (Directly "stamp" on the board famous configurations, such as gliders, pulsars, blinkers, toads and blocks)
- **Visual Customization** (Choose the colors of live and dead cells)
- **Custom Rules** (Specify the neighbor thresholds for the birth and death of cells)
- **I/O Features** (user can load previous game or store current state)

## Requirements

---


**Programming language + version:** C# (.NET 8.0)

**Used libraries:** System, System.Drawing, System.Windows.Forms, System.Text.Json

**Start of the program:** To start the game, run Program.cs.

## Game Rules

---

The **default rules** are:
1. **Underpopulation:** Any live cell with **fewer than 2** live neighbours dies, as if by underpopulation.
2. **Survival:** Any live cell with **2 or 3** live neighbours lives on to the next generation.
3. **Overpopulation:** Any live cell with **more than 3** live neighbours dies, as if by overpopulation.
4. **Reproduction:** Any dead cell with **exactly 3** live neighbours becomes a live cell, as if by reproduction.
To start the simulation the user may change the default rules, colors and space, put some alive cells or "stamp" patterns and then build/run a simulation. Also, user can press stop or load/store buttons.