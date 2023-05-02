using SFML.Graphics;
using SFML.System;

namespace BabaIsYou.DataStructures
{
    public class GridNodeVisual : Drawable
    {
        private NodeVisual nodeVisual;
        public Vector2u GridIndex { get; init; }
        public Node NodeType => nodeVisual.NodeType;

        public static GridNodeVisual FromNodeVisual(NodeVisual nodeVisual, Vector2u gridIndex)
        {
            return new GridNodeVisual()
            {
                nodeVisual = nodeVisual.Clone(),
                GridIndex = gridIndex
            };
        }

        public void Draw(RenderTarget target, RenderStates states)
        {
            target.Draw(nodeVisual, states);
        }

        public void SetPosition(Vector2f position)
        {
            nodeVisual.SetPosition(position);
        }
    }
}
