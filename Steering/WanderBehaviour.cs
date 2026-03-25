using System;
using System.Collections.Generic;
using AAI_MonogameAssignment;
using Project.Entities.BaseEntity;
using Project.PathFinding;

namespace Project.Steering
{
    public class WanderBehaviour : SteeringBehaviour
    {
        private readonly NavGraph _navGraph;
        private readonly Random _random = new Random();

        private List<NodeBase> _path;
        private int _pathIndex;
        private const float WaypointReachedDist = 12f;
        private const int WanderRadius = 5;

        public WanderBehaviour(NavGraph navGraph)
        {
            _navGraph = navGraph;
        }

        public override Vector2D Calculate(MovingEntity entity)
        {
            //random target when end of path
            if (_path == null || _pathIndex >= _path.Count)
            {
                var startNode = _navGraph.GetNodeFromWorldPos(entity.Pos.X, entity.Pos.Y);
                if (startNode != null && startNode.IsWalkable)
                {
                    var destination = PickRandomWalkableNode(startNode);
                    if (destination != null)
                    {
                        _navGraph.ResetNodes();
                        _path = AStar.FindPath(startNode, destination);
                        _pathIndex = 1;
                    }
                }
            }

            //waypoints
            if (_path != null && _pathIndex < _path.Count)
            {
                while (_pathIndex < _path.Count &&
                       entity.Pos.DistanceTo(NodeWorldPos(_path[_pathIndex])) < WaypointReachedDist)
                {
                    _pathIndex++;
                }

                if (_pathIndex < _path.Count)
                {
                    var waypoint = NodeWorldPos(_path[_pathIndex]);
                    Vector2D desired = waypoint.Clone().Sub(entity.Pos).Normalize().Multiply(entity.MaxSpeed);
                    return desired.Sub(entity.Velocity);
                }
            }

            //slow down when no path or at destination
            return entity.Velocity.Clone().Multiply(-1);
        }

        public override List<NodeBase> GetCurrentPath() => _path;
        public override int GetCurrentPathIndex() => _pathIndex;

        private NodeBase PickRandomWalkableNode(NodeBase from)
        {
            var candidates = new List<NodeBase>();
            int minRow = Math.Max(0, from.Row - WanderRadius);
            int maxRow = Math.Min(DungeonMap.Rows - 1, from.Row + WanderRadius);
            int minCol = Math.Max(0, from.Col - WanderRadius);
            int maxCol = Math.Min(DungeonMap.Cols - 1, from.Col + WanderRadius);

            for (int row = minRow; row <= maxRow; row++)
            {
                for (int col = minCol; col <= maxCol; col++)
                {
                    var node = _navGraph.Grid[row, col];
                    if (node.IsWalkable && node != from)
                        candidates.Add(node);
                }
            }

            if (candidates.Count == 0) return null;
            return candidates[_random.Next(candidates.Count)];
        }

        private static Vector2D NodeWorldPos(NodeBase node)
        {
            return new Vector2D(
                (node.Col + 0.5f) * DungeonMap.TileSize,
                (node.Row + 0.5f) * DungeonMap.TileSize
            );
        }
    }
}
