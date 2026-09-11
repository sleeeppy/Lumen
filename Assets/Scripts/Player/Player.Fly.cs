using UnityEngine;

public partial class Player
{
    private GameObject curTrails;
    private bool canFlyAgainAfterLanding = true;

    private void Fly()
    {
        if (!isDashHeld || !canFlyAgainAfterLanding)
            return;

        if (flyCoolTime > curTime)
            return;

        if (!isFlying)
        {
            gauge.value -= 0.2f;
            isFlying = true;

            if (curTrails != null)
                Destroy(curTrails, 3f);

            curTrails = Instantiate(flyTrails, transform.position, Quaternion.identity);
        }

        curTrails.transform.position = transform.position;
        isInvincibility = true;
        gauge.value -= flyGauge * Time.deltaTime;

        if (gauge.value <= 0 && !isTouchBottom)
        {
            EndFly();
            return;
        }

        rigid2D.gravityScale = 0;
        spriteRenderer.enabled = false;
        anim.enabled = false;

        float flyX = Input.GetAxis("Horizontal");
        float flyY = Input.GetAxis("Vertical");

        if (isTouchTop) flyY = 0;
        if (isTouchLeft || isTouchRight) flyX = 0;

        rigid2D.velocity = new Vector2(flyX, flyY).normalized * (moveSpeed * 1.5f);
    }

    private void EndFly()
    {
        anim.SetBool("isAirborne", true);
        rigid2D.velocity = Vector2.zero;

        isFlying = false;
        if (!isHit)
            isInvincibility = false;

        rigid2D.gravityScale = highGravity;
        anim.enabled = true;
        transform.rotation = Quaternion.identity;
        curTime = 0;
        currentJumpCount = 0;

        spriteRenderer.enabled = true;
        canFlyAgainAfterLanding = false;
    }
}
