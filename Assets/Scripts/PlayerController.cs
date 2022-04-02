using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float MoveSpeed;
    public Vector2 Velocity;

    public GameObject toyProjectilePrefab;
    public GameObject nipProjectilePrefab;

    private new Rigidbody2D rigidbody2D;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        float x_move = Input.GetAxis("Horizontal");
        float y_move = Input.GetAxis("Vertical");
        Velocity = new Vector2(x_move, y_move) * MoveSpeed;

        if (Input.GetButtonDown("Fire1"))
        {
            Instantiate(toyProjectilePrefab, transform.position, Quaternion.identity);
        }
    }

    private void FixedUpdate()
    {
        rigidbody2D.velocity = Velocity;
    }
}
