using BabaIsYou.DataStructures.DataGrid;
using BabaIsYou.ECS.Components;
using DefaultEcs;
using System.Collections;

namespace BabaIsYou.DataStructures
{
    public class GridCell : IEnumerable<GridNode>
    {
        public GridCell()
        {
            nodes = new List<GridNode>();
        }

        private List<GridNode> nodes;

        public void AddNode(
            Entity entity,
            IndexPositionComponent positionComponent)
        {
            nodes.Add(new GridNode(entity, positionComponent));
        }

        public IEnumerator<GridNode> GetEnumerator()
        {
            return nodes.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerable<GridNode> Nodes => nodes;
    }
}
