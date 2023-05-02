namespace BabaIsYou.ECS.Components;

public class SinkComponent : IComponent<SinkComponent>
{
    public SinkComponent Clone() => new(); 
}
