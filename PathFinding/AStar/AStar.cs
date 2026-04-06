using System;
using System.Collections.Generic;
using System.Linq;

namespace Project.PathFinding
{
    public class AStar
    {
        public static List<NodeBase> FindPath(NodeBase startNode, NodeBase targetNode)
        {
            var openList = new List<NodeBase> { startNode };
            var closedList = new HashSet<NodeBase>();

            startNode.SetG(0);
            startNode.SetH(GetDistance(startNode, targetNode));

            while (openList.Count > 0)
            {
                var current = openList.OrderBy(n => n.F).ThenBy(n => n.H).First();

                if (current == targetNode)
                    return ReconstructPath(targetNode);

                openList.Remove(current);
                closedList.Add(current);

                foreach (var neighbor in current.Neighbors)
                {
                    if (!neighbor.IsWalkable || closedList.Contains(neighbor))
                        continue;

                    float tentativeG = current.G + GetDistance(current, neighbor);

                    if (!openList.Contains(neighbor))
                    {
                        neighbor.SetConnection(current);
                        neighbor.SetG(tentativeG);
                        neighbor.SetH(GetDistance(neighbor, targetNode));
                        openList.Add(neighbor);
                    }
                    else if (tentativeG < neighbor.G)
                    {
                        neighbor.SetConnection(current);
                        neighbor.SetG(tentativeG);
                    }
                }
            }

            return null; 
        }

        private static List<NodeBase> ReconstructPath(NodeBase endNode)
        {
            var path = new List<NodeBase>();
            var current = endNode;

            while (current != null)
            {
                path.Add(current);
                current = current.Connection;
            }

            path.Reverse();
            return path;
        }

        private static float GetDistance(NodeBase a, NodeBase b)
        {
            int dx = Math.Abs(a.Col - b.Col);
            int dy = Math.Abs(a.Row - b.Row);
            return Math.Max(dx, dy) + (1.41421356f - 1) * Math.Min(dx, dy);
        }
    }
}
