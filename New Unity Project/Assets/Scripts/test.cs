using UnityEngine;
using System.Collections;
using System;

public class test : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Action.Create<Test_Action>(this.gameObject);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
[ActionProperties("test", "default", 0, true)]
public class Test_Action : Action
{
    float myTime = 0.0f;

    public override void End()
    {
        Debug.Log("ending");
    }

    public override bool Fail()
    {
        return false;
    }

    public override bool Goal()
    {
        if (Time.time - myTime > 5.0f) return true;
        return false;
    }

    public override void Queued()
    {
        Debug.Log("queued");
    }

    public override void Start()
    {
        myTime = Time.time;
        Debug.Log("starting");
    }

    public override void Suspended()
    {
        Debug.Log("suspended");
    }

    public override void Update()
    {
        Debug.Log("updating");
    }
}

