using SFML.Graphics;
using SFML.System;

namespace BabaIsYou.DataStructures
{
    public class NodeVisual : Drawable
    {
        public Sprite Visual { get; private set; }

        public Node NodeType { get; init; }

        public NodeVisual(Sprite visual, Node nodeType)
        {
            this.Visual = visual;
            this.NodeType = nodeType;
        }

        public void Draw(RenderTarget target, RenderStates states)
        {
            target.Draw(Visual, states);
        }

        public NodeVisual Clone()
        {
            return new NodeVisual(new Sprite(Visual.Texture), NodeType);
        }

        public void SetPosition(Vector2f position)
        {
            Visual.Position = position;
        }
    }
}
