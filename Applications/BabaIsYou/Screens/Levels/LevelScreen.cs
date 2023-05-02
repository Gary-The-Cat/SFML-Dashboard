using BabaIsYou.DataStructures;
using BabaIsYou.ECS;
using BabaIsYou.ECS.Components;
using BabaIsYou.ECS.Extensions;
using BabaIsYou.ECS.Systems;
using BabaIsYou.Enums.Nodes;
using BabaIsYou.Events;
using BabaIsYou.Resources;
using CSharpFunctionalExtensions;
using DefaultEcs;
using DefaultEcs.System;
using ReactiveUI;
using SFML.Graphics;
using SFML.System;
using Shared.Core;
using Shared.ExtensionMethods;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;

namespace BabaIsYou.Screens.Levels
{
    internal class LevelScreen : Screen
    {
        private Text levelText;
        private Grid grid;
        private GridVisual gridVisual;
        private Vector2f screenSize = new Vector2f(1920, 1080);
        private World world;
        private SequentialSystem<float> frameUpdateSystems;
        private SequentialSystem<StepData> stepUpdateSystems;
        private SequentialSystem<RenderTarget> renderSystems;
        private bool isStepQueued;
        private bool isUndoQueued;
        private bool isWinConditionMet;
        private int currentCell;
        private Stack<List<List<object>>> worldStates = new Stack<List<List<object>>>();
        private List<List<object>> previousStep;

        private LevelScreen(string levelName, LevelData levelData)
        {
            var font = new Font("Resources/Font.ttf");
            levelText = new Text(levelName, font)
            {
                Position = new Vector2f(20, 20),
                FillColor = Color.Black,
            };

            grid = new Grid(levelData.GridConfiguration);
            gridVisual = GridVisual.CreateFromGrid(grid);
            gridVisual.SetCentre(screenSize / 2);

            ConfigureEcs();
            PopulateNodesFromLevelData(levelData);

            isStepQueued = true;

            MessageBus.Current.Listen<QueueStepEvent>()
                .Subscribe(_ => isStepQueued = true);

            MessageBus.Current.Listen<QueueUndoEvent>()
                .Subscribe(_ => isUndoQueued = true);

            MessageBus.Current.Listen<WinEvent>()
                .Subscribe(_ => isWinConditionMet = true);
        }

        public override void OnUpdate(float deltaT)
        {
            frameUpdateSystems.Update(deltaT);

            if (isUndoQueued)
            {
                this.Undo();
            }

            if (isStepQueued)
            {
                isStepQueued = false;
                StepData stepData = GetStepData();

                stepUpdateSystems.Update(stepData);

                if (!isUndoQueued)
                {
                    this.CaptureWorldSnapshot();
                }
                else
                {
                    isUndoQueued = false;
                }
            }

            if (isWinConditionMet)
            {
                FillWinSquares();
            }
        }

        private void CaptureWorldSnapshot()
        {
            if (previousStep != null)
            {
                worldStates.Push(previousStep);
            }

            previousStep = world.Clone();
        }

        private void Undo()
        {
            if (!worldStates.Any())
            {
                return;
            }

            var entities = world.GetEntities().AsSet();
            foreach (var entity in entities.GetEntities())
            {
                entity.Dispose();
            }

            var worldSnapshot = worldStates.Pop();
            world.PopulateFromClone(worldSnapshot);

            if (!worldStates.Any())
            {
                worldStates.Push(worldSnapshot);
            }

            previousStep = worldStates.Peek();
        }

