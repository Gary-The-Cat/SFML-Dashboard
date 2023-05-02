using BabaIsYou.DataStructures.DataGrid;
using BabaIsYou.ECS.Components;
using DefaultEcs;
using SFML.System;

namespace BabaIsYou.DataStructures
{
    public class EntityGrid
    {
        private GridCell[,] entities;

        public EntityGrid(uint gridWidth, uint gridHeight)
        {
            entities = new GridCell[gridWidth, gridHeight];
            for (int x = 0; x < gridWidth; x++)
            {
                for (int y = 0; y < gridHeight; y++)
                {
                    entities[x, y] = new GridCell();
                }
            }
        }

        public void AddEntity(
            Entity entity,
            IndexPositionComponent positionComponent,
            Vector2i position)
        {
            entities[position.X, position.Y].AddNode(entity, positionComponent);
        }

        public IEnumerable<GridNode> GetEntitiesAt(Vector2i position)
        {
            return entities[position.X, position.Y].Nodes;
        }
        public IEnumerable<GridNode> GetEntities()
        {
            var output = new List<GridNode>();
            for (int x = 0; x < entities.GetLength(0); x++)
            {
                for (int y = 0; y < entities.GetLength(1); y++)
                {
                    foreach (var node in entities[x, y])
                    {
                        output.Add(node);
                    }
                }
            }

            return output;
        }

        public IEnumerable<GridNode> GetMovingEntities()
        {
            var output = new List<GridNode>();
            for (int x = 0; x < entities.GetLength(0); x++)
            {
                for (int y = 0; y < entities.GetLength(1); y++)
                {
                    foreach (var node in entities[x, y])
                    {
                        if (node.HasMoveRequest)
                        {
                            output.Add(node);
                        }
                    }
                }
            }

            return output;
        }
    }
}
