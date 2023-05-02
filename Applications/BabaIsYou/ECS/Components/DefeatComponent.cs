namespace BabaIsYou.ECS.Components;

public class DefeatComponent : IComponent<DefeatComponent>
{
    public DefeatComponent Clone() => new ();
}