        private void FillWinSquares()
        {
            if (currentCell >= grid.TotalGridCellCount)
            {
                return;
            }


            var entityFilter = world.GetEntities()
                .With<IndexPositionComponent>()
                .With<SpriteComponent>()
                .AsSet();

            var entities = new Dictionary<Vector2u, SpriteComponent>();
            foreach (var entity in entityFilter.GetEntities())
            {
                entities[entity.Get<IndexPositionComponent>().Position] =
                    entity.Get<SpriteComponent>();
            }

            int fillTo = Math.Min(grid.TotalGridCellCount, currentCell + 2);
            for (int i = currentCell; i < fillTo; i++)
            {
                uint x = (uint)(currentCell % (int)grid.GridIndexSize.X);
                uint y = (uint)(currentCell / (int)grid.GridIndexSize.X);
                var newSprite = i % 2 == 0
                    ? NodeTypes.NodeTypeToVisualMap[Adjective.You]().Visual
                    : NodeTypes.NodeTypeToVisualMap[Adjective.Win]().Visual;

                var key = new Vector2u(x, y);
                var intKey = new Vector2i((int)x, (int)y);
                if (!entities.TryGetValue(key, out var sprite))
                {
                    // This is a new entity.
                    var entity = world.CreateEntity();
                    IndexPositionComponent indexPositionComponent = new();
                    SpriteComponent spriteComponent = new();

                    entity.Set(indexPositionComponent);
                    entity.Set(spriteComponent);
                    entity.Set<PositionComponent>(new());

                    indexPositionComponent.QueuedPosition = intKey;
                    indexPositionComponent.MovePosition = intKey;
                    indexPositionComponent.Position = key;

                    spriteComponent.Sprite = newSprite;
                }
                else
                {
                    sprite.Sprite = newSprite;
                }

                currentCell++;
            }

            isStepQueued = true;
        }

        private StepData GetStepData()
        {
            var output = new StepData(gridVisual.GridIndexWidth, gridVisual.GridIndexHeight);
            var indexPositionFilter = world.GetEntities().With<IndexPositionComponent>().AsSet();

            foreach (var entity in indexPositionFilter.GetEntities())
            {
                var positionComponent = entity.Get<IndexPositionComponent>();
                output.Grid.AddEntity(
                    entity,
                    positionComponent,
                    positionComponent.Position.ToVector2i());
            }

            return output;
        }

        public override void OnRender(RenderTarget target)
        {
            target.Clear(new Color(0x12, 0x18, 0x21));

            target.Draw(gridVisual);

            renderSystems.Update(target);
        }

        public static Result<LevelScreen> TryCreateLevelFromData(LevelData levelData)
        {
            return new LevelScreen("level", levelData);
        }

        private void ConfigureEcs()
        {
            world = new World();

            frameUpdateSystems = new SequentialSystem<float>(
                new YouUpdateSystem(world));

            stepUpdateSystems = new SequentialSystem<StepData>(
                new CollisionUpdateSystem(gridVisual, world),
                new TeleSystem(world),
                new IndexPositionUpdaterSystem(gridVisual, world),
                new PhraseParsingSystem(world),
                new SinkUpdateSystem(world),
                new ShutOpenSystem(world),
                new DefeatSystem(world),
                new MeltSystem(world),
                new WinUpdateSystem(world));

            renderSystems = new SequentialSystem<RenderTarget>(
                new SpriteRenderSystem(world));
        }

        private void PopulateNodesFromLevelData(LevelData levelData)
        {
            _ = CameraCreation.CreateStaticCamera(world, new View(screenSize / 2, screenSize));

            foreach (var node in levelData.Nodes)
            {
                var entity = GetBaseEntity();
                entity.IndexPositionComponent.QueuedPosition =
                    new Vector2i((int)node.IndexPosition.X, (int)node.IndexPosition.Y);
                entity.NodeTypeComponent.Node = node.NodeType;

                // Apply non-transient components, currently limited to text
                if (NodeTypes.IsNodeText(node.NodeType))
                {
                    PushComponent pushComponent = new();
                    entity.Entity.Set(pushComponent);
                    pushComponent.IsTransient = false;
                }

                var nodeVisual = NodeTypes.NodeTypeToVisualMap[node.NodeType]();
                entity.SpriteComponent.Sprite = nodeVisual.Visual;
            }
        }

        private (
            DefaultEcs.Entity Entity,
            PositionComponent PositionComponent,
            IndexPositionComponent IndexPositionComponent,
            SpriteComponent SpriteComponent,
            NodeTypeComponent NodeTypeComponent)
            GetBaseEntity()
        {
            var entity = world.CreateEntity();

            PositionComponent positionComponent = new();
            IndexPositionComponent indexPositionComponent = new();
            SpriteComponent spriteComponent = new();
            NodeTypeComponent nodeTypeComponent = new();

            entity.Set(positionComponent);
            entity.Set(indexPositionComponent);
            entity.Set(spriteComponent);
            entity.Set(nodeTypeComponent);

            return (entity, positionComponent, indexPositionComponent, spriteComponent, nodeTypeComponent);
        }
    }
}
