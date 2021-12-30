using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileArrow : Projectile
{
    [SerializeField] float _gravity;

    bool isGravityOn;

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        rb.gravityScale = 0f;

        startPosition = transform.position;
        isGravityOn = false;
        hasHitGround = false;
    }

    void FixedUpdate()
    {
        if (isGravityOn)
        {
            float angle = Mathf.Atan2(rb.velocity.y, rb.velocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }

        if (Vector3.Distance(startPosition, transform.position) >= travelDistance && !isGravityOn && !hasHitGround)
        {
            hasHitGround = true;
            isGravityOn = true;
            rb.gravityScale = _gravity;
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        attackDetails.position = transform.position;

        if (collision.CompareTag("Player"))
        {
            bool isHit = Player.Instance.Damage(attackDetails, entity, false);

            if (isHit)
            {
                rb.velocity = Vector2.zero;
                EnemyArrowPool.Instance.ReturnToPool(this);
            }
        }
        else
        {
            hasHitGround = true;
            isGravityOn = false;
            rb.gravityScale = 0f;
            rb.velocity = Vector2.zero;

            if (gameObject.activeSelf)
                StartCoroutine(DisableProjectile(projectileDurationAfterHitGround));
        }
    }

    IEnumerator DisableProjectile(float duration)
    {
        yield return new WaitForSeconds(duration);
        EnemyArrowPool.Instance.ReturnToPool(this);
    }
}
