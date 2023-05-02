using RoverSimulation.DataStructures;
using SFML.Graphics;
using SFML.System;
using Shared.Core;
using Shared.Helpers;
using Shared.Interfaces;
using System;
using System.Collections.Generic;

namespace RoverSimulation.Screens
{
    public class RoverSimulationGridScreen : Screen
    {
        private IApplicationManager applicaitonManager;
        private SharedRoverData sharedRoverData;
        private List<Vector2u> path;
        private Vector2f windowSize;
        private Vector2f cellSize;
        private Vector2i gridSize;
        private Cell[,] cellVisuals;
        private Sprite background;
        private Rover rover;

        private bool isRunning;
        private float timeSinceLastStep;
        private int currentStep;

        private const float StepTime = 1;

        public RoverSimulationGridScreen(
            IApplicationManager applicaitonManager,
            SharedRoverData roverData)
        {
            this.applicaitonManager = applicaitonManager;
            this.sharedRoverData = roverData;
            this.sharedRoverData.StartNewSimulation = StartNewSimulation;
        }

        public override void InitializeScreen()
        {
            var size = applicaitonManager.GetWindowSize();
            var blueprint = new Texture(BlueprintHelper.CreateBlueprint(size.X, size.Y));
            background = new Sprite(blueprint);

            var windowSize = applicaitonManager.GetWindowSize();

            this.windowSize = new Vector2f(windowSize.X, windowSize.Y);
            this.gridSize = new Vector2i(15, 10);

            this.cellSize = GetGridSquareSize(this.windowSize, this.gridSize);
            this.cellVisuals = GetCells();

            this.rover = new Rover(cellSize);
        }

        public override void OnUpdate(float dt)
        {
            if (isRunning)
            {
                timeSinceLastStep += dt;

                if (timeSinceLastStep > StepTime)
                {
                    timeSinceLastStep = 0;
                    currentStep++;

                    if (path.Count == currentStep)
                    {
                        isRunning = false;
                        ShowAllCellsDesireability();
                        return;
                    }

                    var cellIndexPosition = path[currentStep];
                    var cell = cellVisuals[cellIndexPosition.Y, cellIndexPosition.X];

                    var newPosition = cell.Centroid;
                    rover.SetDesiredXPosition(newPosition.X, 0.9f);
                    rover.SetDesiredYPosition(newPosition.Y, 0.9f);

                    if (!cell.HasBeenVisited)
                    {
                        sharedRoverData.Gold += cell.GoldValue;
                        sharedRoverData.Fuel += cell.FuelValue;
                    }

                    sharedRoverData.Fuel -= 2;

                    cell.SetVisited();
                }

                rover.Update(dt);
            }

            foreach (var cell in cellVisuals)
            {
                cell.OnUpdate(dt);
            }
        }

        private void ShowAllCellsDesireability()
        {
            for (int y = 0; y < this.gridSize.Y; y++)
            {
                for (int x = 0; x < this.gridSize.X; x++)
                {
                    var cell = cellVisuals[x, y];
                    cell.MarkPath();
                }
            }
        }

        public override void OnRender(RenderTarget target)
        {
            target.Draw(background);

            foreach (var cell in cellVisuals)
            {
                cell.OnRender(target);
            }

            rover.OnRender(target);
        }

        private Cell[,] GetCells()
        {
            // Determine how large the grid cells can be

            Cell[,] visuals = new Cell[this.gridSize.X, this.gridSize.Y];

            var topLeft = GetPositionOfCell(0, 0);

            var fuelTexture = new Texture(new Image("Resources\\Battery.png"));
            fuelTexture.GenerateMipmap();
            fuelTexture.Smooth = true;

            var goldTexture = new Texture(new Image("Resources\\Gold.png"));
            goldTexture.GenerateMipmap();
            goldTexture.Smooth = true;

            var depletedGoldTexture = new Texture(new Image("Resources\\Gold_Depleted.png"));
            depletedGoldTexture.GenerateMipmap();
            depletedGoldTexture.Smooth = true;

            var depletedFuelTexture = new Texture(new Image("Resources\\Battery_Depleted.png"));
            depletedFuelTexture.GenerateMipmap();
            depletedFuelTexture.Smooth = true;

            for (int y = 0; y < this.gridSize.Y; y++)
            {
                for (int x = 0; x < this.gridSize.X; x++)
                {
                    var position = new Vector2f(
                        topLeft.X + x * cellSize.X,
                        topLeft.Y + y * cellSize.Y);

                    var cell = new Cell(
                        cellSize,
                        GetCellColour(y, x),
                        position,
                        fuelTexture,
                        goldTexture,
                        depletedFuelTexture,
                        depletedGoldTexture);

                    visuals[x, y] = cell;
                }
            }

            return visuals;
        }

