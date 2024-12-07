using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDIndicator : MonoBehaviour
{
    [SerializeField] private RectTransform indicator; // 화살표 UI
    [SerializeField] private Camera mainCamera;       // 메인 카메라
    [SerializeField] private RectTransform canvasRect; // UI 캔버스 RectTransform
    [SerializeField] private Transform boss;          // 보스 Transform
    [SerializeField] private Image indicatorImage; // 화살표 이미지

    // Update is called once per frame
    void Update()
    {
        Vector3 screenPoint = mainCamera.WorldToViewportPoint(boss.position);

        // 보스가 화면 안에 있으면 화살표 숨기기
        if (screenPoint.x >= 0f && screenPoint.x <= 1f && screenPoint.y >= 0f && screenPoint.y <= 1f && screenPoint.z > 0)
        {
            indicator.gameObject.SetActive(false);
            return;
        }

        indicator.gameObject.SetActive(true);

        // 화면 밖 위치 계산
        Vector2 indicatorPosition = GetIndicatorPosition(screenPoint);
        indicator.anchoredPosition = indicatorPosition;

        // 보스 방향으로 회전
        Vector3 bossDirection = boss.position - mainCamera.transform.position;
        indicatorImage.rectTransform.localScale = new Vector3(
            bossDirection.x > 0 ? 1 : -1, // 오른쪽 방향이면 그대로, 왼쪽 방향이면 뒤집기
            bossDirection.y > 0 ? 1 : -1, // 위쪽 방향이면 그대로, 아래쪽 방향이면 뒤집기
            1
        );
    }

    private Vector2 GetIndicatorPosition(Vector3 screenPoint)
    {
        // 화면의 경계 바깥으로 화살표 위치 보정
        float x = Mathf.Clamp(screenPoint.x, 0.05f, 0.95f);
        float y = Mathf.Clamp(screenPoint.y, 0.05f, 0.95f);

        // UI 캔버스 내 위치로 변환
        Vector2 viewportPosition = new Vector2(x, y);
        Vector2 worldPositionOnCanvas = new Vector2(
            (viewportPosition.x - 0.5f) * canvasRect.sizeDelta.x,
            (viewportPosition.y - 0.5f) * canvasRect.sizeDelta.y
        );

        return worldPositionOnCanvas;
    }
}
