using UnityEngine;

/// <summary>
/// 아이템 장착 시 실제로 게임에 적용되는 효과 모음.
/// 이전에는 MonoBehaviour(ItemLogic)였지만 인스턴스가 new 로 잘못 생성되고 있었고,
/// 모든 메서드가 FindObjectOfType 로 대상을 찾으므로 컴포넌트일 이유가 없어 static 으로 정리했다.
/// ItemDatabase 의 각 ItemDefinition.ApplyEffect 에서 참조한다.
/// </summary>
public static class ItemEffects
{
    public static void IncreaseMaxLife()
    {
        Player player = Object.FindObjectOfType<Player>();
        if (player == null)
            return;

        player.maxLife += 1;
        player.life += 1;
        player.UpdateLifeIcon(player.life);
    }

    public static void IncreaseMaxGauge()
    {
        Player player = Object.FindObjectOfType<Player>();
        if (player != null)
            player.gauge.maxValue += 0.2f;
    }

    public static void SetBasicAttack()
    {
        Attack attack = Object.FindObjectOfType<Attack>();
        if (attack != null)
            attack.whatAttack = AttackType.Basic;
    }

    public static void SetLaserAttack()
    {
        Attack attack = Object.FindObjectOfType<Attack>();
        if (attack != null)
            attack.whatAttack = AttackType.Laser;
    }

    public static void EquipNail1()
    {
        Player player = Object.FindObjectOfType<Player>();
        if (player != null)
            player.isEquippedSkill[0] = true;
    }

    public static void EquipNail2()
    {
        Player player = Object.FindObjectOfType<Player>();
        if (player != null)
            player.isEquippedSkill[1] = true;
    }

    // ─────────────────────────────────────────────────────────────
    // 미구현 스텁 (틀만). 구현 시 이 메서드 본문만 채우면 된다.
    // ─────────────────────────────────────────────────────────────

    // TODO(미구현): 공격력 증가. Ring3/4/5 가 공유한다.
    //   현재 Player/Attack 에 공격력(데미지) 스탯이 없으므로, 스탯을 먼저 추가한 뒤
    //   여기서 올린다. 반지마다 증가량이 달라야 하면 ItemDefinition 에 수치 필드를
    //   추가해 데이터로 분기할 것(클래스 분리 불필요).
    public static void IncreaseAttackPower()
    {
        // 예) var attack = Object.FindObjectOfType<Attack>(); attack.damage += amount;
    }

    // TODO(미구현): 공격 타입을 유도(homing) 공격으로 변경.
    //   whatAttack 을 Homing 으로 바꿔도 Attack.FireHoming() 본문이 비어 있어 발사되지 않는다.
    //   Attack.FireHoming() 구현이 함께 필요.
    public static void SetHomingAttack()
    {
        Attack attack = Object.FindObjectOfType<Attack>();
        if (attack != null)
            attack.whatAttack = AttackType.Homing;
    }
}
