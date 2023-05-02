using Leopotam.Ecs;
using Newtonsoft.Json.Linq;
using Shared.CollisionData;
using Shared.ECS.Components;
using System.Collections.Generic;

namespace ShootEmUp.Serialization
{
    public class LevelLoader
    {
        public static List<EcsEntity> LoadLevel(
            EcsWorld world,
            GameLevel level,
            Dictionary<string, SerializableEnemy> baseEnemies)
        {
            var entities = new List<EcsEntity>();

            // Loop over each of enemies in the level
            foreach (var levelEnemy in level.Enemies)
            {
                entities.Add(LoadDefaultEntity(world, baseEnemies, levelEnemy));
            }

            return entities;
        }

        private static EcsEntity LoadDefaultEntity(
            EcsWorld world,
            Dictionary<string, SerializableEnemy> baseEnemies,
            LevelEnemy levelEnemy)
        {
            var type = levelEnemy.EnemyType;

            var ecsEntity = world.CreateEntity();

            // Create all of the components for this entity and save them to the EcsEntity
            var enemy = baseEnemies[levelEnemy.EnemyType];
            foreach (var component in enemy.Components)
            {
                switch (component.Key)
                {
                    case nameof(PositionComponent):
                        ProcessComponent<PositionComponent>(world, ecsEntity, enemy, levelEnemy.DefaultOverrides);
                        break;
                    case nameof(MovementComponent):
                        ProcessComponent<MovementComponent>(world, ecsEntity, enemy, levelEnemy.DefaultOverrides);
                        break;
                    case nameof(ActivationComponent):
                        ProcessComponent<ActivationComponent>(world, ecsEntity, enemy, levelEnemy.DefaultOverrides);
                        break;
                    case nameof(PositionActivationComponent):
                        ProcessComponent<PositionActivationComponent>(world, ecsEntity, enemy, levelEnemy.DefaultOverrides);
                        break;
                    case nameof(SpriteComponent):
                        ProcessComponent<SpriteComponent>(world, ecsEntity, enemy, levelEnemy.DefaultOverrides);
                        break;
                    case nameof(ValueDropComponent):
                        ProcessComponent<ValueDropComponent>(world, ecsEntity, enemy, levelEnemy.DefaultOverrides);
                        break;
                    case nameof(ProjectileComponent):
                        ProcessComponent<ProjectileComponent>(world, ecsEntity, enemy, levelEnemy.DefaultOverrides);
                        break;
                    case nameof(CollisionComponent):
                        ProcessComponent<CollisionComponent>(world, ecsEntity, enemy, levelEnemy.DefaultOverrides);
                        break;
                    case nameof(PathFollowerComponent):
                        ProcessComponent<PathFollowerComponent>(world, ecsEntity, enemy, levelEnemy.DefaultOverrides);
                        break;
                    case nameof(ContinuousFiringComponent):
                        ProcessComponent<ContinuousFiringComponent>(world, ecsEntity, enemy, levelEnemy.DefaultOverrides);
                        break;
                    case nameof(DeathComponent):
                        ProcessComponent<DeathComponent>(world, ecsEntity, enemy, levelEnemy.DefaultOverrides);
                        break;
                }
            }

            var cc = world.GetComponent<CollisionComponent>(ecsEntity);
            var sc = world.GetComponent<SpriteComponent>(ecsEntity);
            var pc = world.GetComponent<PositionComponent>(ecsEntity);

            if (cc != null && sc != null)
            {
                cc.Body = CollisionHelper.GetCollisionCircle(pc.Position, sc);
            }

            return ecsEntity;
        }

        public static void ProcessComponent<T>(
            EcsWorld world,
            EcsEntity entity,
            SerializableEnemy fileEnemy,
            Dictionary<string, object> overrides)
            where T : class, new()
        {
            // Create the position component in the world.
            var component = world.AddComponent<T>(entity);

            // Map the data stored in the default entity to the component.
            fileEnemy.MapComponent(component);

            // Apply all default value overrides
            var type = component.GetType();

            foreach (var newOverride in overrides)
            {
                var targetProperty = type.GetProperty(newOverride.Key);

                if (targetProperty != null)
                {
                    targetProperty.SetValue(
                        component,
                        ((JObject)newOverride.Value).ToObject(targetProperty.PropertyType));
                }
            }
        }
    }
}
