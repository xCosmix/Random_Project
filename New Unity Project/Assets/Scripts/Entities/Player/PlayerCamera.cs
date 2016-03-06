using UnityEngine;
using System.Collections;

public class PlayerCamera : MonoBehaviour {

    public float speed;

    protected Vector3 target;
    protected Vector3 delta;
    private static PlayerCamera instance;

    public static PlayerCamera Get ()
    {
        if (instance == null) instance = FindObjectOfType<PlayerCamera>();
        return instance;
    }

	void Start ()
    {
        target = transform.position;
    }

	void FixedUpdate () {
        delta = target - transform.position;
        transform.position += delta * Time.deltaTime * speed;
	}

    public void Target(Transform obj)
    {
        target = obj.transform.position;
        target.z = transform.position.z;
    }
    public void Target(Vector3 pos)
    {
        target = pos;
        target.z = transform.position.z;
    }
}
