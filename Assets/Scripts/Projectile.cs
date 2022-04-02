using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Projectile : MonoBehaviour
{

    public enum ProjectileType { TOY, NIP, MEOW };

    public ProjectileType Type;

    public Vector2 StartVelocity;  // starting velocity of the object in units/second
    public Vector2 Acceleration;   // acceleration of the projectile, in units/second/second

    private new Rigidbody2D rigidbody2D;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        rigidbody2D.velocity = StartVelocity;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        rigidbody2D.velocity += Acceleration * Time.fixedDeltaTime;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 7)    // projectile bounds
                                                // is there a way to look this up by name?
        {
            Debug.Log(gameObject + " collided with projectile bounds");
            Destroy(gameObject);
        }
    }
}
