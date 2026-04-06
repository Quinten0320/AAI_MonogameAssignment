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

            if (_path == null || _pathIndex >= _path.Count || targetNode != _lastTargetNode || startNode != _lastStartNode)
            {
                if (startNode != null && targetNode != null && startNode.IsWalkable && targetNode.IsWalkable)
                {
                    _navGraph.ResetNodes();
                    _path = AStar.FindPath(startNode, targetNode);
                    _pathIndex = 1;
                    _lastTargetNode = targetNode;
                    _lastStartNode = startNode;
                }
            }

            if (_path != null && _pathIndex < _path.Count)
            {
                while (_pathIndex < _path.Count &&
                       entity.Pos.DistanceTo(_path[_pathIndex].WorldPos) < WaypointReachedDist)
                {
                    _pathIndex++;
                }

                if (_pathIndex < _path.Count)
                    return SeekToward(entity, _path[_pathIndex].WorldPos);
            }

            return SeekToward(entity, _target);
        }

        public override List<NodeBase> GetCurrentPath() => _path;
        public override int GetCurrentPathIndex() => _pathIndex;
    }
}
