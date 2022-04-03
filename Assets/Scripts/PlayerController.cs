using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float MoveSpeed;
    public Vector2 Velocity;

    public GameObject toyProjectilePrefab;
    public GameObject nipProjectilePrefab;

    public int DefaultLayer;
    public int InvulnLayer;

    public float InvulnTime;
    public float FlashTime;

    public bool Invuln = false;

    public Sprite[] carpetSprites;
    public float carpetAnimTime;

    new Rigidbody2D rigidbody2D;
    Transform projectileParent;
    GameController gameController;
    SpriteRenderer spriteRenderer;
    SpriteRenderer carpetSpriteRenderer;

    float invulnEndTime;
    float nextFlashTime;
    float nextCarpetFrame;

    int carpetFrameIdx;
    // Start is called before the first frame update
    void Start()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        gameController = FindObjectOfType<GameController>();
        projectileParent = GameObject.Find("Projectiles").transform;
        spriteRenderer = GetComponent<SpriteRenderer>();
        carpetSpriteRenderer = transform.Find("Carpet").GetComponent<SpriteRenderer>();
        carpetFrameIdx = 0;
        nextCarpetFrame = Time.time + carpetAnimTime;
    }

    // Update is called once per frame
    void Update()
    {
        float x_move = Input.GetAxis("Horizontal");
        float y_move = Input.GetAxis("Vertical");
        Velocity = new Vector2(x_move, y_move) * MoveSpeed;

        if (Input.GetButton("Fire1") & Time.time > gameController.toyNextFireTime)
        {
            gameController.toyNextFireTime = Time.time + (1 / gameController.toyFireRate);
            Instantiate(toyProjectilePrefab, transform.position, Quaternion.Euler(Vector3.forward * Random.Range(0, 360)), projectileParent);
            gameController.PlayPlayerFire();
        }

        if (Input.GetButtonDown("Fire2") & gameController.Bombs > 0)
        {
            Instantiate(nipProjectilePrefab, transform.position, Quaternion.identity, projectileParent);
            gameController.Bombs--;
            gameController.PlayPlayerFire();
        }
        if (Invuln)
        {
            if (Time.time > nextFlashTime)
            {
                spriteRenderer.enabled = !spriteRenderer.enabled;
                nextFlashTime += FlashTime;
            }
            if (Time.time > invulnEndTime)
            {
                Invuln = false;
                spriteRenderer.enabled = true;
                gameObject.layer = DefaultLayer;
            }
        }

        if (Time.time > nextCarpetFrame)
        {
            carpetFrameIdx++;
            carpetFrameIdx %= carpetSprites.Length;
            carpetSpriteRenderer.sprite = carpetSprites[carpetFrameIdx];
            nextCarpetFrame += carpetAnimTime;
        }
    }

    private void FixedUpdate()
    {
        rigidbody2D.velocity = Velocity;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 10)   // LiveEnemies
        {
            gameController.PlayerHitByCat();
            invulnEndTime = Time.time + InvulnTime;
            nextFlashTime = Time.time + FlashTime;
            spriteRenderer.enabled = false;
            gameObject.layer = InvulnLayer;
            Invuln = true;
        }

        if (collision.gameObject.layer == 13)   // EnemyProjectile
        {
            gameController.PlayerHitByProjectile(collision.gameObject.GetComponent<Projectile>().Type);
            invulnEndTime = Time.time + InvulnTime;
            nextFlashTime = Time.time + FlashTime;
            spriteRenderer.enabled = false;
            gameObject.layer = InvulnLayer;
            Invuln = true;

            if (collision.gameObject.GetComponent<Projectile>().Type == Projectile.ProjectileType.MAGIC)
            {
                Destroy(collision.gameObject);
            }
        }
    }
}
