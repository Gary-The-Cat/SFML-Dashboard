using BabaIsYou.ECS.Components;
using DefaultEcs;
using SFML.System;
using Shared.ExtensionMethods;

namespace BabaIsYou.DataStructures.DataGrid
{
    public class GridNode
    {
        public GridNode(
            Entity entity,
            IndexPositionComponent positionComponent)
        {
            this.Entity = entity;
            this.PositionComponent = positionComponent;
            this.MoveRequest = positionComponent.QueuedPosition - positionComponent.Position.ToVector2i();
        }

        public Entity Entity { get; }

        public IndexPositionComponent PositionComponent { get; }

        public Vector2i MoveRequest { get; }

        public Vector2u Position => PositionComponent.Position;

        public Vector2i QueuedPosition => PositionComponent.QueuedPosition;

        public bool HasMoveRequest => MoveRequest != default;
    }
}
