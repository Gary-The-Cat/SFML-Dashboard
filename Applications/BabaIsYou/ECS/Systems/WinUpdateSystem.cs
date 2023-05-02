using BabaIsYou.DataStructures;
using BabaIsYou.ECS.Components;
using DefaultEcs;
using DefaultEcs.System;
using ReactiveUI;
using SFML.System;

namespace BabaIsYou.ECS.Systems
{
    internal class WinUpdateSystem : ISystem<StepData>
    {
        private World world;

        private EntitySet winEntityFilter => this.world.GetEntities()
            .With<WinComponent>()
            .With<IndexPositionComponent>().AsSet();

        private EntitySet youEntityFilter => this.world.GetEntities()
            .With<YouComponent>()
            .With<IndexPositionComponent>().AsSet();

        public WinUpdateSystem(World world)
        {
            this.world = world;
        }

        public bool IsEnabled { get; set; } = true;

        public void Update(StepData t)
        {
            var winPositions = new List<Vector2u>();
            foreach (var winEntity in winEntityFilter.GetEntities())
            {
                winPositions.Add(winEntity.Get<IndexPositionComponent>().Position);
            }

            foreach (var youEntity in youEntityFilter.GetEntities())
            {
                if (winPositions.Contains(youEntity.Get<IndexPositionComponent>().Position))
                {
                    MessageBus.Current.SendMessage(new WinEvent());
                }
            }
        }

        public void Dispose() { }
    }
}
