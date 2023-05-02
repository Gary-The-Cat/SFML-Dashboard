using Newtonsoft.Json;
using SFML.Graphics;
using SFML.System;
using Shared.CollisionData;
using Shared.DataStructures;
using Shared.ECS.Components;
using ShootEmUp.Serialization;
using System.Collections.Generic;
using System.Diagnostics;

namespace ShootEmUp.Development
{
    public static class JsonExporter
    {
        public static string GetEnemyJsonStringFormatted(SerializableEnemy enemy)
        {
            var enemyString = JsonConvert.SerializeObject(enemy, Formatting.Indented);

            Debugger.Break();

            return enemyString;
        }

        public static SerializableEnemy DefaultUfoEnemy()
        {
            var ufo = new SerializableEnemy(EnemyType.Ufo);

            var positionComponent = new PositionComponent();

            var movementComponent = new MovementComponent();
            movementComponent.Velocity = new SFML.System.Vector2f(-400, 0);

            var spriteComponent = new SpriteComponent();
            spriteComponent.SpriteFile = "Resources/Ufo.png";
            spriteComponent.Sprite = new SFML.Graphics.Sprite(new SFML.Graphics.Texture(spriteComponent.SpriteFile));

            var collisionComponent = new CollisionComponent();
            collisionComponent.CollidesWithLayers =
                (byte)(CollisionComponent.Player | CollisionComponent.PlayerArtillery);
            collisionComponent.Body = CollisionHelper.GetCollisionCircle(positionComponent.Position, spriteComponent);
            collisionComponent.CollisionLayer = CollisionComponent.Enemy;

            var activationComponent = new ActivationComponent();
            var positionActivationComponent = new PositionActivationComponent();
            var deathComponent = new DeathComponent();
            var valueDropComponent = new ValueDropComponent() { Value = 300 };

            ufo.Components.Add(nameof(PositionComponent), positionComponent);
            ufo.Components.Add(nameof(MovementComponent), movementComponent);
            ufo.Components.Add(nameof(SpriteComponent), spriteComponent);
            ufo.Components.Add(nameof(CollisionComponent), collisionComponent);
            ufo.Components.Add(nameof(ActivationComponent), activationComponent);
            ufo.Components.Add(nameof(PositionActivationComponent), positionActivationComponent);
            ufo.Components.Add(nameof(DeathComponent), deathComponent);
            ufo.Components.Add(nameof(ValueDropComponent), valueDropComponent);

            return ufo;
        }

        public static SerializableEnemy DefaultMovingFiringEnemyEnemy()
        {
            var enemy = new SerializableEnemy(EnemyType.DynamicBoulder);


            var ePositionComponent = new PositionComponent();
            var eSpriteComponent = new SpriteComponent();
            var collisionComponent = new CollisionComponent();
            var pathFollowerComponent = new PathFollowerComponent();
            var projectileComponent = new ProjectileComponent();
            var firingComponent = new ContinuousFiringComponent();

            // Can launch projectiles
            var projectileA = new Projectile(
                "Resources/Bullet.png",
                0.2f,
                new Vector2f(-1, 0),
                new List<Vector2f>() { new Vector2f(-100, 50) });

            projectileA.TexturePath = "Resources/Bullet.png";

            projectileComponent.Projectiles.Add(projectileA);

            ePositionComponent.Position = new Vector2f(700, 1000);
            eSpriteComponent.Sprite = new Sprite(new Texture("Resources/Asteroid.png"));

            collisionComponent.Body = CollisionHelper.GetCollisionCircle(ePositionComponent.Position, eSpriteComponent);
            collisionComponent.CollisionLayer = CollisionComponent.Enemy;
            collisionComponent.CollidesWithLayers = (byte)(CollisionComponent.Player | CollisionComponent.PlayerArtillery);

            pathFollowerComponent.Duration = 4;
            pathFollowerComponent.AttackPattern = 1;

            var activationComponent = new ActivationComponent();
            var positionActivationComponent = new PositionActivationComponent();
            var deathComponent = new DeathComponent();
            var valueDropComponent = new ValueDropComponent() { Value = 300 };

            enemy.Components.Add(nameof(PositionComponent), ePositionComponent);
            enemy.Components.Add(nameof(SpriteComponent), eSpriteComponent);
            enemy.Components.Add(nameof(CollisionComponent), collisionComponent);
            enemy.Components.Add(nameof(ActivationComponent), activationComponent);
            enemy.Components.Add(nameof(PathFollowerComponent), pathFollowerComponent);
            enemy.Components.Add(nameof(ProjectileComponent), projectileComponent);
            enemy.Components.Add(nameof(ContinuousFiringComponent), firingComponent);
            enemy.Components.Add(nameof(PositionActivationComponent), positionActivationComponent);
            enemy.Components.Add(nameof(DeathComponent), deathComponent);
            enemy.Components.Add(nameof(ValueDropComponent), valueDropComponent);

            return enemy;
        }
    }
}
