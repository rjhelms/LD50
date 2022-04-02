using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public string pathParentName;
    public Transform pathParent;
    public Transform[] path;
    public int pathIndex;

    public Sprite[] standingSprites;
    public Sprite[] sleepingSprites;

    public float MoveSpeed;
    public Vector2 Velocity;
    public Vector2 DeadVelocity;
    public float PathTargetDistance = 0.25f;

    public bool Started = false;
    public bool Alive = true;

    public bool DoesMeow;

    public GameObject MeowPrefab;
    public float meowInterval;
    public float meowIntervalSpread;
    public float meowChance;

    int pathLength;

    new Rigidbody2D rigidbody2D;
    Transform projectileParent;
    GameController gameController;
    SpriteRenderer spriteRenderer;
    Transform deadEnemiesParent;

    int spriteIdx;
    float nextMeowTime;

    public void BuildPath()
    {
        pathParent = GameObject.Find(pathParentName).transform;
        pathLength = pathParent.childCount;
        path = new Transform[pathLength];
        for (int i = 0; i < pathLength; i++)
        {
            path[i] = pathParent.Find(i.ToString());
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        gameController = FindObjectOfType<GameController>();
        projectileParent = GameObject.Find("Projectiles").transform;
        deadEnemiesParent = GameObject.Find("DeadEnemies").transform;
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteIdx = Random.RandomRange(0, standingSprites.Length);
        spriteRenderer.sprite = standingSprites[spriteIdx];
        if (DoesMeow)
        {
            SetNextMeowTime();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!Started)
        {
            if (DoesMeow)
            {
                nextMeowTime += Time.deltaTime;
            }
            return;
        }

        if (Alive)
        {
            DoMovement();
            if (DoesMeow & Time.time > nextMeowTime)
            {
                SetNextMeowTime();
                if (Random.value < meowChance)
                {
                    Meow();
                }
            }
            if (Velocity.x > 0.1f)
            {
                spriteRenderer.flipX = true;
            }
            else if (Velocity.x < -0.1f)
            {
                spriteRenderer.flipX = false;
            }
        } else
        {
            Velocity = Vector2.Lerp(Velocity, DeadVelocity, Time.deltaTime * 5);
            if (!spriteRenderer.isVisible)
                Destroy(gameObject);
        }
    }

    private void FixedUpdate()
    {
        rigidbody2D.velocity = Velocity;
    }

    private void DoMovement()
    {
        // calculate velocity along path
        Velocity = path[pathIndex].position - transform.position;
        if (Velocity.magnitude < PathTargetDistance)
        {
            // move to next path point
            pathIndex++;
            pathIndex %= pathLength;
            // and recalculate velocity
            Velocity = path[pathIndex].position - transform.position;
        }
        Velocity = Velocity.normalized * MoveSpeed;
    }

    private void Die()
    {
        transform.parent = deadEnemiesParent;
        spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 0.5f);
        Alive = false;
        gameObject.layer = 11;  // DeadEnemies
        transform.position += new Vector3(0, 0, 5); // move back 5 to put behind other things
        spriteRenderer.sprite = sleepingSprites[spriteIdx];
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 8)
        {
            Projectile projectile = collision.gameObject.GetComponent<Projectile>();
            Die();
            if (projectile.Type == Projectile.ProjectileType.TOY)
            {
                Destroy(collision.gameObject);
            }
        }
    }

    private void SetNextMeowTime()
    {
        float thisMeowInterval = meowInterval + Random.Range(-meowIntervalSpread, meowIntervalSpread);
        nextMeowTime = Time.time + thisMeowInterval;
    }

    private void Meow()
    {
        GameObject newMeow = Instantiate(MeowPrefab, transform.position, Quaternion.identity, projectileParent);
        Projectile meowProjectile = newMeow.GetComponent<Projectile>();
        Debug.Log("Setting meow velocity");
        meowProjectile.StartVelocity = Velocity;
    }
}
