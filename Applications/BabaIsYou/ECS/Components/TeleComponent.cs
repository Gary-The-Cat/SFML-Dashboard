namespace BabaIsYou.ECS.Components;
internal class TeleComponent : IComponent<TeleComponent>
{
    public TeleComponent Clone() => new();
}
