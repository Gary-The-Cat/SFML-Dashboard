namespace BabaIsYou.ECS.Components
{
    public class YouComponent : IComponent<YouComponent>
    {
        public YouComponent Clone() => new();
    }
}
