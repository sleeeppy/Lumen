using UnityEngine;

public class ItemLogic : MonoBehaviour
{
    private int originalMaxLife = 0; // 원래 최대 체력 저장
    private float originalMaxValue;

    public void Ring1()
    {
        Player player = FindObjectOfType<Player>();
        if (player != null)
        {
            if (originalMaxLife == 0) // 처음 적용할 때만 저장
            {
                originalMaxLife = player.maxLife; // 원래 최대 체력 저장
                player.maxLife += 2; // 최대 체력 증가
                player.life += 2;
                player.UpdateLifeIcon(player.life);
                Debug.Log("최대 체력 2 증가");
            }
        }
    }

    public void UnEquipRing1()
    {
        Player player = FindObjectOfType<Player>();
        if (player != null)
        {
            if (originalMaxLife != 0)
            {
                player.maxLife = originalMaxLife; // 원래 최대 체력으로 되돌리기
                originalMaxLife = 0; // 원래 값 초기화
                player.life -= 2;
                player.UpdateLifeIcon(player.life);
                Debug.Log("최대 체력 원래대로 복원");
            }
        }
    }

    public void Ring2()
    {
        Player player = FindObjectOfType<Player>();
        if (player != null)
        {
            if(originalMaxValue == 0)
            {
                originalMaxValue = player.gauge.maxValue; // 원래 최대 게이지 저장
                player.gauge.maxValue += 0.2f; // 최대 게이지 증가
                Debug.Log("최대 게이지 0.2 증가");
            }
        }
    }

    public void UnEquipRing2()
    {
        Player player = FindObjectOfType<Player>();
        if (player != null)
        { 
            if(originalMaxValue != 0) // 원래 최대 값이 0이 아닐 때만 복원
            {
                player.gauge.maxValue = originalMaxValue; // 원래 최대 게이지로 되돌리기
                originalMaxValue = 0; // 원래 값 초기화
                Debug.Log("최대 게이지 원래대로 복원");
            }
        }
    }

    public void Ring3()
    {
        // Ring3의 효과 구현
        Debug.Log("Ring3 효과 발동: 공격력 증가");
    }

    public void UnEquipRing3()
    {

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
        // Bracelet1의 효과 구현
        Debug.Log("Bracelet1 효과 발동: 속도 증가");
    }

    public void UnEquipBracelet1()
    {

    }

    public void Bracelet2()
    {
        // Bracelet2의 효과 구현
        Debug.Log("Bracelet2 효과 발동: 속도 증가");
    }

    public void Bracelet3()
    {
        // Bracelet3의 효과 구현
        Debug.Log("Bracelet3 효과 발동: 속도 증가");
    }

    public void Nail1()
    {
        // Nail1의 효과 구현
        Debug.Log("Nail1 효과 발동: 추가 마나 제공");
    }

    public void Nail2()
    {
        // Nail2의 효과 구현
        Debug.Log("Nail2 효과 발동: 체력 회복");
    }
}
