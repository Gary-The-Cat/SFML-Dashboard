using Ninject;
using SFML.Graphics;
using SFML.System;
using Shared.Core;
using Shared.Interfaces;
using Shared.Interfaces.Services;
using System;

namespace RoverSimulation
{
    public class RoverSimulationInstance : ApplicationInstanceBase, IApplicationInstance
    {
        private IApplicationService appService;

        public RoverSimulationInstance(IApplicationService appService)
        {
            this.appService = appService;

            var texture = new Texture(new Image("Resources\\RoverIcon.png"));
            texture.GenerateMipmap();
            texture.Smooth = true;

            Thumbnail = new RectangleShape(new Vector2f(300, 300))
            {
                Texture = texture
            };
        }

        public string DisplayName => "Rover Simulation";

        public override void Initialize()
        {
            var roverSimulation = appService.Kernel.Get<RoverSimulationScreen>();
            AddChildScreen(roverSimulation);

            base.Initialize();
        }
    }
}
