namespace Project.PathFinding
{
    public class NavGraph
    {
        public NodeBase[,] Grid { get; private set; }

        public NavGraph(DungeonMap map)
        {
            Grid = new NodeBase[DungeonMap.Rows, DungeonMap.Cols];

            for (int row = 0; row < DungeonMap.Rows; row++)
                for (int col = 0; col < DungeonMap.Cols; col++)
                    Grid[row, col] = new NodeBase(col, row, !map.IsWall(col, row));

            for (int row = 0; row < DungeonMap.Rows; row++)
            {
                for (int col = 0; col < DungeonMap.Cols; col++)
                {
                    var node = Grid[row, col];

                    // Cardinal neighbors
                    bool up = row > 0;
                    bool down = row < DungeonMap.Rows - 1;
                    bool left = col > 0;
                    bool right = col < DungeonMap.Cols - 1;

                    if (up) node.Neighbors.Add(Grid[row - 1, col]);
                    if (down) node.Neighbors.Add(Grid[row + 1, col]);
                    if (left) node.Neighbors.Add(Grid[row, col - 1]);
                    if (right) node.Neighbors.Add(Grid[row, col + 1]);

                    // Diagonal neighbors (only if both adjacent cardinal tiles are walkable to prevent corner-cutting)
                    if (up && left && Grid[row - 1, col].IsWalkable && Grid[row, col - 1].IsWalkable)
                        node.Neighbors.Add(Grid[row - 1, col - 1]);
                    if (up && right && Grid[row - 1, col].IsWalkable && Grid[row, col + 1].IsWalkable)
                        node.Neighbors.Add(Grid[row - 1, col + 1]);
                    if (down && left && Grid[row + 1, col].IsWalkable && Grid[row, col - 1].IsWalkable)
                        node.Neighbors.Add(Grid[row + 1, col - 1]);
                    if (down && right && Grid[row + 1, col].IsWalkable && Grid[row, col + 1].IsWalkable)
                        node.Neighbors.Add(Grid[row + 1, col + 1]);
                }
            }
        }

        public NodeBase GetNode(int col, int row)
        {
            if (col < 0 || col >= DungeonMap.Cols || row < 0 || row >= DungeonMap.Rows) return null;
            return Grid[row, col];
        }

        public NodeBase GetNodeFromWorldPos(float x, float y)
        {
            int col = (int)(x / DungeonMap.TileSize);
            int row = (int)(y / DungeonMap.TileSize);
            return GetNode(col, row);
        }

        public void ResetNodes()
        {
            for (int row = 0; row < DungeonMap.Rows; row++)
                for (int col = 0; col < DungeonMap.Cols; col++)
                    Grid[row, col].Reset();
        }
    }
}
