using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    [SerializeField] private GameObject firePos;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField, Range(0, 100)] private float distance;
    [SerializeField] private float bulletCoolTime;
    [SerializeField] private float bulletForce = 10f;
    private Player PlayerScript;
    private float curTime;

    void Start()
    {
        PlayerScript = gameObject.GetComponent<Player>();
    }

    void Update()
    {
        curTime += Time.deltaTime;

        FirePosCalculator();

        if (curTime >= bulletCoolTime)
            Fire();

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
        bullet.transform.position = firePos.transform.position;

        Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();

        Vector2 targetPosition = firePos.transform.position;
        rigid.AddForce((targetPosition - (Vector2)transform.position).normalized * bulletForce, ForceMode2D.Impulse);

        curTime = 0;
    }
}
