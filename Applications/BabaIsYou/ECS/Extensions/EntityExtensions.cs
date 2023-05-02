using BabaIsYou.ECS.Components;
using DefaultEcs;

namespace BabaIsYou.ECS.Extensions;
internal static class EntityExtensions
{
    public static IEnumerable<object> GetComponents(this Entity entity)
    {
        if (entity.Has<IndexPositionComponent>())
        {
            yield return entity.Get<IndexPositionComponent>().Clone();
        }

        if (entity.Has<PositionComponent>())
        {
            yield return entity.Get<PositionComponent>().Clone();
        }

        if (entity.Has<SpriteComponent>())
        {
            yield return entity.Get<SpriteComponent>().Clone();
        }

        if (entity.Has<LensComponent>())
        {
            yield return entity.Get<LensComponent>().Clone();
        }

        if (entity.Has<NodeTypeComponent>())
        {
            yield return entity.Get<NodeTypeComponent>().Clone();
        }

        if (entity.Has<PushComponent>())
        {
            yield return entity.Get<PushComponent>().Clone();
        }

        if (entity.Has<StopComponent>())
        {
            yield return entity.Get<StopComponent>().Clone();
        }

        if (entity.Has<TilePlacementComponent>())
        {
            yield return entity.Get<TilePlacementComponent>().Clone();
        }

        if (entity.Has<WinComponent>())
        {
            yield return entity.Get<WinComponent>().Clone();
        }

        if (entity.Has<YouComponent>())
        {
            yield return entity.Get<YouComponent>().Clone();
        }

        if (entity.Has<SinkComponent>())
        {
            yield return entity.Get<SinkComponent>().Clone();
        }

        if (entity.Has<DefeatComponent>())
        {
            yield return entity.Get<DefeatComponent>().Clone();
        }

        if (entity.Has<HotComponent>())
        {
            yield return entity.Get<HotComponent>().Clone();
        }

        if (entity.Has<MeltComponent>())
        {
            yield return entity.Get<MeltComponent>().Clone();
        }

        if (entity.Has<TeleComponent>())
        {
            yield return entity.Get<TeleComponent>().Clone();
        }
    }

    public static void AddComponents(this Entity entity, IEnumerable<object> components)
    {
        foreach (var component in components)
        {
            switch (component)
            {
                case IndexPositionComponent indexPositionComponent: entity.Set(indexPositionComponent);
                    break;
                case PositionComponent positionComponent: entity.Set(positionComponent);
                    break;
                case SpriteComponent spriteComponent: entity.Set(spriteComponent);
                    break;
                case LensComponent lensComponent: entity.Set(lensComponent);
                    break;
                case NodeTypeComponent nodeTypeComponent: entity.Set(nodeTypeComponent);
                    break;
                case PushComponent pushComponent: entity.Set(pushComponent);
                    break;
                case StopComponent stopComponent: entity.Set(stopComponent);
                    break;
                case TilePlacementComponent tilePlacementComponent: entity.Set(tilePlacementComponent);
                    break;
                case WinComponent winComponent: entity.Set(winComponent);
                    break;
                case YouComponent youComponent: entity.Set(youComponent);
                    break;
                case SinkComponent sinkComponent: entity.Set(sinkComponent);
                    break;
                case DefeatComponent defeatComponent: entity.Set(defeatComponent);
                    break;
                case HotComponent hotComponent: entity.Set(hotComponent);
                    break;
                case MeltComponent meltComponent: entity.Set(meltComponent);
                    break;
                case TeleComponent teleComponent: entity.Set(teleComponent);
                    break;
                default:
                    break;

            }
        }
    }
}
