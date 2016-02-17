using UnityEngine;
using System.Collections;

public class PlayerCamera : MonoBehaviour {

    public float speed;
    public float distance = 10.0f;
    public float dynamic_distance_fact = 2.0f;

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

	void Update () {
        delta = target - transform.position;
        transform.position += delta * Time.deltaTime * speed;
	}

    public void Target(Transform obj)
    {
        float dynamic_distance = distance * DinamicDistance(obj.position);
        Vector3 plane_origin = obj.position + (transform.forward * -dynamic_distance);
        Plane height_plane = new Plane(transform.forward, plane_origin);
        Ray ray_height = new Ray(transform.position, (plane_origin - transform.position).normalized);
        float distance_height = 0.0f;
        height_plane.Raycast(ray_height, out distance_height);
        target = ray_height.GetPoint(distance_height);
    }
    public void Target(Vector3 pos)
    {
        float dynamic_distance = distance * DinamicDistance(pos);
        Vector3 plane_origin = pos + (transform.forward * -dynamic_distance);
        Plane height_plane = new Plane(transform.forward, plane_origin);
        Ray ray_height = new Ray(transform.position, (plane_origin - transform.position).normalized);
        float distance_height = 0.0f;
        height_plane.Raycast(ray_height, out distance_height);
        target = ray_height.GetPoint(distance_height);
    }
    /// <summary>
    /// returns a fact to divide by fixed distance 2 get dynamic zooming
    /// </summary>
    /// <param name="t"></param>
    /// <returns></returns>
    private float DinamicDistance (Vector3 t)
    {
        Plane plane = new Plane(transform.forward, t);
        Ray cam_ray = new Ray(transform.position, transform.forward);
        float distance = 0.0f;
        plane.Raycast(cam_ray, out distance);
        Vector3 point = cam_ray.GetPoint(distance);
        float distance_inNormal = (t - point).magnitude;
        return Mathf.Clamp(distance_inNormal / dynamic_distance_fact, 1.0f, 2.0f);
    }
}
