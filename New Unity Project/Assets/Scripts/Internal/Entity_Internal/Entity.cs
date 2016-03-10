using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Forces;

[RequireComponent(typeof(Rigidbody2D))]
public abstract class Entity : MonoBehaviour {

    #region Components

    protected Rigidbody2D rigidBody;
    protected ActionController actionController;
    protected SpriteRenderer spriteRenderer;
    protected Animator animator;
    public Animator Animator
    {
        get { return animator; }
    }

    public Rigidbody2D RigidBody
    {
        get { return rigidBody; }
    }

    #endregion

    #region EntityProps

    public Stats stats;

    #endregion

    #region CharacterController

    public CCProperties characterController;
    public abstract Vector2 AimDir { get; set; }

    #endregion

    #region Events

    delegate void DamageEvent(HitProps hit);
    //event DamageEvent OnDamage;
    //event DamageEvent OnDead;

    #region Damage & Dead

    public sealed class HitProps
    {
        public readonly Weapon weapon;
        public readonly Vector2 hitPoint;
        public readonly float time;

        public HitProps(Weapon weapon, Vector2 hitPoint)
        {
            this.weapon = weapon;
            this.hitPoint = hitPoint;

            time = Time.time;
        }
    }

    private HitProps lastHit;
    public const float inmuneTime = 0.2f;

    public void WeaponHit (Weapon weapon, Vector2 hitPoint)
    {
        if (lastHit != null) 
            if (Time.time - lastHit.time < inmuneTime) return;

        lastHit = new HitProps(weapon, hitPoint);
        Damage();
    }

    private void Damage ()
    { 
        stats.life--;
        if (stats.life <= 0) { stats.life = 0; Die(); } else { Hit(); }
    }
    private void Die ()
    {
        //OnDead(lastHit);
        StartCoroutine(DieSteps());
    }
    private void Hit ()
    {
        //OnDamage(lastHit);
        StartCoroutine(HitSteps());
    }

    private IEnumerator DieSteps ()
    {
        yield return StartCoroutine(DieAnimation());
        Destroy(this.gameObject);
    }
    private IEnumerator HitSteps ()
    {
        StartCoroutine(DefaultHitAnimation());
        StartCoroutine(HitAnimation());
        yield break;
    }
    protected virtual IEnumerator DieAnimation () { yield break; }
    protected virtual IEnumerator HitAnimation () { yield break; }

    private IEnumerator DefaultHitAnimation ()
    {
        float pushForce = (lastHit.weapon.HitWeight - characterController.weight);
        pushForce = pushForce < 0.2f ? 0.2f : pushForce;

        Vector2 direction = ((Vector2)transform.position - lastHit.hitPoint).normalized;
        rigidBody.AddForce(direction * pushForce, ForceMode2D.Impulse);

        Color realColor = spriteRenderer.color;
        spriteRenderer.color = Color.red;

        yield return new WaitForSeconds(inmuneTime);

        spriteRenderer.color = realColor;
    }

    #endregion

    #endregion

    #region Initialization

    void Awake ()
    {
        CustomAwake();
    }

    void Start ()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        actionController = gameObject.AddComponent<ActionController>();
        animator = GetComponent<Animator>();

        CustomStart();
    }

    protected virtual void CustomStart ()
    {

    }

    protected virtual void CustomAwake()
    {

    }

    #endregion

    #region Updates

    void Update ()
    {
        CustomUpdate();
        LookAimDir();
	}

    void FixedUpdate ()
    {
        CustomFixedUpdate();
        //forceController.Actualize();
    }

    protected virtual void CustomUpdate ()
    {

    }

    protected virtual void CustomFixedUpdate ()
    {

    }

    #endregion

    private void LookAimDir ()
    {
        float dir = Mathf.Sign(AimDir.x);
        Vector2 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * dir;

        transform.localScale = scale;
    }
}

#region Structs

    /// <summary>
    /// the stats of the entity
    /// </summary>
    [System.Serializable]
    public struct Stats
    {
        public int life;
        public int level;
    }

    [System.Serializable]
    public struct CCProperties
    {
        public float speed;
        public float weight;
        public ForceProps[] forces;
        public float Speed
        {
            get { return speed * Time.deltaTime * 100.0f; }
        }
    }

#endregion
