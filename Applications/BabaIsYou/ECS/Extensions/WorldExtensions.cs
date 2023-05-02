using DefaultEcs;

namespace BabaIsYou.ECS.Extensions;

internal static class WorldExtensions
{
    internal static List<List<object>> Clone (this World world)
    {
        var clone = new List<List<object>> ();

        var entities = world.GetEntities().AsSet().GetEntities();
        foreach (var entity in entities)
        {
            clone.Add(entity.GetComponents().ToList());
        }

        return clone;
    }

    public static void PopulateFromClone(this World world, List<List<object>> entities)
    {
        foreach (var entity in entities)
        {
            var worldEntity = world.CreateEntity();
            worldEntity.AddComponents(entity);
        }
    }
}
