using System.Collections;
using UnityEngine;
using DG.Tweening;

public partial class Player
{
    private float dashHoldTime;
    private bool isDashing;
    private Vector2 dashDirection;
    private float lastDashTime;
    private bool dashEndedAndCanFly;

    private const float dashInitialFactor = 0.65f;
    private const float dashAdditionalFactor = 0.35f;
    private const float shortReverseDashFactor = 0.2f;
    private const float longReverseDashFactor = 0.25f;

    private void UpdateDashAndFlyInput()
    {
        if (isDashPressed)
        {
            dashHoldTime = 0f;
            if (CanStartDash())
                Dash();
        }

        if (isDashHeld)
            dashHoldTime += Time.deltaTime;

        TryFlyAfterDash();
        TryStartFlyFromInput();

        if (isDashReleased && isFlying)
            EndFly();
    }

    private bool CanStartDash()
    {
        return isGrounded
            && !isFlying
            && !isDashing
            && Time.time >= lastDashTime + dashCooldown
            && !Input.GetKey(KeyCode.W);
    }

    private void TryFlyAfterDash()
    {
        if (isDashing || !dashEndedAndCanFly || isFlying || isGrounded)
            return;

        if (Input.GetKey(KeyCode.W))
            Fly();

        dashEndedAndCanFly = false;
    }

    private void TryStartFlyFromInput()
    {
        if (!isDashHeld || isDashing || !canFlyAgainAfterLanding)
            return;

        if (Input.GetKey(KeyCode.W) || !isGrounded || isFlying)
            Fly();
    }

    private void Dash()
    {
        if (gauge.value < dashGauge || isDashing)
            return;

        dashEndedAndCanFly = false;
        StartCoroutine(DashCoroutine());
    }

    private IEnumerator DashCoroutine()
    {
        anim.speed = 2f;
        yield return StartCoroutine(PerformDashPhase(dashInitialFactor));

        Vector2 lastDashDirection = dashDirection;

        if (Input.GetKey(KeyCode.W) && !isGrounded)
            dashEndedAndCanFly = true;

        if (dashHoldTime >= longDashThreshold)
        {
            anim.speed = 0.7f;
            yield return StartCoroutine(PerformDashPhase(dashAdditionalFactor, dashDirection));
        }
        else if (dashEndedAndCanFly)
            Fly();

        float reverseFactor = dashHoldTime >= longDashThreshold ? longReverseDashFactor : shortReverseDashFactor;

        if (lastDashDirection == Vector2.left && Input.GetKey(KeyCode.D))
            yield return StartCoroutine(PerformDashPhase(reverseFactor, Vector2.right));
        else if (lastDashDirection == Vector2.right && Input.GetKey(KeyCode.A))
            yield return StartCoroutine(PerformDashPhase(reverseFactor, Vector2.left));
    }

    private IEnumerator PerformDashPhase(float dashFactor, Vector2? direction = null)
    {
        if (gauge.value < dashGauge * dashFactor && dashFactor > 0.3f)
            yield break;

        if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Dash"))
            anim.SetBool("isDash", true);

        if (dashFactor > 0.3f)
        {
            targetGauge = gauge.value - dashGauge * dashFactor;
            DOTween.To(() => gauge.value, x => gauge.value = x, targetGauge, 0.15f)
                .SetEase(Ease.OutQuint);
        }

        StartCoroutine(SyncDelaySliderToGauge());

        isDashing = true;
        lastDashTime = Time.time;
        StartCoroutine(DashInvincibilityCoroutine());

        dashDirection = direction ?? (spriteRenderer.flipX ? Vector2.right : Vector2.left);

        if (Mathf.Abs(dashFactor - dashAdditionalFactor) > 0.01f)
            SpawnDashParticle();

        float clampedX = Mathf.Clamp(
            transform.position.x + dashDirection.x * dashSpeed * dashFactor,
            leftBorder,
            rightBorder);

        float dashDuration = dashTime * dashFactor;
        Ease ease = dashFactor <= 0.3f ? Ease.InQuad : Ease.Linear;

        transform.DOMoveX(clampedX, dashDuration)
            .SetEase(ease)
            .OnComplete(() => isDashing = false);

        yield return new WaitForSeconds(dashDuration);

        anim.SetBool("isDash", false);
        anim.speed = 1f;
    }

    private void SpawnDashParticle()
    {
        GameObject particles = Instantiate(dashParticle, transform.position, Quaternion.identity);
        particles.transform.rotation = dashDirection == Vector2.right
            ? Quaternion.Euler(0, 90, 0) : Quaternion.Euler(0, -90, 0);
        particles.transform.position = transform.position;
        Destroy(particles, 2f);
    }

    private IEnumerator DashInvincibilityCoroutine()
    {
        isDashInvincibility = true;
        yield return new WaitForSeconds(dashTime);
        isDashInvincibility = false;
    }

    private IEnumerator SyncDelaySliderToGauge()
    {
        yield return new WaitForSeconds(0.15f);
        DOTween.To(() => delaySlider.value, x => delaySlider.value = x, gauge.value, 0.15f);
    }
}
