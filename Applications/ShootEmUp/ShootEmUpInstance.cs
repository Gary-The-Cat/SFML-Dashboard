using Ninject;
using SFML.Graphics;
using SFML.System;
using Shared.Core;
using Shared.Interfaces;
using Shared.Interfaces.Services;
using ShootEmUp.Screens;
using System;

namespace ShootEmUp
{
    public class ShootEmUpInstance : ApplicationInstanceBase, IApplicationInstance
    {
        private IApplicationService appService;

        public ShootEmUpInstance(IApplicationService appService)
        {
            this.appService = appService;

            var texture = new Texture(new Image("Resources//ShootEmUpIcon.png"));
            texture.GenerateMipmap();
            texture.Smooth = true;

            Thumbnail = new RectangleShape(new Vector2f(300, 300))
            {
                Texture = texture
            };
        }

        public string DisplayName => "Shoot-Em-Up";

        public override void Initialize()
        {
            var roverSimulation = appService.Kernel.Get<HomeScreen>();
            AddChildScreen(roverSimulation);

            base.Initialize();
        }
    }
}
