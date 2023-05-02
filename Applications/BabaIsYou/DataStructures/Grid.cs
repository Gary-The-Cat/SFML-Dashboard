using BabaIsYou.DataStructures;
using CSharpFunctionalExtensions;
using SFML.System;

namespace BabaIsYou.ECS.Components
{
    public struct Grid
    {
        public Grid(GridConfiguration config)
        {
            this.Configuration= config;
            this.GridIndexSize = config.GridIndexSize;
            this.CellWidth = config.CellWidth;
            this.CellHeight = config.CellHeight;
        }

        public Vector2u GridIndexSize { get; }

        public float CellWidth { get; }

        public float CellHeight { get; }

        public float GridWidth => CellWidth * GridIndexSize.X;

        public float GridHeight => CellHeight * GridIndexSize.Y;

        public int TotalGridCellCount => (int)(GridIndexSize.X * GridIndexSize.Y);

        public Vector2f GridSize => new Vector2f(GridWidth, GridHeight);

        public GridConfiguration Configuration { get; }

        internal Maybe<Vector2f> GetCellTopLeftFromIndex(Vector2u cellIndex)
        {
            if (cellIndex.X > GridIndexSize.X || cellIndex.Y > GridIndexSize.Y)
            {
                return Maybe.None;
            }

            return new Vector2f(
                cellIndex.X * CellWidth,
                cellIndex.Y * CellHeight);
        }

        internal Maybe<Vector2u> GetCell(Vector2i screenPosition)
        {
            // Check that the click was within the bounds of the grid
            var isAboveGrid = screenPosition.X >= GridWidth || screenPosition.Y >= GridHeight;
            var isBelowGrid = screenPosition.X < 0 || screenPosition.Y < 0;

            if (isBelowGrid || isAboveGrid)
            {
                return Maybe.None;
            }

            var x = (uint)((screenPosition.X / GridWidth) * GridIndexSize.X);
            var y = (uint)((screenPosition.Y / GridHeight) * GridIndexSize.Y);

            return new Vector2u(x, y);
        }
    }
}
