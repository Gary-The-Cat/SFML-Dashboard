using Leopotam.Ecs;
using SFML.Graphics;
using SFML.System;
using Shared.CollisionData;
using Shared.DataStructures;
using Shared.ECS.Components;
using Shared.ECS.Helpers;
using Shared.ECS.LeoEcs;
using Shared.ECS.Systems;
using Shared.ScreenConfig;
using ShootEmUp.Development;
using ShootEmUp.Helpers;
using ShootEmUp.Serialization;
using System.Collections.Generic;
using System.Linq;

namespace ShootEmUp.Screens
{
    public class GameRunner
    {
        public bool IsInitialized { get; set; }

        private EcsWorld world;
        private EcsSystems<float> updateSystems;
        private EcsRenderSystems renderSystems;
        private View defaultView;

        private EcsEntity player;
        private EcsEntity mainCamera;
        private List<EcsEntity> levelEnemies;
        private List<GameLevel> allLevels;

        public void Initialize(View view)
        {
            this.defaultView = new View(view);
            this.world = new EcsWorld();
            this.updateSystems = new EcsSystems<float>(this.world);
            this.renderSystems = new EcsRenderSystems(this.world);

            this.PopulateWorld();

            this.IsInitialized = true;
        }

        public void Reset()
        {

        }

        public void Restart()
        {

        }

        public void Pause()
        {

        }

        public void OnUpdate(float deltaT)
        {
            this.updateSystems.Run(deltaT);

            var isResetRequested = CheckResetRequested();
            if (isResetRequested)
            {
                ProcessRestartLevel();
            }

            var isGameOverRequested = CheckGameOverRequested();
            if (isGameOverRequested)
            {
                ProcessGameOver();
            }
        }

        public void OnRender(RenderTarget renderTarget)
        {
            this.renderSystems.Run(renderTarget);
        }

        private void SpawnSystems()
        {
            this.updateSystems.Add(new UserMovementSystem());
            this.updateSystems.Add(new MovementSystem());
            this.updateSystems.Add(new CollisionMovementSystem());
            this.updateSystems.Add(new AttachedSystem());
            this.updateSystems.Add(new PathFollowerUpdateSystem());
            this.updateSystems.Add(new UserProjectileSystem());
            this.updateSystems.Add(new EnemyFiringSystem());
            this.updateSystems.Add(new ProjectileSystem());
            this.updateSystems.Add(new LinearSpriteUpdaterSystem());
            this.updateSystems.Add(new RandomSpriteUpdaterSystem());
            this.updateSystems.Add(new CameraSystem());
            this.updateSystems.Add(new CameraScreenBoundSystem());
            this.updateSystems.Add(new CameraActivationBoundUpdaterSystem());
            this.updateSystems.Add(new BoundUpdaterSystem());
            this.updateSystems.Add(new BoundEnforcerSystem());
            this.updateSystems.Add(new BoundActivationSystem());
            this.updateSystems.Add(new BoundCullingSystem());
            this.updateSystems.Add(new LifetimeSystem());

            // Process all collisions
            this.updateSystems.Add(new CollisionSystem());
            this.updateSystems.Add(new CollisionResolutionSystem());

            // Post collision updates (dependent on collision system happening first)
            this.updateSystems.Add(new ScoreUpdateSystem());
            this.updateSystems.Add(new LifeRemovalSystem());
            this.updateSystems.Add(new ResetLevelSystem());
            this.updateSystems.Add(new EndGameSystem());
            this.updateSystems.Add(new DeathSystem());

            this.renderSystems.Add(new SpriteRenderSystem());

            if (true)
            {
                this.renderSystems.Add(new DebugRenderSystem());
            }

            this.renderSystems.Add(new PlayerHudRenderSystem(new View(defaultView)));
        }

        private void PopulateWorld()
        {
            _ = this.world.CreateEntityWith<
               PositionComponent,
               SpriteComponent>(
               out var positionComponent,
               out var spriteComponent);

            var texture = new Texture("Resources/Background.png");
            var sprite = new Sprite(texture);
            spriteComponent.Sprite = sprite;
            positionComponent.Position = new Vector2f(defaultView.Size.X, defaultView.Size.Y) / -2;

            this.player = this.SpawnPlayer();

            this.mainCamera = CameraCreation.CreateSideScrollCamera(world, defaultView);

            CameraCreation.CreatePlayerFollowCamera(player, world, defaultView);

            this.SpawnSystems();

            var allEnemies = WorldLoader.LoadEnemyFiles("Resources/EnemyFiles/");

            this.allLevels = WorldLoader.LoadLevelFiles("Resources/LevelFiles/");

            var level = this.allLevels.First();

            levelEnemies = LevelLoader.LoadLevel(world, level, allEnemies);

            ////EnemySpawning.SpawnPathFollowerFiringEnemy(world);

            this.Initialize();
        }

        private void Initialize()
        {
            this.updateSystems.Initialize();
            this.renderSystems.Initialize();
        }

