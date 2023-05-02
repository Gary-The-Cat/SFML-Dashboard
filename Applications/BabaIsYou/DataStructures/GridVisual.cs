using BabaIsYou.DataStructures;
using CSharpFunctionalExtensions;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using Shared.ExtensionMethods;

namespace BabaIsYou.ECS.Components;

public class GridVisual : Drawable
{
    public bool IsDebug { get; set; }

    public Vector2f Position { get; private set; }

    private Grid grid;

    private RectangleShape background;

    private static Color Colour = new Color(0xaa, 0xaa, 0xaa);

    private SelectedGridCell selectedGridCell = new SelectedGridCell();
    
    public float GridWidth => grid.CellWidth * grid.GridIndexSize.X;

    public float GridHeight => grid.CellHeight * grid.GridIndexSize.Y;
    
    public uint GridIndexWidth => grid.GridIndexSize.X;

    public uint GridIndexHeight => grid.GridIndexSize.Y;

    public Vector2f GridSize => new Vector2f(GridWidth, GridHeight);

    public Vector2u SelectedCell => selectedGridCell.SelectedCell;

    public GridConfiguration GridConfiguration => grid.Configuration;

    internal Maybe<Vector2f> GetCellTopLeftFromIndex(Vector2u cellIndex)
    {
        var result = grid.GetCellTopLeftFromIndex(cellIndex);

        if (result.HasValue)
        {
            return result.Value + Position;
        }

        return result;
    }

    internal Maybe<Vector2u> GetCell(Vector2i screenPosition) =>
        grid.GetCell(screenPosition - Position.ToVector2i());

    internal bool Contains(Vector2i cell) =>
        cell.X >= 0 &&
        cell.Y >= 0 &&
        cell.X < grid.GridIndexSize.X &&
        cell.Y < grid.GridIndexSize.Y;

    public void OnUpdate()
    {
        if (Keyboard.IsKeyPressed(Keyboard.Key.Left))
        {
            if(selectedGridCell.SelectedCell.X > 0)
            {
                selectedGridCell.SelectedCell = new Vector2u(
                    selectedGridCell.SelectedCell.X - 1,
                    selectedGridCell.SelectedCell.Y);
            }
        }

        if (Keyboard.IsKeyPressed(Keyboard.Key.Right))
        {
            if (selectedGridCell.SelectedCell.X < grid.GridIndexSize.X - 1)
            {
                selectedGridCell.SelectedCell = new Vector2u(
                    selectedGridCell.SelectedCell.X + 1,
                    selectedGridCell.SelectedCell.Y);
            }
        }

        if (Keyboard.IsKeyPressed(Keyboard.Key.Up))
        {
            if (selectedGridCell.SelectedCell.Y > 0)
            {
                selectedGridCell.SelectedCell = new Vector2u(
                    selectedGridCell.SelectedCell.X,
                    selectedGridCell.SelectedCell.Y - 1);
            }
        }

        if (Keyboard.IsKeyPressed(Keyboard.Key.Down))
        {
            if (selectedGridCell.SelectedCell.Y < grid.GridIndexSize.Y - 1)
            {
                selectedGridCell.SelectedCell = new Vector2u(
                    selectedGridCell.SelectedCell.X,
                    selectedGridCell.SelectedCell.Y + 1);
            }
        }

        selectedGridCell.Sprite.Position =
            grid.GetCellTopLeftFromIndex(selectedGridCell.SelectedCell).Value
            + Position;
    }

    public void Draw(RenderTarget target, RenderStates states)
    {
        target.Draw(background, states);

        if (!IsDebug)
        {
            return;
        }

        var gridIndexSize = grid.GridIndexSize;
        var cellWidth = grid.CellWidth;
        var cellHeight = grid.CellHeight;

        // DEBUG USE ONLY
        // Get the lines indicated by the grid
        for (uint x = 0; x < gridIndexSize.X; x++)
        {
            var min = new Vector2f(Position.X + x * cellWidth, Position.Y + 0);
            var max = new Vector2f(Position.X + x * cellWidth, Position.Y + gridIndexSize.Y * cellHeight);

            target.DrawLine(min, max, Colour);
        }

        target.DrawLine(
            new Vector2f(Position.X + gridIndexSize.X * cellWidth, Position.Y),
            new Vector2f(Position.X + gridIndexSize.X * cellWidth, Position.Y + gridIndexSize.Y * cellHeight),
            Colour);

        for (uint y = 0; y < gridIndexSize.Y; y++)
        {
            var min = new Vector2f(Position.X + 0, Position.Y + y * cellWidth);
            var max = new Vector2f(Position.X + gridIndexSize.X * cellWidth, Position.Y + y * cellHeight);

            target.DrawLine(min, max, Colour);
        }

        target.DrawLine(
            new Vector2f(Position.X + 0, Position.Y + gridIndexSize.Y * cellHeight),
            new Vector2f(Position.X + gridIndexSize.X * cellWidth, Position.Y + gridIndexSize.Y * cellHeight),
            Colour);
    }

    public void SetCentre(Vector2f position)
    {
        var topLeft = position - GridSize / 2;

        Position = topLeft;

        background.Position = topLeft;
    }

    public static GridVisual CreateFromGrid(Grid grid)
    {
        var gridSize = new Vector2f(
            grid.CellWidth * grid.GridIndexSize.X,
            grid.CellHeight * grid.GridIndexSize.Y);

        return new GridVisual()
        {
            grid = grid,
            background = new RectangleShape(gridSize)
            {
                FillColor = new Color(3, 3, 3),
            }
        };
    }
}
