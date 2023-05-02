namespace BabaIsYou.ECS.Components;

internal class ShutComponent : IComponent<ShutComponent>
{
    public ShutComponent Clone() => new ShutComponent();
}
