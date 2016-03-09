using UnityEngine;
using System.Collections.Generic;
namespace Pathfinding
{
    public static class Path
    {
        private const int layerMask = 128;

        public static Vector2[] FindPath(Vector3 from, Vector3 target)
        {
            Node startNode = Grid.GetClosestNode(from);
            Node targetNode = Grid.GetClosestNode(target);

            if (Physics2D.Linecast(startNode.position, targetNode.position))
            {
                return AStarPath(startNode, targetNode);
            }
            return StraightPath(startNode.position, targetNode.position);
        }

        private static Vector2[] StraightPath (Vector3 from, Vector3 target)
        {
            return new Vector2[] { from, target };
        }
        private static Vector2[] AStarPath (Node from, Node target)
        {
            #region Defintion

            Node startNode = from;
            Node targetNode = target;

            Heap<Node> openSet = new Heap<Node>(Grid.GetMaxSize());
            Node currentNode;

            foreach (Node n in Grid.GetGrid())
            {
                n.values.Reset();
            }

            #endregion

            #region Start 

            openSet.Add(startNode);

            #endregion

            #region Loop

            while (openSet.Count > 0)
            {
                #region Find lowest cost node

                currentNode = openSet.RemoveFirst();
                currentNode.values.closed = true;

                #endregion

                #region Check if arrived to goal

                if (currentNode == targetNode)
                {
                    return RetracePath(startNode, targetNode);
                }

                #endregion

                #region Add neighbours of current node to openSet or update them 

                foreach (Coords neighbour in currentNode.neighbours)
                {
                    Node nb_node = Grid.GetNode(neighbour);
                    bool openSetContains = openSet.Contains(nb_node);

                    if (!nb_node.walkable || nb_node.values.closed) continue;

                    int distance2Neighbour = currentNode.values.gCost + Grid.GetDistance(currentNode, nb_node);

                    if (distance2Neighbour < nb_node.values.gCost || !openSetContains)
                    {
                        nb_node.values.gCost = distance2Neighbour;
                        nb_node.values.hCost = Grid.GetDistance(nb_node, targetNode);
                        nb_node.values.Parent = currentNode;

                        if (!openSetContains)
                        {
                            openSet.Add(nb_node);
                        }
                        else
                        {
                            openSet.UpdateItem(nb_node);
                        }
                    }
                }

                #endregion

                #region Check if couldnt reach goal

                if (openSet.Count == 0)
                {
                    break;
                }

                #endregion
            }

            #endregion

            #region Default

            return new Vector2[] { startNode.position };

            #endregion
        }
        private static Vector2[] RetracePath(Node startNode, Node endNode)
        {
            List<Node> path = new List<Node>();
            Node currentNode = endNode;

            if (currentNode == startNode) return new Vector2[] { startNode.position };

            while (currentNode != startNode)
            {
                path.Add(currentNode);
                currentNode = currentNode.values.Parent;
            }
            
            Vector2[] output = new Vector2[path.Count];
            for (int i = 0; i < path.Count; i++)
            {
                output[i] = path[path.Count - 1 - i].position;
            }

            return output;
        }
    }
}
