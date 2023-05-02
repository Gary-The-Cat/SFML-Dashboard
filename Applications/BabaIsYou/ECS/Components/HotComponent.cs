namespace BabaIsYou.ECS.Components;
public class HotComponent : IComponent<HotComponent>
{
    public HotComponent Clone() => new();
}
