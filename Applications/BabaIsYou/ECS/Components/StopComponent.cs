namespace BabaIsYou.ECS.Components
{
    public class StopComponent : IComponent<StopComponent>
    {
        public StopComponent Clone() => new();
    }
}
