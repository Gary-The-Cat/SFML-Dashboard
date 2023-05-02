namespace BabaIsYou.ECS.Components
{
    internal class PushComponent : IComponent<PushComponent>
    {
        internal bool IsTransient { get; set; } = true;

        public PushComponent Clone() => new PushComponent { IsTransient = this.IsTransient };
    }
}
