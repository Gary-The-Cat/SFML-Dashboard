using BabaIsYou.ECS.Components;
using DefaultEcs;
using SFML.Graphics;
using SFML.System;
using Shared.ScreenConfig;

namespace BabaIsYou.ECS;

public static class CameraCreation
{
    public static Entity CreateStaticCamera(
        World world,
        View view)
    {
        PositionComponent positionComponent = new();
        LensComponent lensComponent = new();
        var camera = world.CreateEntity();
        camera.Set(positionComponent);
        camera.Set(lensComponent);

        positionComponent.Position = view.Size / 2;
        lensComponent.View = view;
        lensComponent.View.Viewport = ScreenConfiguration.SinglePlayer;

        return camera;
    }
}