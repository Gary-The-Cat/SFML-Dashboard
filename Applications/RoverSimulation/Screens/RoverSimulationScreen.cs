using Ninject;
using Ninject.Parameters;
using RoverSimulation.DataStructures;
using RoverSimulation.Screens;
using Shared.Core;
using Shared.Core.Hierarchy;
using Shared.Interfaces;

namespace RoverSimulation
{
    public class RoverSimulationScreen : StackedScreen
    {
        private IKernel kernel;

        public RoverSimulationScreen(IKernel kernel)
        {
            this.kernel = kernel;
        }

        public override void InitializeScreen()
        {
            var sharedRoverData = new SharedRoverData();
            var constructorArgument = new ConstructorArgument("roverData", sharedRoverData);


            var grid = kernel.Get<RoverSimulationGridScreen>(constructorArgument);
            var hud = kernel.Get<RoverSimulationHudScreen>(constructorArgument);

            AddScreen(grid);
            AddScreen(hud);

            grid.InitializeScreen();
        }
    }
}
