using BabaIsYou.DataStructures;
using BabaIsYou.ECS.Components;
using DefaultEcs.System;
using DefaultEcs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.System;

namespace BabaIsYou.ECS.Systems;
internal class DefeatSystem : ISystem<StepData>
{
    private World world;

    public DefeatSystem(World world)
    {
        this.world = world;
    }

    private EntitySet defeatComponentFilter => world.GetEntities()
        .With<DefeatComponent>()
        .With<IndexPositionComponent>()
        .AsSet();

    private EntitySet youComponentFilter => world.GetEntities()
        .With<YouComponent>()
        .With<IndexPositionComponent>()
        .AsSet();

    public bool IsEnabled { get; set; } = true;

    public void Dispose() { }

    public void Update(StepData state)
    {
        var defeatPositions = new List<Vector2u>();
        foreach (var hotEntity in defeatComponentFilter.GetEntities())
        {
            defeatPositions.Add(hotEntity.Get<IndexPositionComponent>().Position);
        }

        foreach (var youEntity in youComponentFilter.GetEntities())
        {
            if (defeatPositions.Contains(youEntity.Get<IndexPositionComponent>().Position))
            {
                youEntity.Dispose();
            }
        }
    }
}
