using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    [SerializeField] private GameObject firePos;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private GameObject laserPrefab;
    [SerializeField, Range(0, 100)] private float distance;
    [SerializeField] private float bulletCoolTime;
    [SerializeField] private float bulletForce = 10f;
    private Player PlayerScript;
    private float curTime;

    private GameObject currentLaser; // 현재 레이저 오브젝트를 저장할 변수

    [HideInInspector] public string whatAttack;

    private bool wasMousePressedWhileFlying = false; // 날고 있을 때 마우스가 눌렸는지 추적하는 변수

    void Start()
    {
        PlayerScript = gameObject.GetComponent<Player>();
    }

    void Update()
    {
        curTime += Time.deltaTime;

        FirePosCalculator();

        if (curTime >= bulletCoolTime)
        {
            if (whatAttack == "Baracelet1")
                Fire();
        }

        if (whatAttack == "Baracelet2")
            FireLaser();

        // 레이저 방향 업데이트
        if (currentLaser != null)
        {
            UpdateLaserDirection();
        }
    }

    void FirePosCalculator()
    {
        // 마우스 위치를 화면 좌표로 가져옴
        Vector3 mousePos = Input.mousePosition;

        // Z축 값을 카메라와의 거리로 설정
        mousePos.z = Camera.main.transform.position.z * -1;

        // 화면 좌표를 월드 좌표로 변환
        Vector2 worldPosition = Camera.main.ScreenToWorldPoint(mousePos);
        //Debug.Log($"{worldPosition}");

        var position = transform.position;

        // worldPosition을 Vector3로 변환
        Vector3 worldPosition3D = new Vector3(worldPosition.x, worldPosition.y, position.z);

        // 마우스와 플레이어 간의 방향 벡터를 계산
        Vector3 direction = (worldPosition3D - position).normalized;

        firePos.transform.position = position + direction * distance;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        if (angle <= -45f && angle >= -125f)
        {
            //Debug.Log($"angle : -125 ~ -45,{angle}");
        }

        // firePos를 마우스 방향으로 회전
        firePos.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }

    void Fire()
    {
        if (!Input.GetMouseButton(0))
            return;

        if (PlayerScript.isFlying)
            return;

        GameObject bullet = Instantiate(bulletPrefab, firePos.transform.position, firePos.transform.rotation);
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        Vector3 direction = (firePos.transform.position - transform.position).normalized;
        direction.z = 0;

        rb.AddForce(direction * bulletForce, ForceMode.Impulse);
        
        // 총알 사거리 제한 
        // Bullet bulletScript = bullet.GetComponent<Bullet>(); // Bullet 스크립트가 있다고 가정
        // bulletScript.SetDistanceLimit(10f); // 10f는 사거리 제한 값입니다.

        curTime = 0;
    }

    void FireLaser()
    {
        if (PlayerScript.isFlying) // 플레이어가 날고 있다면
        {
            if (currentLaser != null)
            {
                Destroy(currentLaser); // 현재 레이저 파괴
                currentLaser = null; // 현재 레이저 변수 초기화
            }
            wasMousePressedWhileFlying = Input.GetMouseButton(0); // 마우스가 눌렸는지 상태 저장
            return; // 레이저 생성하지 않음
        }

        if (wasMousePressedWhileFlying) // 날기 상태가 끝났고 마우스가 눌렸던 경우
        {
            currentLaser = Instantiate(laserPrefab, firePos.transform.position, firePos.transform.rotation);
            UpdateLaserDirection(); // 레이저 방향 초기화
            wasMousePressedWhileFlying = false; // 상태 초기화
        }

        if (Input.GetMouseButtonDown(0)) // 마우스 버튼을 눌렀을 때
        {
            currentLaser = Instantiate(laserPrefab, firePos.transform.position, firePos.transform.rotation);
            UpdateLaserDirection(); // 레이저 방향 초기화
        }
        else if (Input.GetMouseButtonUp(0) && currentLaser != null) // 마우스 버튼을 떼었을 때
        {
            Destroy(currentLaser); // 레이저 파괴
            currentLaser = null; // 현재 레이저 변수 초기화
        }
    }

    void UpdateLaserDirection()
    {
        // 레이저의 방향을 마우스 방향으로 설정
        Vector3 direction = (firePos.transform.position - transform.position).normalized;
        direction.z = 0;

        // 레이저의 위치를 firePos로 업데이트
        currentLaser.transform.position = firePos.transform.position;

        // 마우스 위치를 기준으로 레이저의 회전 설정
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = Camera.main.transform.position.z * -1; // Z축 값을 카메라와의 거리로 설정
        Vector2 worldPosition = Camera.main.ScreenToWorldPoint(mousePos);
        Vector3 targetDirection = (worldPosition - (Vector2)transform.position).normalized;

        float angle = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg; // 각도 계산
        currentLaser.transform.rotation = Quaternion.Euler(-angle, 90, 0); // 마우스 위치를 기준으로 레이저의 회전 설정
    }

    private void OnApplicationPause(bool pauseStatus) 
    {
        if(pauseStatus)
            Destroy(currentLaser);
    }
}
