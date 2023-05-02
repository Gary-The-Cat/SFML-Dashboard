using Newtonsoft.Json;
using SFML.Graphics;

namespace BabaIsYou.ECS.Components;

public class SpriteComponent : IComponent<SpriteComponent>
{
    private string spriteFile;

    [JsonIgnore]
    public Sprite Sprite { get; set; }

    public string SpriteFile
    {
        get => spriteFile;
        set
        {
            spriteFile = value;
            if (!string.IsNullOrEmpty(spriteFile))
            {
                var texture = new Texture(spriteFile);
                Sprite = new Sprite(texture);
            }
            else
            {
                Sprite = null;
            }
        }
    }

    public SpriteComponent Clone() => new SpriteComponent()
    {
        Sprite = new Sprite(this.Sprite),
    };
}