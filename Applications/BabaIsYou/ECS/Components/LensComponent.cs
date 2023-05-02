using SFML.Graphics;

namespace BabaIsYou.ECS.Components;

public class LensComponent : IComponent<LensComponent>
{
    public View View { get; set; }

    public LensComponent Clone() => new LensComponent { View = View };
}