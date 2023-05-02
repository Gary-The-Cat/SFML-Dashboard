using BabaIsYou.DataStructures;
using BabaIsYou.ECS.Components;
using DefaultEcs;
using DefaultEcs.System;
using ReactiveUI;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BabaIsYou.ECS.Systems;
internal class MeltSystem : ISystem<StepData>
{
    private World world;

    public MeltSystem(World world)
    {
        this.world = world;
    }

    private EntitySet meltComponentFilter => world.GetEntities()
        .With<MeltComponent>()
        .With<IndexPositionComponent>()
        .AsSet();

    private EntitySet hotComponentFilter => world.GetEntities()
        .With<HotComponent>()
        .With<IndexPositionComponent>()
        .AsSet();

    public bool IsEnabled { get; set; } = true;

    public void Dispose() { }

    public void Update(StepData state)
    {
        var hotPositions = new List<Vector2u>();
        foreach (var hotEntity in hotComponentFilter.GetEntities())
        {
            hotPositions.Add(hotEntity.Get<IndexPositionComponent>().Position);
        }

        foreach (var meltEntity in meltComponentFilter.GetEntities())
        {
            if (hotPositions.Contains(meltEntity.Get<IndexPositionComponent>().Position))
            {
                meltEntity.Dispose();
            }
        }
    }
}
