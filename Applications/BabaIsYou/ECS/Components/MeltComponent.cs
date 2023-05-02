namespace BabaIsYou.ECS.Components;

public class MeltComponent : IComponent<MeltComponent>
{
    public MeltComponent Clone() => new ();
}
