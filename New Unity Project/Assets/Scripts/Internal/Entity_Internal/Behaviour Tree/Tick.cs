using UnityEngine;
using System.Collections.Generic;

namespace BehaviourTree
{
    public class Tick
    {

        List<Node> openNodes = new List<Node>();

        public int nodeCount = 0;
        public Blackboard blackBoard;
        public Tree tree;

        public Tick(Blackboard blackBoard, Tree tree)
        {
            this.blackBoard = blackBoard;
            this.tree = tree;
        }
        public Node[] GetOpenNodes()
        {
            Node[] output = new Node[openNodes.Count];
            for (int i = 0; i < output.Length; i++)
            {
                output[i] = openNodes[i];
            }
            return output;
        }

        public void EnterNode(Node node)
        {
            nodeCount++;
            openNodes.Add(node);
        }
        public void OpenNode(Node node)
        {
            //debug
        }
        public void TickNode(Node node)
        {
            //debug
        }
        public void CloseNode(Node node)
        {
            //debug
            openNodes.Remove(node);
        }
        public void ExitNode(Node node)
        {
            //debug
        }
    }
}
