using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public partial class Player
{
    public void OnHitByBullet()
    {
        if (isInvincibility || isDashInvincibility)
            return;

        life--;

        if (life == 0)
        {
            Die();
            return;
        }

        isHit = true;
        UpdateLifeIcon(life);
        StartCoroutine(InvincibilityCoroutine());
        StartCoroutine(HitInvincibility());

        flash = true;
        cg.alpha = 0.3f;

        CameraShake.instance.ShakeCamera();
    }

    public void Die()
    {
        SceneManager.LoadScene("Lobby");
    }

    private IEnumerator InvincibilityCoroutine()
    {
        isInvincibility = true;
        yield return new WaitForSeconds(hitInvincibilityTime);
        isInvincibility = false;
        isHit = false;
    }

    private IEnumerator HitInvincibility()
    {
        float elapsedTime = 0f;
        float waitTime = 0.17f;
        const float minWaitTime = 0.05f;

        while (elapsedTime < hitInvincibilityTime)
        {
            Color color = spriteRenderer.color;
            color.a = color.a == 1f ? 0.25f : 1f;
            spriteRenderer.color = color;

            yield return new WaitForSeconds(waitTime);
            elapsedTime += waitTime; 
            waitTime = Mathf.Max(waitTime * 0.9f, minWaitTime);
        }

        Color finalColor = spriteRenderer.color;
        finalColor.a = 1f;
        spriteRenderer.color = finalColor;
    }
}
