namespace BabaIsYou.ECS.Components
{
    public class TilePlacementComponent : IComponent<TilePlacementComponent>
    {
        public TilePlacementComponent Clone() => new();
    }
}
