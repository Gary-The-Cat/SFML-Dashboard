using SFML.Graphics;
using SFML.System;

namespace BabaIsYou.ECS.Components
{
    public class SelectedGridCell
    {
        public Vector2u SelectedCell { get; set; }

        public RectangleShape Sprite { get; set; }

        public SelectedGridCell()
        {
            SelectedCell = new Vector2u(0, 0);

            Sprite = new RectangleShape()
            {
                Size = new Vector2f(64, 64),
                OutlineColor = Color.Blue,
                OutlineThickness = 2,
                FillColor = Color.Transparent,
            };
        }
    }
}
