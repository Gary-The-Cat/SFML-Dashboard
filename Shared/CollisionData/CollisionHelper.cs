using SFML.System;
using Shared.CollisionData.CollisionShapes; 

namespace Shared.CollisionData
{
    public static class CollisionHelper
    {
        ////public static Rectangle GetCollisionRectangle(Vector2f position, SpriteComponent spriteComponent)
        ////{
        ////    var bounds = spriteComponent.Sprite.GetLocalBounds();

        ////    return new Rectangle(
        ////        position.X + bounds.Width / 2,
        ////        position.Y + bounds.Height / 2,
        ////        bounds.Width / 2,
        ////        bounds.Height / 2);
        ////}

        ////public static Circle GetCollisionCircle(Vector2f position, SpriteComponent spriteComponent)
        ////{
        ////    var bounds = spriteComponent.Sprite.GetLocalBounds();

        ////    var offset = 0.8f;

        ////    return new Circle(
        ////        position.X + bounds.Width / 2,
        ////        position.Y + bounds.Height / 2,
        ////        bounds.Width / 2 * offset);
        ////}
    }
}