        private Vector2f GetPositionOfCell(int x, int y)
        {
            var halfGridSize = new Vector2f(
                (gridSize.X * cellSize.X) / 2,
                (gridSize.Y * cellSize.Y) / 2);

            var centreScreen = windowSize / 2;

            var topLeft = centreScreen - halfGridSize;

            return new Vector2f(
                topLeft.X + cellSize.X * x,
                topLeft.Y + cellSize.Y * y);
        }

        private Color GetCellColour(int y, int x)
        {
            var low = new Color(0xB0, 0xBE, 0xC5);
            var high = new Color(0xCF, 0xD8, 0xDC);
            if (y % 2 == 0)
            {
                return (x % 2 == 0) ? low : high;
            }
            else
            {
                return (x % 2 == 0) ? high : low;
            }
        }

        private Vector2f GetGridSquareSize(Vector2f windowSize, Vector2i gridSize)
        {
            // :TODO: Actually calculate the max size
            return new Vector2f(80, 80);
        }

        public void StartNewSimulation(
            float[,] fuelValues,
            float[,] goldValues,
            List<Vector2u> path)
        {
            ResetState();
            var (normalizedFuelValues, normalizedGoldValues) = NormalizeFuelValues(fuelValues, goldValues);
            SetMap(fuelValues, normalizedFuelValues, goldValues, normalizedGoldValues);
            SetPath(path);
        }

        private (float[,], float[,]) NormalizeFuelValues(float[,] fuelValues, float[,] goldValues)
        {
            // Normalize the values
            var minFuel = float.MaxValue;
            var maxFuel = float.MinValue;
            var minGold = float.MaxValue;
            var maxGold = float.MinValue;

            var yCount = fuelValues.GetLength(1);
            var xCount = fuelValues.GetLength(0);

            for (int y = 0; y < yCount; y++)
            {
                for (int x = 0; x < xCount; x++)
                {
                    var fuelValue = fuelValues[x, y];
                    
                    if (fuelValue < minFuel)
                    {
                        minFuel = fuelValue;
                    }

                    if (fuelValue > maxFuel)
                    {
                        maxFuel = fuelValue;
                    }

                    var goldValue = goldValues[x, y];

                    if (goldValue < minGold)
                    {
                        minGold = goldValue;
                    }

                    if (goldValue > maxGold)
                    {
                        maxGold = goldValue;
                    }
                }
            }

            var normalizedFuelValues = new float[xCount, yCount];
            var normalizedGoldValues = new float[xCount, yCount];

            for (int y = 0; y < yCount; y++)
            {
                for (int x = 0; x < xCount; x++)
                {
                    normalizedFuelValues[x, y] = (fuelValues[x, y] - minFuel) / (maxFuel - minFuel);
                    normalizedGoldValues[x, y] = (goldValues[x, y] - minGold) / (maxGold - minGold);
                }
            }

            return (normalizedFuelValues, normalizedGoldValues);
        }

        public void ResetState()
        {
            for (int y = 0; y < this.gridSize.Y; y++)
            {
                for (int x = 0; x < this.gridSize.X; x++)
                {
                    cellVisuals[x, y].Reset();
                }
            }
        }

        public void SetMap(
            float[,] fuelValues,
            float[,] normalizedFuelValues,
            float[,] goldValues,
            float[,] normalizedGoldValues) 
        {
            for (int y = 0; y < this.gridSize.Y; y++)
            {
                for (int x = 0; x < this.gridSize.X; x++)
                {
                    cellVisuals[x, y].Initialize(
                        normalizedFuelValues[x, y],
                        normalizedGoldValues[x, y],
                        fuelValues[x, y],
                        goldValues[x, y]);
                }
            } 
        }

        public void SetPath(List<Vector2u> path)
        {
            this.path = path;

            this.isRunning = true;
            this.currentStep = -1;
            this.timeSinceLastStep = 0;
        }
    }
}