        private EcsEntity SpawnPlayer()
        {
            var player = this.world.CreateEntity();

            // Position
            var positionComponent = this.world.AddComponent<PositionComponent>(player);
            positionComponent.Position = new Vector2f(this.defaultView.Size.X, this.defaultView.Size.Y + 1080) / 2;


            // Sprite
            var spriteComponent = this.world.AddComponent<LinearAnimatedSpriteComponent>(player);

            var texture = new Texture("Resources/Ship.png");
            var sprite = new Sprite(texture);
            spriteComponent.Sprite = sprite;
            spriteComponent.FrameTime = 0.05f;
            spriteComponent.AddFrames(new List<IntRect>()
            {
                new IntRect(0, 0, 256, 256)
            });

            // Collision
            var collisionComponent = this.world.AddComponent<CollisionComponent>(player);
            collisionComponent.Body = CollisionHelper.GetCollisionCircle(positionComponent.Position, spriteComponent);
            collisionComponent.CollisionLayer = CollisionComponent.Player;
            collisionComponent.CollidesWithLayers = (byte)(CollisionComponent.Enemy | CollisionComponent.EnemyArtillery);

            // Can launch projectiles
            var projectileComponent = this.world.AddComponent<ProjectileComponent>(player);
            var projectileA = new Projectile(
                "Resources/Bullet.png",
                0.2f,
                new Vector2f(1, 0),
                new List<Vector2f>() { new Vector2f(228, 96) });

            var projectileB = new Projectile(
                "Resources/BulletB.png",
                0.1f,
                new Vector2f(1, 0),
                new List<Vector2f>() { new Vector2f(180, 10), new Vector2f(180, 120) });

            projectileComponent.Projectiles.Add(projectileA);
            projectileComponent.Projectiles.Add(projectileB);

            var positionBoundComponent = this.world.AddComponent<PositionBoundComponent>(player);
            positionBoundComponent.LowerLeftOffset = new Vector2f(-32, -32);
            positionBoundComponent.UpperRightOffset = new Vector2f(-290, -280);

            var livesComponent = this.world.AddComponent<LivesComponent>(player);
            livesComponent.Lives = 1;

            _ = this.world.AddComponent<DeathComponent>(player);
            _ = this.world.AddComponent<LevelRestartComponent>(player);
            _ = this.world.AddComponent<EndGameComponent>(player);
            _ = this.world.AddComponent<MovementComponent>(player);
            _ = this.world.AddComponent<UserInputComponent>(player);
            _ = this.world.AddComponent<ScoreComponent>(player);
            _ = this.world.AddComponent<PlayerComponent>(player);

            return player;
        }

        public void ResetGame()
        {
            foreach (var levelEnemy in this.levelEnemies)
            {
                this.world.TryRemoveEntity(levelEnemy);
            }

            var playerPositionComponent =this.world.GetComponent<PositionComponent>(player);
            playerPositionComponent.Position = new Vector2f(ScreenConfiguration.StaticWidth, ScreenConfiguration.StaticHeight + 1080) / 2;

            var playerLivesComponent =this.world.GetComponent<LivesComponent>(player);
            playerLivesComponent.Lives = 5;

            var playerScoreComponent =this.world.GetComponent<ScoreComponent>(player);
            playerScoreComponent.Score = 0;

            var playerProjectileComponent =this.world.GetComponent<ProjectileComponent>(player);
            while (playerProjectileComponent.ProjectileLevel > 0)
            {
                playerProjectileComponent.LevelDown();
            }

            var cameraPositionComponent =this.world.GetComponent<PositionComponent>(mainCamera);
            cameraPositionComponent.Position = new Vector2f(ScreenConfiguration.StaticWidth, ScreenConfiguration.StaticHeight + 1080) / 2;

            var level = this.allLevels.First();

            var allEnemies = WorldLoader.LoadEnemyFiles("Resources/EnemyFiles/");

            levelEnemies = LevelLoader.LoadLevel(world, level, allEnemies);
        }


        private void ProcessGameOver()
        {
            this.ResetGame();

            var playerEndGameComponent = this.world.GetComponent<EndGameComponent>(in player);
            playerEndGameComponent.IsGameOverRequested = false;
        }

        private void ProcessRestartLevel()
        {
            // Get the players position component
            var playerPositionComponent = this.world.GetComponent<PositionComponent>(in player);
            var playerRestartComponent = this.world.GetComponent<LevelRestartComponent>(in player);

            // Get the current screen locations to determine where to place the player
            var cameraLensComponent = this.world.GetComponent<LensComponent>(in mainCamera);
            var cameraLeft = (cameraLensComponent.View.Center - cameraLensComponent.View.Size / 2).X;

            // Move the player to the left of the screen
            playerPositionComponent.Position = new Vector2f(cameraLeft + 100, (ScreenConfiguration.StaticHeight + 1080) / 2);

            playerRestartComponent.IsRestartRequested = false;
        }

        private bool CheckResetRequested()
        {
            var entities = this.world.GetFilter<EcsFilter<LevelRestartComponent>>();

            foreach (var entity in entities)
            {
                if (entities.Components1[entity].IsRestartRequested)
                {
                    return true;
                }
            }

            return false;
        }

        private bool CheckGameOverRequested()
        {
            var entities = this.world.GetFilter<EcsFilter<EndGameComponent>>();

            foreach (var entity in entities)
            {
                if (entities.Components1[entity].IsGameOverRequested)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
