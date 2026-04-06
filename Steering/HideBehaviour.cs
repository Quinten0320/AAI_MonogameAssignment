using System;
using System.Collections.Generic;
using AAI_MonogameAssignment;
using Project.Entities.BaseEntity;
using Project.PathFinding;

namespace Project.Steering
{
    public class HideBehaviour : SteeringBehaviour
    {
        private readonly Vector2D _playerPos;
        private readonly NavGraph _navGraph;
        private readonly DungeonMap _map;

        private List<NodeBase> _path;
        private int _pathIndex;
        private NodeBase _lastEntityNode;
        private NodeBase _lastPlayerNode;
        private const float WaypointReachedDist = 12f;

        public HideBehaviour(Vector2D playerPos, NavGraph navGraph, DungeonMap map)
        {
            _playerPos = playerPos;
            _navGraph = navGraph;
            _map = map;
        }

        public override Vector2D Calculate(MovingEntity entity)
        {
            var entityNode = _navGraph.GetNodeFromWorldPos(entity.Pos.X, entity.Pos.Y);
            var playerNode = _navGraph.GetNodeFromWorldPos(_playerPos.X, _playerPos.Y);

            if (_path == null || _pathIndex >= _path.Count ||
                entityNode != _lastEntityNode || playerNode != _lastPlayerNode)
            {
                _lastEntityNode = entityNode;
                _lastPlayerNode = playerNode;
                var hideSpot = FindHideSpot(entityNode, playerNode);

                if (hideSpot != null && entityNode != null && entityNode.IsWalkable)
                {
                    _navGraph.ResetNodes();
                    _path = AStar.FindPath(entityNode, hideSpot);
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

            Vector2D desired = entity.Pos.Clone().Sub(_playerPos).Normalize().Multiply(entity.MaxSpeed);
            return desired.Sub(entity.Velocity);
        }

        public override List<NodeBase> GetCurrentPath() => _path;
        public override int GetCurrentPathIndex() => _pathIndex;

        private NodeBase FindHideSpot(NodeBase entityNode, NodeBase playerNode)
        {
            if (entityNode == null || playerNode == null) return null;

            NodeBase bestNode = null;
            float bestDist = float.MaxValue;

            for (int row = 0; row < DungeonMap.Rows; row++)
            {
                for (int col = 0; col < DungeonMap.Cols; col++)
                {
                    var node = _navGraph.Grid[row, col];
                    if (node.IsWalkable) continue;

                    float dx = col - playerNode.Col;
                    float dy = row - playerNode.Row;
                    float len = MathF.Sqrt(dx * dx + dy * dy);
                    if (len < 0.5f) continue;

                    dx /= len;
                    dy /= len;

                    int hideCol = col + (int)Math.Round(dx);
                    int hideRow = row + (int)Math.Round(dy);

                    var candidate = _navGraph.GetNode(hideCol, hideRow);
                    if (candidate == null || !candidate.IsWalkable) continue;

                    float dist = Math.Abs(candidate.Col - entityNode.Col) +
                                 Math.Abs(candidate.Row - entityNode.Row);
                    if (dist < bestDist)
                    {
                        bestDist = dist;
                        bestNode = candidate;
                    }
                }
            }

            if (bestNode != null) return bestNode;

            float furthestDist = 0;
            for (int row = 0; row < DungeonMap.Rows; row++)
            {
                for (int col = 0; col < DungeonMap.Cols; col++)
                {
                    var node = _navGraph.Grid[row, col];
                    if (!node.IsWalkable) continue;
                    float d = Math.Abs(node.Col - playerNode.Col) + Math.Abs(node.Row - playerNode.Row);
                    if (d > furthestDist)
                    {
                        furthestDist = d;
                        bestNode = node;
                    }
                }
            }
            return bestNode;
        }
    }
}
