using Leopotam.Ecs;
using SFML.Graphics;
using SFML.System;
using Shared.CollisionData;
using Shared.DataStructures;
using Shared.ECS.Components;
using Shared.Maths.Spline;
using System.Collections.Generic;

namespace Shared.ECS.Helpers
{
    // :NOTE: This class is for testing purposes only and should not be used to 
    // generate enemies in a a playable version of the game.
    public static class EnemySpawning
    {
        public static EcsEntity SpawnContinuousFiringEnemy(EcsWorld world)
        {
            var enemy = world.CreateEntityWith<
                PositionComponent,
                SpriteComponent,
                ContinuousFiringComponent,
                ProjectileComponent>(
                out var ePositionComponent,
                out var eSpriteComponent,
                out _,
                out var projectileComponent);

            ePositionComponent.Position = new Vector2f(700, 1000);
            eSpriteComponent.Sprite = new Sprite(new Texture("Resources/Asteroid.png"));

            // Can launch projectiles
            var projectileA = new Projectile(
                "Resources/Bullet.png",
                0.2f,
                new Vector2f(-1, 0),
                new List<Vector2f>() { new Vector2f(-100, 50) });

            projectileComponent.Projectiles.Add(projectileA);

            return enemy;
        }

        public static EcsEntity SpawnPathFollowerEnemy(EcsWorld world)
        {
            var enemy = world.CreateEntityWith<
                PositionComponent,
                SpriteComponent,
                CollisionComponent,
                PathFollowerComponent>(
                out var ePositionComponent,
                out var eSpriteComponent,
                out var collisionComponent,
                out var pathFollowerComponent);

            ePositionComponent.Position = new Vector2f(700, 1000);
            eSpriteComponent.Sprite = new Sprite(new Texture("Resources/Asteroid.png"));

            collisionComponent.Body = CollisionHelper.GetCollisionCircle(ePositionComponent.Position, eSpriteComponent);
            collisionComponent.CollisionLayer = CollisionComponent.Enemy;
            collisionComponent.CollidesWithLayers = (byte)(CollisionComponent.Player | CollisionComponent.PlayerArtillery);

            pathFollowerComponent.Duration = 4;
            pathFollowerComponent.AttackPattern = 1;

            return enemy;
        }

        public static EcsEntity SpawnPathFollowerFiringEnemy(EcsWorld world)
        {
            var enemy = world.CreateEntityWith<
                PositionComponent,
                SpriteComponent,
                CollisionComponent,
                PathFollowerComponent,
                ContinuousFiringComponent,
                ProjectileComponent>(
                out var ePositionComponent,
                out var eSpriteComponent,
                out var collisionComponent,
                out var pathFollowerComponent,
                out _,
                out var projectileComponent);

            // Can launch projectiles
            var projectileA = new Projectile(
                "Resources/Bullet.png",
                0.2f,
                new Vector2f(-1, 0),
                new List<Vector2f>() { new Vector2f(-100, 50) });

            projectileComponent.Projectiles.Add(projectileA);

            ePositionComponent.Position = new Vector2f(700, 1000);
            eSpriteComponent.Sprite = new Sprite(new Texture("Resources/Asteroid.png"));

            collisionComponent.Body = CollisionHelper.GetCollisionCircle(ePositionComponent.Position, eSpriteComponent);
            collisionComponent.CollisionLayer = CollisionComponent.Enemy;
            collisionComponent.CollidesWithLayers = (byte)(CollisionComponent.Player | CollisionComponent.PlayerArtillery);

            pathFollowerComponent.Duration = 4;
            pathFollowerComponent.AttackPattern = 1;

            return enemy;
        }

    }
}
