using BabaIsYou.DataStructures;
using BabaIsYou.ECS.Components;
using DefaultEcs;
using DefaultEcs.System;
using SFML.System;

namespace BabaIsYou.ECS.Systems;

internal class TeleSystem : ISystem<StepData>
{
    private World world;

    public TeleSystem(World world)
    {
        this.world = world;
    }

    private EntitySet teleComponentFilter => world.GetEntities()
        .With<TeleComponent>()
        .With<IndexPositionComponent>()
        .AsSet();

    private EntitySet positionComponentFilter => world.GetEntities()
        .With<IndexPositionComponent>()
        .AsSet();

    public bool IsEnabled { get; set; } = true;

    public void Dispose() { }

    public void Update(StepData state)
    {
        var telePositions = new Dictionary<Vector2i, Vector2i>();
        var entities = teleComponentFilter.GetEntities();

        if (entities.IsEmpty)
        {
            return;
        }

        Vector2i previousPosition = entities[entities.Length - 1].Get<IndexPositionComponent>().QueuedPosition;

        foreach (var entity in teleComponentFilter.GetEntities())
        {
            var position = entity.Get<IndexPositionComponent>().QueuedPosition;
            telePositions.Add(previousPosition, position);
            previousPosition = position;
        }

        foreach (var entity in positionComponentFilter.GetEntities())
        {
            if (teleComponentFilter.Contains(entity))
            {
                continue;
            }

            var positionComponent = entity.Get<IndexPositionComponent>();
            if (telePositions.TryGetValue(positionComponent.QueuedPosition, out var newPosition))
            {
                positionComponent.QueuedPosition = newPosition;
            }
        }
    }
}
