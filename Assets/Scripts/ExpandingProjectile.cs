using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpandingProjectile : Projectile
{
    public float BlowSizeFactor;
    public float BlowTime;
    public bool StopsOnBlow = true;

    private float blowStartTime;
    private float blowEndTime;

    private new Collider2D collider2D;

    private SpriteRenderer spriteRenderer;
    private Color startColor;
    private Color endColor;

    protected override void Start()
    {
        base.Start();
        collider2D = gameObject.GetComponent<Collider2D>();
        collider2D.enabled = false;
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        startColor = spriteRenderer.color;
        endColor = startColor;
        endColor.a = 0.0f;

    }
    protected override void Update()
    {
        if (Active)
        {
            float lerpFactor = (Time.time - blowStartTime) / BlowTime;
            transform.localScale = Vector2.Lerp(Vector2.one, Vector2.one * BlowSizeFactor, lerpFactor);
            spriteRenderer.color = Color.Lerp(startColor, endColor, lerpFactor);
            if (Time.time > blowEndTime)
            {
                Destroy(gameObject);
            }
        }
        else
        {
            base.Update();
        }

    }
    protected override void LifeEnd()
    {
        Active = true;
        blowStartTime = Time.time;
        blowEndTime = blowStartTime + BlowTime;
        if (StopsOnBlow)
        {
            rigidbody2D.velocity = Vector2.zero;
        }
        collider2D.enabled = true;
    }
}
