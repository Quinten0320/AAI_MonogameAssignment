using System;
using System.Collections.Generic;
using AAI_MonogameAssignment;
using Project.Entities.BaseEntity;
using Project.PathFinding;

namespace Project.Steering
{
    public class FleeBehaviour : SteeringBehaviour
    {
        private readonly Vector2D _target;
        private readonly NavGraph _navGraph;

        private List<NodeBase> _path;
        private int _pathIndex;
        private NodeBase _lastEntityNode;
        private const float WaypointReachedDist = 12f;
        private const int FleeDistance = 5;

        public FleeBehaviour(Vector2D target, NavGraph navGraph)
        {
            _target = target;
            _navGraph = navGraph;
        }

        public override Vector2D Calculate(MovingEntity entity)
        {
            var entityNode = _navGraph.GetNodeFromWorldPos(entity.Pos.X, entity.Pos.Y);

            if (_path == null || _pathIndex >= _path.Count || entityNode != _lastEntityNode)
            {
                _lastEntityNode = entityNode;
                var fleeTarget = FindFleeDestination(entityNode);

                if (fleeTarget != null && entityNode != null && entityNode.IsWalkable)
                {
                    _navGraph.ResetNodes();
                    _path = AStar.FindPath(entityNode, fleeTarget);
                    _pathIndex = 1;
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

            Vector2D desired = entity.Pos.Clone().Sub(_target).Normalize().Multiply(entity.MaxSpeed);
            return desired.Sub(entity.Velocity);
        }

        public override List<NodeBase> GetCurrentPath() => _path;
        public override int GetCurrentPathIndex() => _pathIndex;

        private NodeBase FindFleeDestination(NodeBase entityNode)
        {
            if (entityNode == null) return null;

            var threatNode = _navGraph.GetNodeFromWorldPos(_target.X, _target.Y);
            if (threatNode == null) return null;

            float dx = entityNode.Col - threatNode.Col;
            float dy = entityNode.Row - threatNode.Row;
            float len = MathF.Sqrt(dx * dx + dy * dy);
            if (len < 0.001f) { dx = 1; dy = 0; len = 1; }
            dx /= len;
            dy /= len;

            for (int dist = FleeDistance; dist >= 2; dist--)
            {
                int col = (int)Math.Round(entityNode.Col + dx * dist);
                int row = (int)Math.Round(entityNode.Row + dy * dist);
                var node = _navGraph.GetNode(col, row);
                if (node != null && node.IsWalkable)
                    return node;
            }

            NodeBase best = null;
            float bestDist = 0;
            for (int row = 0; row < DungeonMap.Rows; row++)
            {
                for (int col = 0; col < DungeonMap.Cols; col++)
                {
                    var node = _navGraph.Grid[row, col];
                    if (!node.IsWalkable) continue;
                    float d = Math.Abs(node.Col - threatNode.Col) + Math.Abs(node.Row - threatNode.Row);
                    if (d > bestDist)
                    {
                        bestDist = d;
                        best = node;
                    }
                }
            }
            return best;
        }

    }
}
