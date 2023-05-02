namespace BabaIsYou.ECS.Components
{
    public class NodeTypeComponent : IComponent<NodeTypeComponent>
    {
        public Node Node { get; set; }

        public NodeTypeComponent Clone() => new NodeTypeComponent { Node = this.Node };
    }
}
