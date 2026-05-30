using System.Collections;
using UnityEngine;
using DG.Tweening;

public partial class Player
{
    public void UseNail1() => UseNail(0);
    public void UseNail2() => UseNail(1);

    private void UseNail(int skillIndex)
    {
        if (skillIndex < 0 || skillIndex >= canUse.Length || !canUse[skillIndex])
        {
            Debug.Log($"Nail{skillIndex + 1}을 사용할 수 없습니다.");
            return;
        }

        skillLastUsedTimes[skillIndex] = Time.time;
        skillGauge -= skillCosts[skillIndex];
        UpdateSkillGauge();

        if (skillIndex == 0)
            StartCoroutine(Nail1Coroutine());

        Debug.Log($"Nail{skillIndex + 1} 사용!");
    }

    private IEnumerator Nail1Coroutine()
    {
        Boss boss = FindObjectOfType<Boss>();
        boss.bulletEmitter.Kill();
        yield return new WaitForSeconds(0.1f);
        boss.bulletEmitter.Play();
    }

    private void UpdateSkillFades()
    {
        for (int i = 0; i < isEquippedSkill.Length; i++)
        {
            if (!isEquippedSkill[i])
                continue;

            float remainingCooldown = skillLastUsedTimes[i] + skillCooldowns[i] - Time.time;
            if (remainingCooldown > 0)
                skillFades.fillAmount = Mathf.Clamp01(remainingCooldown / skillCooldowns[i]);
        }
    }

    public void UpdateSkillGauge()
    {
        skillGaugeText.text = Mathf.Floor(skillGauge).ToString();
        DOTween.To(() => skillGaugeImage.fillAmount, x => skillGaugeImage.fillAmount = x, skillGauge / maxSkillGauge, 0.2f)
            .SetEase(Ease.OutBounce);
    }

    private void CheckNailAvailability()
    {
        for (int i = 0; i < isEquippedSkill.Length; i++)
        {
            if (!isEquippedSkill[i])
                continue;

            bool ready = Time.time >= skillLastUsedTimes[i] + skillCooldowns[i]
                && skillGauge >= skillCosts[i];

            glowObject.SetActive(ready);
            canUse[i] = ready;
        }
    }
}
