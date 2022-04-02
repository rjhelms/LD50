using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float MoveSpeed;
    public Vector2 Velocity;

    public GameObject toyProjectilePrefab;
    public GameObject nipProjectilePrefab;



    new Rigidbody2D rigidbody2D;
    Transform projectileParent;
    GameController gameController;
    // Start is called before the first frame update
    void Start()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        gameController = FindObjectOfType<GameController>();
        projectileParent = GameObject.Find("Projectiles").transform;
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
        }

        if (Input.GetButtonDown("Fire2") & gameController.Bombs > 0)
        {
            Instantiate(nipProjectilePrefab, transform.position, Quaternion.identity, projectileParent);
            gameController.Bombs--;
        }
    }

    private void FixedUpdate()
    {
        rigidbody2D.velocity = Velocity;
    }
}
