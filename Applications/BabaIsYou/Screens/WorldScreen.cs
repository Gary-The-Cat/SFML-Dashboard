using BabaIsYou.DataStructures;
using BabaIsYou.ECS.Components;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using Shared.Core;
using Shared.Interfaces;

namespace BabaIsYou.Screens.LevelSelect
{
    public class WorldScreen : Screen
    {
        private IApplicationManager appManager;
        private Dictionary<Vector2u, LevelThumbnail> levels;
        private GridVisual levelSelectGrid;

        public WorldScreen(IApplicationManager appManager)
        {
            this.appManager = appManager;
            levels = new Dictionary<Vector2u, LevelThumbnail>();
            var gridConfiguration = new GridConfiguration()
            {
                GridIndexSize = new Vector2u(2, 2),
                CellWidth = 64,
                CellHeight = 64,
            };

            levelSelectGrid = GridVisual.CreateFromGrid(new Grid(gridConfiguration));

            var screenSize = new Vector2f(1920, 1080);

            this.levelSelectGrid.SetCentre(screenSize / 2);

            levelSelectGrid.GetCell(new Vector2i(0, 0));

            for (uint i = 0; i < 4; i++)
            {
                var level = new LevelThumbnail()
                {
                    LevelSprite = new Sprite(new Texture($"Resources/LevelImages/{i + 1}.png")),
                    IndexPosition = new Vector2u(i % 2, i / 2),
                    LevelPath = $"Resources/Levels/Level {i + 1}.level",
                };

                level.LevelSprite.Position =
                    levelSelectGrid.GetCellTopLeftFromIndex(level.IndexPosition).Value;

                levels.Add(level.IndexPosition, level);
            }
        }

        public override void OnUpdate(float deltaT)
        {
            base.OnUpdate(deltaT);

            levelSelectGrid.OnUpdate();

            if (Keyboard.IsKeyPressed(Keyboard.Key.Enter))
            {
                var selectedCell = levelSelectGrid.SelectedCell;

                // :TODO: We will have to cache the screens we have loaded previous to kill them or re-show them
                var levelThumbnail = levels[selectedCell];
                ////var level = Levels.LevelScreen.TryCreateLevelFromFile(levelThumbnail.LevelPath);
                ////if (level.HasValue)
                ////{
                ////    appManager.AddChildScreen(level.Value);
                ////}
            }
        }

        public override void OnRender(RenderTarget target)
        {
            target.Draw(levelSelectGrid);

            foreach (var level in levels)
            {
                target.Draw(level.Value.LevelSprite);
            }
        }
    }
}
