using BabaIsYou.DataStructures;
using BabaIsYou.ECS.Components;
using DefaultEcs;
using DefaultEcs.System;

namespace BabaIsYou.ECS.Systems
{
    internal class IndexPositionUpdaterSystem : ISystem<StepData>
    {
        private readonly GridVisual grid;
        private readonly World world;

        public IndexPositionUpdaterSystem(GridVisual grid, World world)
        {
            this.grid = grid;
            this.world = world;
        }   

        private EntitySet positionComponentFilter => world.GetEntities()
            .With<IndexPositionComponent>()
            .With<PositionComponent>()
            .AsSet();

        public bool IsEnabled { get; set; }

        public void Update(StepData t)
        {
            foreach (var entity in positionComponentFilter.GetEntities())
            {
                var indexComponent = entity.Get<IndexPositionComponent>();
                indexComponent.Position.X = (uint)indexComponent.QueuedPosition.X;
                indexComponent.Position.Y = (uint)indexComponent.QueuedPosition.Y;

                var maybePosition = grid.GetCellTopLeftFromIndex(indexComponent.Position);

                if (maybePosition.HasValue)
                {
                    var positionComponent = entity.Get<PositionComponent>();
                    positionComponent.Position = maybePosition.Value;
                }
            }
        }

        public void Dispose() { }
    }
}
