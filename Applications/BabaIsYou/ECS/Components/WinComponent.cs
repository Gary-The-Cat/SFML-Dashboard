namespace BabaIsYou.ECS.Components
{
    public class WinComponent : IComponent<WinComponent>
    {
        public WinComponent Clone() => new();
    }
}
