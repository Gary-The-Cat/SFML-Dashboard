using DefaultEcs;
using SFML.System;

namespace BabaIsYou.DataStructures
{
    public class StepData
    {
        public StepData(uint gridWidth, uint gridHeight)
        {
            Grid = new EntityGrid(gridWidth, gridHeight);
        }

        public EntityGrid Grid { get; private set; }

        public List<(Entity Entity, Vector2i Movement)> GetMovement()
        {
            var movingEntities = Grid.GetMovingEntities();
            return movingEntities.Select(e => (e.Entity, e.MoveRequest)).ToList();
        }

        public List<(Entity Entity, Vector2i QueuedPosition)> GetQueuedPositionMap()
        {
            var movingEntities = Grid.GetEntities();
            return movingEntities.Select(e => (e.Entity, e.QueuedPosition)).ToList();
        }
    }
}
