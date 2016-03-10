using UnityEngine;
using System.Collections;

public class Blade : MonoBehaviour {

    [Range(0.0f, 360.0f)]
    public float angle = 90.0f;
    public float windowAngle = 20.0f;
    public float radius = 1.0f;
    public float duration = 0.1f;
    public int directionValue = 1;

    protected Vector2 dir;
    private Weapon owner;

    public void New(Vector2 dir, Weapon owner)
    {
        this.dir = dir;
        this.owner = owner;

        StartCoroutine(BladeCheck());
    }

    // Update is called once per frame
    IEnumerator BladeCheck () {
        float currentTime = 0.0f;
        float interpolation = 0.0f;
        Transform ownerT = owner.owner.transform;

        while (currentTime < duration)
        {
            interpolation = currentTime / duration;
            Collider2D[] colls = Physics2D.OverlapCircleAll(ownerT.position, radius, 512);

            Vector2 currentBladeDirection = Quaternion.AngleAxis(angle / 2.0f - (angle * interpolation), Vector3.forward) * dir;

            foreach (Collider2D coll in colls)
            {
                Entity ent = coll.GetComponent<Entity>();
                if (coll.gameObject == owner.owner) continue;

                Vector2 position = coll.gameObject.transform.position;
                Vector2 currentDir = position - (Vector2)ownerT.position;
                float angleDiff = Vector2.Angle(currentDir.normalized, currentBladeDirection);

                if (angleDiff < windowAngle)
                {
                    ent.WeaponHit(owner, transform.position);
                }
            }

            yield return new WaitForFixedUpdate();
            currentTime += Time.fixedDeltaTime;
        }

        ///raw solution to wait animation
        yield return new WaitForSeconds(0.3f);

        ObjectPool.Pool.Destroy(this.gameObject);
	}
}
