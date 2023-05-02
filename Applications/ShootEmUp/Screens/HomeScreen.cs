using Leopotam.Ecs;
using Ninject;
using Ninject.Parameters;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using Shared.Core;
using Shared.Core.Hierarchy;
using Shared.Events.CallbackArgs;
using Shared.Interfaces;
using Shared.Interfaces.Services;
using Shared.Menus;
using ShootEmUp.Development;
using System;

namespace ShootEmUp.Screens
{
    public class HomeScreen : Screen
    {
        private IApplicationService appService;
        private IEventService eventService;
        private IApplicationManager applicationManager;

        private GridScreen gridScreen;
        private Screen gameScreen;

        public HomeScreen(
            IApplicationService appService,
            IEventService eventService,
            IApplicationManager appManager)
        {
            this.appService = appService;
            this.eventService = eventService;
            this.applicationManager = appManager;

            gridScreen = appService.Kernel.Get<GridScreen>();

            eventService.RegisterMouseClickCallback(
                this.Id,
                new MouseClickCallbackEventArgs(Mouse.Button.Left),
                gridScreen.OnMousePress);

            gridScreen.AddMenuItem(0, 0, GetGameMenuItem());
        }

        public override void OnUpdate(float deltaT)
        {
            base.OnUpdate(deltaT);
        }

        public override void OnRender(RenderTarget target)
        {
            base.OnRender(target);

            gridScreen.OnRender(target);
        }

        private MenuItem GetGameMenuItem()
        {
            var gameMenuItem = new MenuItem("StartNewGame");
            var gameMenuIcon = new Texture(new Image("Resources/Controller.png"));
            gameMenuIcon.GenerateMipmap();
            gameMenuIcon.Smooth = true;

            gameMenuItem.Canvas = new RectangleShape(new Vector2f(512, 512))
            {
                Texture = gameMenuIcon,
            };

            gameMenuItem.OnClick = () =>
            {
                if (gameScreen == null)
                {
                    gameScreen = GetGameScreen();
                    this.applicationManager.AddChildScreen(gameScreen);
                }

                this.applicationManager.SetActiveScreen(gameScreen);
            };

            return gameMenuItem;
        }

        private Screen GetGameScreen()
        {
            var output = this.appService.Kernel.Get<Game>();

            return output;
        }
    }
}
