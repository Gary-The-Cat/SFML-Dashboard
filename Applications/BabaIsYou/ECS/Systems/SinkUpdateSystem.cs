using BabaIsYou.DataStructures;
using BabaIsYou.ECS.Components;
using DefaultEcs;
using DefaultEcs.System;
using SFML.System;

namespace BabaIsYou.ECS.Systems;

internal class SinkUpdateSystem : ISystem<StepData>
{
    private World world;

    private EntitySet sinkComponentFilter => world.GetEntities()
        .With<SinkComponent>()
        .With<IndexPositionComponent>()
        .AsSet();

    private EntitySet allEntitiesFilter => world.GetEntities()
        .With<IndexPositionComponent>()
        .AsSet();

    public SinkUpdateSystem(World world)
    {
        this.world = world;
    }

    public bool IsEnabled { get; set; }

    public void Update(StepData t)
    {
        var sinkPositions = new List<Vector2u>();
        var sinkEntities = new Dictionary<Vector2u, Entity>();
        foreach (var sinkEntity in sinkComponentFilter.GetEntities())
        {
            var position = sinkEntity.Get<IndexPositionComponent>().Position;
            sinkEntities.Add(position, sinkEntity);
            sinkPositions.Add(position);
        }

        foreach (var entity in allEntitiesFilter.GetEntities())
        {
            if (sinkEntities.ContainsValue(entity))
            {
                continue;
            }

            var position = entity.Get<IndexPositionComponent>().Position;
            if (sinkPositions.Contains(position))
            {
                // Note calling dispose here rather than disable will permanently remove the entity.
                entity.Dispose();
                sinkEntities[position].Dispose();
            }
        }
    }

    public void Dispose() { }
}
