using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundScroller : MonoBehaviour
{
    public Vector3 ScrollMoveDistance;
    public Vector3 FrameScrollSpeed;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += FrameScrollSpeed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "MainCamera")
        {
            Debug.Log("Scrolling " + gameObject);
            transform.position += ScrollMoveDistance;
        }
    }
}
