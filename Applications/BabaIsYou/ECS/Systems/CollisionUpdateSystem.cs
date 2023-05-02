using BabaIsYou.DataStructures;
using BabaIsYou.ECS.Components;
using DefaultEcs;
using DefaultEcs.System;
using SFML.System;
using Shared.ExtensionMethods;

namespace BabaIsYou.ECS.Systems
{
    internal class CollisionUpdateSystem : ISystem<StepData>
    {
        private readonly World world;

        private readonly GridVisual gridVisual;

        public CollisionUpdateSystem(GridVisual gridVisual, World world)
        {
            this.gridVisual = gridVisual;
            this.world = world;
        }

        private EntitySet positionComponentFilter => world.GetEntities()
            .With<IndexPositionComponent>()
            .AsSet();

        public bool IsEnabled { get; set; }

        public void Update(StepData step)
        {
            // Get the pairs we need to transition
            var moveRequests = step.GetMovement();
            var validMoveRequests = RemoveInvalidMoveRequests(moveRequests, step.Grid);
            var invalidMoveRequests = moveRequests.Except(validMoveRequests);

            // Reset the invalid move requests
            foreach (var moveRequest in invalidMoveRequests)
            {
                var positionComponent = moveRequest.Entity.Get<IndexPositionComponent>();
                positionComponent.QueuedPosition = positionComponent.Position.ToVector2i();
            }

            // Try to move each of the valid move requests
            var queuedPositionMap = step.GetQueuedPositionMap();
            foreach (var moveRequest in validMoveRequests)
            {
                var positionComponent = moveRequest.Entity.Get<IndexPositionComponent>();
                var intPosition = positionComponent.Position.ToVector2i();

                var successful = TryMoveRequest(
                    moveRequest.Entity,
                    intPosition,
                    moveRequest.Movement,
                    queuedPositionMap);

                if (successful)
                {
                    ApplyMoveRequest(
                        positionComponent.MovePosition,
                        moveRequest.Movement,
                        queuedPositionMap);
                }
                else
                {
                    ClearMoveRequest(
                        positionComponent.QueuedPosition,
                        moveRequest.Movement,
                        queuedPositionMap);
                }
            }
        }

        public void Dispose() { }

        private void ClearMoveRequest(
            Vector2i currentPosition,
            Vector2i movement,
            List<(Entity Entity, Vector2i QueuedPosition)> queuedPositionMap)
        {
            var currentCell = currentPosition;
            var entitiesInCell = queuedPositionMap.Where(e => e.QueuedPosition == currentCell);
            while (entitiesInCell.Any())
            {
                foreach (var entity in entitiesInCell)
                {
                    var indexPositionComponent = entity.Entity.Get<IndexPositionComponent>();
                    var positionInt = indexPositionComponent.Position.ToVector2i();
                    indexPositionComponent.MovePosition = positionInt;
                    indexPositionComponent.QueuedPosition = positionInt;
                }

                currentCell += movement;
            }
        }

        private void ApplyMoveRequest(
            Vector2i currentPosition,
            Vector2i movement,
            List<(Entity Entity, Vector2i QueuedPosition)> queuedPositionMap)
        {
            var currentCell = currentPosition;
            var entitiesInCell = queuedPositionMap.Where(e => e.QueuedPosition == currentCell);
            while (entitiesInCell.Any())
            {
                foreach (var entity in entitiesInCell)
                {
                    var indexPositionComponent = entity.Entity.Get<IndexPositionComponent>();
                    indexPositionComponent.QueuedPosition = indexPositionComponent.MovePosition;
                }

                currentCell += movement;
            }
        }

        private bool TryMoveRequest(
            Entity entity,
            Vector2i currentPosition,
            Vector2i movement,
            List<(Entity Entity, Vector2i QueuedPosition)> queuedPositionMap)
        {
            var desiredCellPosition = currentPosition + movement;
            var entitiesInDesiredCell = queuedPositionMap
                .Where(e => e.QueuedPosition == desiredCellPosition && e.Entity != entity).ToList();

            var successful = true;

            // That move would push the entity outside of the game grid.
            if (!gridVisual.Contains(desiredCellPosition))
            {
                successful = false;
            }

            foreach (var gridEntity in entitiesInDesiredCell)
            {
                // There is an entity in this cell, && it needs to be moved.
                var indexPositionComponent = gridEntity.Entity.Get<IndexPositionComponent>();

               
                if (gridEntity.Entity.Has<YouComponent>())
                {
                    continue;
                }

                // TODO - Don't like this being a special case.
                if (gridEntity.Entity.Has<ShutComponent>() && !entity.Has<OpenComponent>())
                {
                    successful = false;
                }

                if (gridEntity.Entity.Has<StopComponent>())
                {
                    successful = false;
                }

                if (gridEntity.Entity.Has<PushComponent>())
                {
                    successful &= TryMoveRequest(
                        gridEntity.Entity,
                        indexPositionComponent.QueuedPosition,
                        movement,
                        queuedPositionMap);

                    continue;
                }
            }

            if (successful)
            {
                // There is an entity in this cell, && it needs to be moved.
                var indexPositionComponent = entity.Get<IndexPositionComponent>();

                var intPosition = indexPositionComponent.Position.ToVector2i();

                indexPositionComponent.MovePosition = intPosition + movement;
            }

            return successful;
        }

        private List<(Entity Entity, Vector2i Movement)> RemoveInvalidMoveRequests(
            List<(Entity Entity, Vector2i Movement)> moveRequests,
            EntityGrid entityGrid)
        {
            var validMoveRequests = new List<(Entity Entity, Vector2i Movement)>();

            foreach (var moveRequest in moveRequests)
            {
                // Get the position of the current component requesting to move
                var positionComponent = moveRequest.Entity.Get<IndexPositionComponent>();

                // That move would push the entity outside of the game grid.
                if (!gridVisual.Contains(positionComponent.QueuedPosition))
                {
                    continue;
                }

                bool safeToMove = true;
                var entitiesAtCell = entityGrid.GetEntitiesAt(positionComponent.QueuedPosition);
                foreach (var gridEntity in entitiesAtCell)
                {
                    // There is an entity in that position with a Stop component
                    if (gridEntity.Entity.Has<StopComponent>())
                    {
                        safeToMove = false;
                        continue;
                    }
                }

                if (!safeToMove)
                {
                    continue;
                }

                validMoveRequests.Add(moveRequest);
            }

            return validMoveRequests;
        }
    }
}
