using BabaIsYou.ECS.Components;
using CSharpFunctionalExtensions;
using DefaultEcs;
using DefaultEcs.System;
using SFML.Graphics;

namespace BabaIsYou.ECS.Systems;

public class SpriteRenderSystem : ISystem<RenderTarget>
{
    private World world;

    public SpriteRenderSystem(World world)
    {
        this.world = world;
    }

    private EntitySet positionComponentFilter => world.GetEntities()
        .With<PositionComponent>()
        .AsSet();

    private EntitySet spriteComponentFilter => world.GetEntities()
        .With<SpriteComponent>()
        .With<PositionComponent>()
        .AsSet();

    ////private EntitySet linearAnimatedSpriteComponentFilter => world.GetEntities()
    ////    .With<LinearAnimatedSpriteComponent>()
    ////    .With<PositionComponent>()
    ////    .AsSet();

    ////private EntitySet randomAnimatedSpriteComponentFilter => world.GetEntities()
    ////    .With<RandomAnimatedSpriteComponent>()
    ////    .With<PositionComponent>()
    ////    .AsSet();

    private EntitySet lensComponentFilter => world.GetEntities()
        .With<LensComponent>()
        .AsSet();

    public bool IsEnabled { get; set; }

    public void Dispose()
    {
        throw new NotImplementedException();
    }

    // :TODO: Easy refactor here.
    public void Update(RenderTarget target)
    {
        foreach (var entity in positionComponentFilter.GetEntities())
        {

        }

        foreach (var camera in lensComponentFilter.GetEntities())
        {
            var lens = camera.Get<LensComponent>();
            target.SetView(lens.View);

            foreach (var entity in spriteComponentFilter.GetEntities())
            {
                var spriteComponent = entity.Get<SpriteComponent>();
                var positionComponent = entity.Get<PositionComponent>();

                spriteComponent.Sprite.Position = positionComponent.GetPosition();

                target.Draw(spriteComponent.Sprite);
            }
        }

        ////foreach (var camera in lensComponentFilter.GetEntities())
        ////{
        ////    var lens = camera.Get<LensComponent>();
        ////    target.SetView(lens.View);

        ////    foreach (var entity in linearAnimatedSpriteComponentFilter.GetEntities())
        ////    {
        ////        var spriteComponent = entity.Get<SpriteComponent>();
        ////        var positionComponent = entity.Get<PositionComponent>();

        ////        spriteComponent.Sprite.Position = positionComponent.GetPosition();

        ////        target.Draw(spriteComponent.Sprite);
        ////    }
        ////}


        ////foreach (var camera in lensComponentFilter.GetEntities())
        ////{
        ////    var lens = camera.Get<LensComponent>();
        ////    target.SetView(lens.View);

        ////    foreach (var entity in randomAnimatedSpriteComponentFilter.GetEntities())
        ////    {
        ////        var spriteComponent = entity.Get<SpriteComponent>();
        ////        var positionComponent = entity.Get<PositionComponent>();

        ////        spriteComponent.Sprite.Position = positionComponent.GetPosition();

        ////        target.Draw(spriteComponent.Sprite);
        ////    }
        ////}
    }
}