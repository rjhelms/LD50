using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{

    public Transform pathParent;
    public Transform[] path;
    public int pathIndex;

    public float MoveSpeed;
    public Vector2 Velocity;
    public float PathTargetDistance = 0.25f;

    int pathLength;

    new Rigidbody2D rigidbody2D;
    Transform projectileParent;
    GameController gameController;

    // Start is called before the first frame update
    void Start()
    {
        pathLength = pathParent.childCount;
        path = new Transform[pathLength];
        for (int i = 0; i < pathLength; i++)
        {
            path[i] = pathParent.Find(i.ToString());
        }

        rigidbody2D = GetComponent<Rigidbody2D>();
        gameController = FindObjectOfType<GameController>();
        projectileParent = GameObject.Find("Projectiles").transform;
    }

    // Update is called once per frame
    void Update()
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

    private void FixedUpdate()
    {
        rigidbody2D.velocity = Velocity;
    }
}
