using BabaIsYou.ECS.Components;
using BabaIsYou.Events;
using DefaultEcs;
using DefaultEcs.System;
using ReactiveUI;
using SFML.System;
using SFML.Window;

namespace BabaIsYou.ECS.Systems
{
    public class YouUpdateSystem : ISystem<float>
    {
        private float keyJustPressedTimer;
        private float undoKeyJustPressedTimer;
        private float minimumKeyPressDelay = 0.15f;
        private World world { get; }

        public YouUpdateSystem(World world)
        {
            this.world = world;
        }

        public bool IsEnabled { get; set; } = true;

        public void Update(float deltaT)
        {
            var youFilter = world.GetEntities().With<YouComponent>().With<IndexPositionComponent>().AsSet();
            var moveRequest = new Vector2i();
            keyJustPressedTimer += deltaT;
            undoKeyJustPressedTimer += deltaT;

            if (Keyboard.IsKeyPressed(Keyboard.Key.Left) || Keyboard.IsKeyPressed(Keyboard.Key.A))
            {
                moveRequest.X -= 1;
            }

            if (Keyboard.IsKeyPressed(Keyboard.Key.Right) || Keyboard.IsKeyPressed(Keyboard.Key.D))
            {
                moveRequest.X += 1;
            }

            if (Keyboard.IsKeyPressed(Keyboard.Key.Up) || Keyboard.IsKeyPressed(Keyboard.Key.W))
            {
                moveRequest.Y -= 1;
            }

            if (Keyboard.IsKeyPressed(Keyboard.Key.Down) || Keyboard.IsKeyPressed(Keyboard.Key.S))
            {
                moveRequest.Y += 1;
            }

            var isKeyPressed = moveRequest != default(Vector2i);
            if (keyJustPressedTimer < minimumKeyPressDelay && isKeyPressed)
            {
                return;
            }

            foreach (var youEntity in youFilter.GetEntities())
            {
                var indexPositionComponent = youEntity.Get<IndexPositionComponent>();

                indexPositionComponent.QueuedPosition.X = 
                    (int)indexPositionComponent.Position.X + moveRequest.X;

                indexPositionComponent.QueuedPosition.Y = 
                    (int)indexPositionComponent.Position.Y + moveRequest.Y;
            }

            if (Keyboard.IsKeyPressed(Keyboard.Key.Space) && keyJustPressedTimer > minimumKeyPressDelay)
            {
                keyJustPressedTimer = 0;
                MessageBus.Current.SendMessage(new QueueStepEvent());
            }

            if (isKeyPressed)
            {
                keyJustPressedTimer = 0;
                MessageBus.Current.SendMessage(new QueueStepEvent());
            }

            if (Keyboard.IsKeyPressed(Keyboard.Key.Z) && undoKeyJustPressedTimer > minimumKeyPressDelay)
            {
                undoKeyJustPressedTimer = 0;
                MessageBus.Current.SendMessage(new QueueUndoEvent());
                MessageBus.Current.SendMessage(new QueueStepEvent());
            }
        }

        public void Dispose() { }
    }
}
