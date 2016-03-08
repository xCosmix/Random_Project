using UnityEngine;
using ObjectPool;
using System.Collections;

public class EnergyPoint : MonoBehaviour {

    [Range(25, 105)]
    public int value = 25;

    protected float minSize = 2.0f;
    protected float maxSize = 5.0f;
    protected Player player;
    protected Rigidbody2D rigidBody;

    public void New(int value)
    {
        player = Player.player;
        this.value = value;
        float sizeInterpolation = (float)value / 105.0f;
        float size = Mathf.Lerp(minSize, maxSize, sizeInterpolation);
        transform.localScale = new Vector2(size, size);

        float randomAngle = Random.Range(0.0f, 360.0f);
        Vector2 dir = Quaternion.AngleAxis(randomAngle, Vector3.forward) * Vector2.up;
        float randomForce = Random.Range(1.0f, 10.0f);

        if (rigidBody == null) rigidBody = GetComponent<Rigidbody2D>();

        rigidBody.AddForce(dir * randomForce, ForceMode2D.Impulse);
    }
	// Update is called once per frame
	void Update () {
        Vector2 delta = player.transform.position - transform.position;
        if (delta.sqrMagnitude < 30.0f)
        {
            rigidBody.AddForce(delta.normalized * 100.0f);

            if (delta.sqrMagnitude < 0.5f)
                PickUp();
        }
	}

    void PickUp()
    {
        player.Score = value;
        Pool.Destroy(this.gameObject);
    }
}
