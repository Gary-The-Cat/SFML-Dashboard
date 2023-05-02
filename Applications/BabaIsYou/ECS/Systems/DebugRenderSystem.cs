using BabaIsYou.DataStructures;
using BabaIsYou.ECS.Components;
using DefaultEcs;
using DefaultEcs.System;
using SFML.Graphics;

namespace BabaIsYou.ECS.Systems
{
    internal class DebugRenderSystem : ISystem<RenderTarget>
    {
        private readonly World world;

        public DebugRenderSystem(World world)
        {
            this.world = world;
        }

        public bool IsEnabled { get; set; }

        private EntitySet gridComponentfilter => world.GetEntities()
            .With<GridVisual>()
            .AsSet();

        public void Dispose() {}

        public void Update(RenderTarget target)
        {
            foreach (var gridEntity in gridComponentfilter.GetEntities())
            {
                target.Draw(gridEntity.Get<GridVisual>());
            }
        }
    }
}
