using SFML.Graphics;

namespace BabaIsYou.ECS.Components;
public class AnimatedSpriteBase : SpriteComponent
{
    private int currentFrame;

    public AnimatedSpriteBase()
    {
        TextureRects = new List<IntRect>();
    }

    internal List<IntRect> TextureRects;

    public float FrameTime { get; set; }

    public float CurerntTime { get; set; }

    public int CurrentFrame
    {
        get => this.currentFrame;
        set
        {
            this.currentFrame = value;
            this.Sprite.TextureRect = TextureRects[this.CurrentFrame];
        }
    }

    public void AddFrames(IEnumerable<IntRect> textureRects)
    {
        foreach (var textureRect in textureRects)
        {
            this.TextureRects.Add(textureRect);
        }
    }
}
