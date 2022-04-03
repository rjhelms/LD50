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

    public float MoveSpeedFudge;

    public bool Started = false;
    public bool Alive = true;

    public bool DoesMeow;

    public GameObject MeowPrefab;
    public float meowInterval;
    public float meowIntervalSpread;
    public float meowChance;

    public bool DoesMagic;

    public GameObject MagicPrefab;
    public float magicInterval;
    public float magicIntervalSpread;
    public float magicChance;
    public float magicForcedMiss;
    int pathLength;

    new Rigidbody2D rigidbody2D;
    Transform projectileParent;
    GameController gameController;
    SpriteRenderer spriteRenderer;
    Transform deadEnemiesParent;

    int spriteIdx;
    float nextMeowTime;
    float nextMagicTime;

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
        spriteIdx = Random.Range(0, standingSprites.Length);
        spriteRenderer.sprite = standingSprites[spriteIdx];
        MoveSpeed *= gameController.EnemySpeedRatio;
        if (DoesMeow)
        {
            meowInterval /= gameController.EnemySpeedRatio;
            SetNextMeowTime();
        }
        if (DoesMagic)
        {
            magicInterval /= gameController.EnemySpeedRatio;
            magicForcedMiss /= gameController.EnemySpeedRatio;
            SetNextMagicTime();
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
            if (DoesMagic)
            {
                nextMagicTime += Time.deltaTime;
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
            if (DoesMagic & Time.time > nextMagicTime)
            {
                SetNextMagicTime();
                if (Random.value < magicChance)
                {
                    Magic();
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
        float moveVariance = Random.Range(1 - MoveSpeedFudge, 1 + MoveSpeedFudge);

        Velocity = Velocity.normalized * MoveSpeed * moveVariance;
    }

    public void Die()
    {
        transform.parent = deadEnemiesParent;
        spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 0.5f);
        Alive = false;
        gameObject.layer = 11;  // DeadEnemies
        transform.position += new Vector3(0, 0, 5); // move back 5 to put behind other things
        spriteRenderer.sprite = sleepingSprites[spriteIdx];
        gameController.TrySpawnPowerup(transform.position);
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
    
    private void SetNextMagicTime()
    {
        float thisMagicInterval = magicInterval + Random.Range(-magicIntervalSpread, magicIntervalSpread);
        nextMagicTime = Time.time + thisMagicInterval;
    }

    private void Meow()
    {
        // don't do anything if not on screen yet
        if (!spriteRenderer.isVisible)
        {
            return;
        }
        GameObject newMeow = Instantiate(MeowPrefab, transform.position, Quaternion.identity, projectileParent);
        Projectile meowProjectile = newMeow.GetComponent<Projectile>();
        meowProjectile.StartVelocity = Velocity;
        gameController.PlayMeow();
    }

    private void Magic()
    {
        // don't do anything if not on screen yet
        if (!spriteRenderer.isVisible)
        {
            return;
        }
        Transform playerTransform = GameObject.Find("Player").transform;
        // only cast spell when facing player
        if (spriteRenderer.flipX & playerTransform.position.x < transform.position.x)
            return;
        if (!spriteRenderer.flipX & playerTransform.position.x > transform.position.x)
            return;
        Vector2 magicVelocity = playerTransform.position - transform.position;
        if (Random.value < 0.5f)
        {
            magicVelocity += Vector2.up * magicForcedMiss;
        } else
        {
            magicVelocity += Vector2.down * magicForcedMiss;
        }
        magicVelocity = magicVelocity.normalized * gameController.MagicSpeed * gameController.EnemySpeedRatio;
        GameObject newMagic = Instantiate(MagicPrefab, transform.position, Quaternion.identity, projectileParent);
        Projectile magicProjectile = newMagic.GetComponent<Projectile>();
        magicProjectile.StartVelocity = magicVelocity;
        gameController.PlayMagic();
    }
}
