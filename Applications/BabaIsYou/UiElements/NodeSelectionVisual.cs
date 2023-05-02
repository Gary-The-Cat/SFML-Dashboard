using BabaIsYou.DataStructures;
using CSharpFunctionalExtensions;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using Shared.Events.EventArgs;
using System.Collections.Generic;
using System.Linq;

namespace BabaIsYou.UiElements
{
    public class NodeSelectionVisual : Drawable
    {
        private NodeSelectionVisual(IEnumerable<NodeVisual> visuals, Vector2f screenPosition)
        {
            this.visuals = visuals.ToList();
            foreach (var visual in this.visuals)
            {
                visual.SetPosition(screenPosition);
            }

            this.SelectedVisual = this.visuals.First();
        }

        private readonly List<NodeVisual> visuals = new List<NodeVisual>();

        public NodeVisual SelectedVisual { get; private set; }

        public void Draw(RenderTarget target, RenderStates states)
        {
            target.Draw(SelectedVisual, states);
        }

        public void OnKeyPress(Keyboard.Key code)
        {
            var currentIndex = visuals.IndexOf(SelectedVisual);

            if (code == SFML.Window.Keyboard.Key.Left)
            {
                currentIndex = currentIndex - 1;
                if (currentIndex < 0)
                {
                    currentIndex = visuals.Count - 1;
                }
            }

            if (code == SFML.Window.Keyboard.Key.Right)
            {
                currentIndex = currentIndex + 1;
                if (currentIndex == visuals.Count)
                {
                    currentIndex = 0;
                }
            }

            SelectedVisual = visuals[currentIndex];
        }

        public static Maybe<NodeSelectionVisual> Create(IEnumerable<NodeVisual> visuals, Vector2f screenPosition)
        {
            if (visuals == null || !visuals.Any())
            {
                return Maybe.None;
            }

            return new NodeSelectionVisual(visuals, screenPosition);
        }
    }
}
