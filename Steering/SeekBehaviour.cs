using System.Collections.Generic;
using AAI_MonogameAssignment;
using Project.Entities.BaseEntity;
using Project.PathFinding;

namespace Project.Steering
{
    public class SeekBehaviour : SteeringBehaviour
    {
        private readonly Vector2D _target;
        private readonly NavGraph _navGraph;

        private List<NodeBase> _path;
        private int _pathIndex;
        private NodeBase _lastTargetNode;
        private NodeBase _lastStartNode;
        private const float WaypointReachedDist = 12f;

        public SeekBehaviour(Vector2D target, NavGraph navGraph)
        {
            _target = target;
            _navGraph = navGraph;
        }

        public override Vector2D Calculate(MovingEntity entity)
        {
            var targetNode = _navGraph.GetNodeFromWorldPos(_target.X, _target.Y);
            var startNode = _navGraph.GetNodeFromWorldPos(entity.Pos.X, entity.Pos.Y);

            //at end of path, target moved tile, or skeleton enters new tile
            if (_path == null || _pathIndex >= _path.Count || targetNode != _lastTargetNode || startNode != _lastStartNode)
            {
                if (startNode != null && targetNode != null && startNode.IsWalkable && targetNode.IsWalkable)
                {
                    _navGraph.ResetNodes();
                    _path = AStar.FindPath(startNode, targetNode);
                    _pathIndex = 1; //skip start node
                    _lastTargetNode = targetNode;
                    _lastStartNode = startNode;
                }
            }

            if (_path != null && _pathIndex < _path.Count)
            {
                while (_pathIndex < _path.Count &&
                       entity.Pos.DistanceTo(NodeWorldPos(_path[_pathIndex])) < WaypointReachedDist)
                {
                    _pathIndex++;
                }

                if (_pathIndex < _path.Count)
                    return SeekToward(entity, NodeWorldPos(_path[_pathIndex]));
            }

            return SeekToward(entity, _target);
        }

        public override List<NodeBase> GetCurrentPath() => _path;
        public override int GetCurrentPathIndex() => _pathIndex;

        private static Vector2D SeekToward(MovingEntity entity, Vector2D target)
        {
            Vector2D desired = target.Clone().Sub(entity.Pos).Normalize().Multiply(entity.MaxSpeed);
            return desired.Sub(entity.Velocity);
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
