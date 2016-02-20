using UnityEngine;
using System.Collections;
using System;

public class test : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Action.Create<Test_Action>(this.gameObject);
        StartCoroutine(start_after());
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    IEnumerator start_after ()
    {
        yield return new WaitForSeconds(2.0f);
        Action.Create<Test_Action_2>(this.gameObject);
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
        if (Time.time - myTime > 10.0f) return true;
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
[ActionProperties("test2", "default", 1, true)]
public class Test_Action_2 : Action
{
    float myTime = 0.0f;

    public override void End()
    {
        Debug.Log("ending2");
    }

    public override bool Fail()
    {
        return false;
    }

    public override bool Goal()
    {
        if (Time.time - myTime > 3.0f) return true;
        return false;
    }

    public override void Queued()
    {
        Debug.Log("queued2");
    }

    public override void Start()
    {
        myTime = Time.time;
        Debug.Log("starting2");
    }

    public override void Suspended()
    {
        Debug.Log("suspended2");
    }

    public override void Update()
    {
        Debug.Log("updating2");
    }
}
