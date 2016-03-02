using UnityEngine;
using System.Collections;
using BehaviourTree;

public class testBT : MonoBehaviour {

    Blackboard blackBoard;
    BehaviourTree.Tree behaviourTree;

    // Use this for initialization
    void Start() {
        blackBoard = new Blackboard(this.gameObject);
        Node root = new Priority(
            new Node[]
            {
                new Sequence(
                    new Node[]
                    {
                        new ButtonPressed("X"),
                        new MemSequence(
                            new Node[]
                            {
                                new ChangeColor(Color.blue),
                                new Wait(1.0f),
                                new ChangePosition(Vector3.zero, Vector3.one*10.0f)
                               // new ChangeColor(Color.red)
                            }
                        )
                    }
                ),
                new ChangeColor(Color.red)
            }
        );
        behaviourTree = new BehaviourTree.Tree(root);
	}
	
	// Update is called once per frame
	void Update () {
        behaviourTree.Tick(blackBoard);
	}
}
