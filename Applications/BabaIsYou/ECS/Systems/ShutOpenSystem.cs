using BabaIsYou.DataStructures;
using BabaIsYou.ECS.Components;
using DefaultEcs;
using DefaultEcs.System;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BabaIsYou.ECS.Systems;
internal class ShutOpenSystem : ISystem<StepData>
{
    private World world;

    public ShutOpenSystem(World world)
    {
        this.world = world;
    }

    private EntitySet shutComponentFilter => world.GetEntities()
        .With<ShutComponent>()
        .With<IndexPositionComponent>()
        .AsSet();

    private EntitySet openComponentFilter => world.GetEntities()
        .With<OpenComponent>()
        .With<IndexPositionComponent>()
        .AsSet();

    public bool IsEnabled { get; set; } = true;

    public void Dispose() { }

    public void Update(StepData state)
    {
        var shutPositions = new Dictionary<Vector2u, Entity>();
        foreach (var shutEntity in shutComponentFilter.GetEntities())
        {
            shutPositions.Add(shutEntity.Get<IndexPositionComponent>().Position, shutEntity);
        }

        foreach (var openEntity in openComponentFilter.GetEntities())
        {
            if (shutPositions.TryGetValue(openEntity.Get<IndexPositionComponent>().Position, out var shutEntity))
            {
                openEntity.Dispose();
                shutEntity.Dispose();
            }
        }
    }
}
