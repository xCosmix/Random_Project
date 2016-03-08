using UnityEngine;
using System.Collections;

namespace BehaviourTree
{
    public class Tree
    {

        Node root; ///TICK NODE

        public Tree(Node root)
        {
            this.root = root;
        }

        public void Tick(Blackboard blackBoard)
        {
            /* TICK */

            Tick tick = new Tick(blackBoard, this);
            root.Execute(tick);

            /* Retrieve info from tick n blackboard */

            Node[] lastOpenNodes = blackBoard.Get(this).treeMemory.Get<Node[]>("openNodes");
            Node[] openNodes = tick.GetOpenNodes();

            /* do not close if still open */

            if (lastOpenNodes != null)
            {
                int start = 0; //pick the difference to start frome the 
                int min = Mathf.Min(lastOpenNodes.Length, openNodes.Length);
                for (int i = 0; i < min; i++)
                {
                    start = i + 1;
                    if (lastOpenNodes[i] != openNodes[i]) break;
                }

                /* close nodes */

                for (int i = lastOpenNodes.Length - 1; i >= start; i--)
                {
                    lastOpenNodes[i]._Close(tick);
                }
            }

            /* populate the blackboard */

            tick.blackBoard.Set("openNodes", openNodes, this);
            tick.blackBoard.Set("nodeCount", tick.nodeCount, this);

            blackBoard.Set("nodeCount", tick.nodeCount, this);
        }
    }
}
