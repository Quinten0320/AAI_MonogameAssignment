using System.Collections.Generic;

namespace Project.PathFinding
{
    public class NodeBase
    {
        public int Col { get; }
        public int Row { get; }
        public bool IsWalkable { get; set; }
        public NodeBase Connection { get; private set; }
        public float G { get; private set; }
        public float H { get; private set; }
        public float F => G + H;

        public List<NodeBase> Neighbors { get; set; } = new List<NodeBase>();

        public NodeBase(int col, int row, bool isWalkable)
        {
            Col = col;
            Row = row;
            IsWalkable = isWalkable;
        }

        public void SetConnection(NodeBase nodeBase) => Connection = nodeBase;
        public void SetG(float g) => G = g;
        public void SetH(float h) => H = h;

        public void Reset()
        {
            Connection = null;
            G = 0;
            H = 0;
        }
    }
}
