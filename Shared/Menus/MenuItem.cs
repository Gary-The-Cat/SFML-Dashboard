﻿using SFML.Graphics;
using SFML.System;
using System;

namespace Shared.Menus
{
    public class MenuItem : IMenuItem, Drawable
    {
        public MenuItem(string id)
        {
            this.Id = id;
        }

        public string Id { get; }

        public string Label { get; set; }

        public Texture Image { get; set; }

        public RectangleShape Canvas { get; set; }

        public Vector2f Position { get; set; }

        public Action OnClick { get; set; }

        public void Draw(RenderTarget target, RenderStates states)
        {
            target.Draw(Canvas, states);

            // :TODO: Draw Label Here?
        }

        public Drawable GetDrawable()
        {
            return this;
        }

        public void SetDrawableOrigin(Vector2f size)
        {
            this.Canvas.Origin = size;
        }

        public void SetDrawablePosition(Vector2f size)
        {
            this.Canvas.Position = size;
        }

        public void SetDrawableSize(Vector2f size)
        {
            this.Canvas.Size = size;
        }
    }
}
