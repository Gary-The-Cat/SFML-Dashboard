using SFML.System;

namespace BabaIsYou.ECS.Components;

public class PositionComponent : IComponent<PositionComponent>
{
    public Vector2f Position { get; set; } = new Vector2f(0, 0);

    // Offset is not currently compatible with all systems and will need to be integrated
    // as needed
    public Vector2f OffsetPosition { get; set; } = new Vector2f(0, 0);

    public Vector2f GetPosition() => Position + OffsetPosition;

    public PositionComponent Clone() => new PositionComponent
    {
        Position = this.Position,
        OffsetPosition = this.OffsetPosition
    };
}