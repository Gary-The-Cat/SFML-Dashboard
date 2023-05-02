using SFML.Graphics;
using static SFML.Window.Keyboard;

namespace Shared.ScreenConfig
{
    public class ScreenConfiguration
    {
        public float Scale => 1f;
        public static uint StaticHeight { get; set; } = 1080;
        public static uint StaticWidth { get; set; } = 1920;
        public uint Height { get; set; } = 1080;
        public uint Width { get; set; } = 1920;

        public bool AllowCameraMovement { get; set; } = true;
        public Key PanLeft { get; set; } = Key.A;
        public Key PanRight { get; set; } = Key.D;
        public Key PanUp { get; set; } = Key.W;
        public Key PanDown { get; set; } = Key.S;
        public Key ZoomIn { get; set; } = Key.Z;
        public Key ZoomOut { get; set; } = Key.X;
        public Key RotateRight { get; set; } = Key.Num1;
        public Key RotateLeft { get; set; } = Key.Num2;

        public static FloatRect SinglePlayer { get; set; } = new FloatRect(0, 0, 1, 1);
        public static FloatRect TwoPlayerLeft { get; set; } = new FloatRect(0, 0, 0.5f, 1);
        public static FloatRect TwoPlayerRight { get; set; } = new FloatRect(0.5f, 0, 0.5f, 1);
        public static FloatRect FourPlayerTopLeft { get; set; } = new FloatRect(0, 0, 0.5f, 0.5f);
        public static FloatRect FourPlayerTopRight { get; set; } = new FloatRect(0.5f, 0, 0.5f, 0.5f);
        public static FloatRect FourPlayerBottomLeft { get; set; } = new FloatRect(0, 0.5f, 0.5f, 0.5f);
        public static FloatRect FourPlayerBottomRight { get; set; } = new FloatRect(0.5f, 0.5f, 0.5f, 0.5f);
        public static FloatRect EightPlayerTopRight { get; set; } = new FloatRect(0.75f, 0f, 0.25f, 0.25f);
    }
}
