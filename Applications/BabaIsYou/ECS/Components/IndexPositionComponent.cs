using SFML.System;

namespace BabaIsYou.ECS.Components;

public class IndexPositionComponent : IComponent<IndexPositionComponent>
{
    public Vector2u Position;
    public Vector2i QueuedPosition;
    public Vector2i MovePosition;

    public IndexPositionComponent()
    {
        Position = new Vector2u(0, 0);
        QueuedPosition = new Vector2i(0, 0);
        MovePosition = new Vector2i(0, 0);
    }

    public IndexPositionComponent Clone() => new IndexPositionComponent()
    {
        Position = Position,
        QueuedPosition = QueuedPosition,
        MovePosition = MovePosition,
    };
}
