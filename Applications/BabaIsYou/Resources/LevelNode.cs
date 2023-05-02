using BabaIsYou.Enums;
using SFML.System;

namespace BabaIsYou.Resources
{
    public class LevelNode
    {
        public Node NodeType { get; init; }

        public Direction Direction { get; init; }

        public Vector2u IndexPosition { get; init; }
    }
}
