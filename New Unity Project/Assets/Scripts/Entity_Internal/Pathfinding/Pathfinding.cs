using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class Pathfinding {

    public static Vector3[] FindPath (Vector3 from, Vector3 target)
    {
        #region Defintion
        Node startNode = Grid.GetClosestNode(from);
        Node targetNode = Grid.GetClosestNode(target);

        Heap<Node> openSet = new Heap<Node>(Grid.GetMaxSize());
        HashSet<Node> closedSet = new HashSet<Node>();
        Node currentNode;

        #endregion

        #region Start 

        openSet.Add(startNode);

        #endregion

        #region Loop

        while (openSet.Count > 0)
        {
            #region Find lowest cost node

            currentNode = openSet.RemoveFirst();
            closedSet.Add(currentNode);

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
                
                if (!nb_node.walkable || closedSet.Contains(nb_node)) continue;

                int distance2Neighbour = currentNode.gCost + Grid.GetDistance(currentNode, nb_node);

                if (distance2Neighbour < nb_node.gCost || !openSetContains)
                {
                    nb_node.gCost = distance2Neighbour;
                    nb_node.hCost = Grid.GetDistance(nb_node, targetNode);
                    nb_node.parent = currentNode;

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

        return new Vector3[] { startNode.position };

        #endregion
    }

    private static Vector3[] RetracePath (Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }

        Vector3[] output = new Vector3[path.Count];
        for (int i = path.Count-1; i >= 0; i--)
        {
            output[i] = path[i].position;
        }

        return output;
    }
}
