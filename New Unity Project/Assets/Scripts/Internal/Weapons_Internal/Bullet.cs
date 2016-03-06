using UnityEngine;
using System.Collections;
using ObjectPool;

[RequireComponent(typeof(CircleCollider2D))]
public class Bullet : MonoBehaviour {

    public float speed;
    public int lifeHits;

    private float startTime;
    private int currentHits;
    private Vector2 dir;
    private Weapon owner;

    public void New (Vector2 dir, Weapon owner)
    {
        this.dir = dir;
        this.owner = owner;

        currentHits = 0;
        startTime = Time.time;
    }
	void Update () {
        transform.Translate(dir * speed * Time.deltaTime, Space.World);

        ///Destroy if the bullet have a lifetime longer than 3 seconds
        if (Time.time - startTime > 3.0f) Pool.Destroy(this.gameObject);
	}

    void OnTriggerEnter2D (Collider2D coll)
    {
        if (coll.gameObject == owner.owner) return;

        Entity ent = coll.gameObject.GetComponent<Entity>();
        if (ent != null)
        {
            ent.WeaponHit(owner, this.transform.position);
        }

        currentHits++;
        if (currentHits >= lifeHits) Pool.Destroy(gameObject);
    }
}
