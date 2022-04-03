using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollObject : MonoBehaviour
{
    public float ZPos;
    public float XVelocity;

    private new Rigidbody2D rigidbody2D;
    // Start is called before the first frame update
    void Start()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, ZPos);
        rigidbody2D = gameObject.GetComponent<Rigidbody2D>();
        rigidbody2D.velocity = Vector2.left * XVelocity;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "MainCamera")
        {
            Destroy(gameObject);
        }
    }
}
