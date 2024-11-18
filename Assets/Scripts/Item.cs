using UnityEngine;
using UnityEngine.SceneManagement;

public class ItemLogic : MonoBehaviour
{
    public void Ring1()
    {
        Player player = FindObjectOfType<Player>();
        if (player != null)
        {
            player.maxLife += 1; // 최대 체력 증가
            player.life += 1;
            player.UpdateLifeIcon(player.life);
            Debug.Log("최대 체력 1 증가");
        }
    }

    public void Ring2()
    {
        Player player = FindObjectOfType<Player>();
        if (player != null)
        {
            player.gauge.maxValue += 0.2f; // 최대 게이지 증가
            Debug.Log("최대 게이지 0.2 증가");
        }
    }

    public void Ring3()
    {
        // Ring3의 효과 구현
        Debug.Log("Ring3 효과 발동: 공격력 증가");
    }

    public void Ring4()
    {
        // Ring4의 효과 구현
        Debug.Log("Ring4 효과 발동: 공격력 증가");
    }

    public void Ring5()
    {
        // Ring5의 효과 구현
        Debug.Log("Ring5 효과 발동: 공격력 증가");
    }

    public void Bracelet1()
    {
        Attack attack = FindObjectOfType<Attack>();
        if (attack != null)
        {
            attack.whatAttack = "Baracelet1";
        }
        // Bracelet1의 효과 구현
        Debug.Log("Bracelet1 효과 발동: 기본 공격으로 변경");
    }

    public void Bracelet2()
    {
        Attack attack = FindObjectOfType<Attack>();
        if (attack != null)
        {
            attack.whatAttack = "Baracelet2";
        }
        Debug.Log("Bracelet2 효과 발동: 레이저 공격으로 변경");
    }

    public void Bracelet3()
    {
        // Bracelet3의 효과 구현
        Debug.Log("Bracelet3 효과 발동: 속도 증가");
    }

    public void Nail1()
    {
        Player player = FindObjectOfType<Player>();
        if (player != null)
        {
            player.isEquippedSkill[0] = true;
        }
    }

    public void Nail2()
    {
        Player player = FindObjectOfType<Player>();
        if (player != null)
        {
            player.isEquippedSkill[1] = true;
        }
    }
}
