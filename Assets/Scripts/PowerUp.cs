using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    public int ZValue;
    public int BValue;
    public float PValue;

    public float YVelocity;

    new Rigidbody2D rigidbody2D;
    GameController gameController;
    // Start is called before the first frame update
    void Start()
    {
        rigidbody2D = gameObject.GetComponent<Rigidbody2D>();
        gameController = FindObjectOfType<GameController>();

        if (transform.position.y > 0)
        {
            YVelocity = -YVelocity;
        }
        rigidbody2D.velocity = new Vector2(0, YVelocity);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 7)    // projectile bounds
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.name == "Player")
        {
            gameController.RegisterPowerUp(ZValue, BValue, PValue);
            Destroy(gameObject);
        }
    }

}
