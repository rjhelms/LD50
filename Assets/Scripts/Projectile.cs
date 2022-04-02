using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Projectile : MonoBehaviour
{

    public enum ProjectileType { TOY, NIP, MEOW };

    public ProjectileType Type;

    public Vector2 StartVelocity;  // starting velocity of the object in units/second
    public Vector2 Acceleration;   // acceleration of the projectile, in units/second/second
    public bool RandomColor;
    public bool RandomSpin;

    public bool FixedLife = false;
    public float lifeTime = 0.5f;
    public bool Active = true;

    protected new Rigidbody2D rigidbody2D;
    private float lifeEnd;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        rigidbody2D.velocity = StartVelocity;

        if (RandomColor)
        {
            GetComponent<SpriteRenderer>().color = Color.HSVToRGB(
                Random.value, Random.Range(0.4f, 0.7f), Random.Range(0.5f, 1));
        }

        if (RandomSpin)
        {
            rigidbody2D.angularVelocity = Random.Range(-720f, 720f);
        }

        if (FixedLife)
        {
            lifeEnd = Time.time + lifeTime;
        }
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (FixedLife & Time.time > lifeEnd)
        {
            Debug.Log(gameObject + "at end of life");
            LifeEnd();
        }
    }

    private void FixedUpdate()
    {
        if (!FixedLife | Time.time < lifeEnd)
        {
            rigidbody2D.velocity += Acceleration * Time.fixedDeltaTime;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 7 & !FixedLife)   // projectile bounds
                                                            // is there a way to look this up by name?
        {
            Debug.Log(gameObject + " collided with projectile bounds");
            Destroy(gameObject);
        }
    }

    protected virtual void LifeEnd()
    {
        Destroy(gameObject);
    }
}
