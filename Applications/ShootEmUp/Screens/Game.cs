using SFML.Graphics;
using SFML.Window;
using Shared.Core;
using Shared.Events.EventArgs;
using Shared.ExtensionMethods;
using Shared.ImageEffects;
using Shared.Interfaces;
using Shared.Interfaces.Services;
using ShootEmUp.Development;
using ShootEmUp.Enums;
using ShootEmUp.Helpers;
using System;
using System.Threading.Tasks;

namespace ShootEmUp.Screens
{
    public class Game : Screen
    {
        private GameState gameState;

        private GameRunner gameRunner;

        private Sprite pauseSprite;
        private Text pausedText;
        private Text loadingLevelText;
        private View defaultView;

        private bool newBackgroundRequired = true;
        int blurValue = 1;

        public Game(
            IApplicationManager appManager,
            IEventService eventService)
        {
            this.gameRunner = new GameRunner();
            this.gameState = GameState.Initializing;
            this.defaultView = appManager.GetDefaultView();
            var centre = this.defaultView.Size / 2;

            pausedText = new Text("Paused", new Font("Resources/Font.ttf"))
            {
                Position = new SFML.System.Vector2f(centre.X, centre.Y),
                FillColor = Color.White,
                CharacterSize = 180
            };

            loadingLevelText = new Text("Loading Level...", new Font("Resources/Font.ttf"))
            {
                Position = new SFML.System.Vector2f(centre.X, centre.Y),
                FillColor = Color.White,
                CharacterSize = 180
            };

            Task.Run(() => 
            {
                this.gameRunner.Initialize(this.defaultView);
                this.gameState = GameState.Running;
            });

            eventService.RegisterKeyboardCallback(
                this.Id,
                new Shared.Events.CallbackArgs.KeyPressCallbackEventArgs(Keyboard.Key.Tilde),
                args => this.OnPausePressed(args));
        }

        private void OnPausePressed(KeyboardEventArgs _)
        {
            this.gameState = this.gameState == GameState.Running
                ? GameState.Paused
                : GameState.Running;
        }

        public override void OnUpdate(float deltaT)
        {
            if (this.gameState != GameState.Running)
            {
                return;
            }

            this.gameRunner.OnUpdate(deltaT);
        }

        public override void OnRender(RenderTarget target)
        {
            switch (this.gameState)
            {
                case GameState.Initializing:
                    target.Clear(new Color(0x55, 0x55, 0x55));
                    target.DrawString(loadingLevelText);
                    return;
                case GameState.Paused:
                    RenderPausedGame(target);
                    break;
                 case GameState.Running:
                    this.gameRunner.OnRender(target);
                    this.newBackgroundRequired = true;
                    break;
                default:
                    throw new InvalidOperationException();
            }
        }

        private void RenderPausedGame(RenderTarget target)
        {
            if (this.newBackgroundRequired)
            {
                RenderTexture renderTexture = new RenderTexture(640, 360);
                Image sfmlImage = new Image(640, 360);

                renderTexture.Clear(Color.White);

                this.gameRunner.OnRender(renderTexture);

                var bitmap = renderTexture.Texture.CopyToImage().ConvertToBitmap();
                var blurredBitmap = GaussianBlur.Convolve(bitmap, GaussianBlur.GaussianKernel(blurValue, blurValue));

                for (uint y = 0; y < blurredBitmap.Height; y++)
                {
                    for (uint x = 0; x < blurredBitmap.Width; x++)
                    {
                        sfmlImage.SetPixel(x, y, blurredBitmap.GetPixel((int)x, (int)y).ToSfmlColour().Darken(20));
                    }
                }

                renderTexture.Texture.Update(sfmlImage);
                renderTexture.Display();
                this.pauseSprite = new Sprite(renderTexture.Texture) { Scale = new SFML.System.Vector2f(3, 3) };

                this.newBackgroundRequired = false;
            }

            target.Draw(this.pauseSprite);

            target.SetView(this.defaultView);
            target.DrawString(pausedText, true);
        }
    }
}
