using SFML.Graphics;
using SFML.System;

namespace BabaIsYou.Screens.LevelSelect
{
    public class LevelThumbnail
    {
        public Sprite LevelSprite { get; init; }

        public Vector2u IndexPosition { get; init; }

        public string LevelPath { get; init; }
    }
}
