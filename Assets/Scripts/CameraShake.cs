using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System.Threading;

public class CameraShake : MonoBehaviour
{
    public static CameraShake instance;
    private CinemachineVirtualCamera cinemachineVirtualCamera = null;
    [SerializeField] private float shakeIntensity = 3f;
    [SerializeField] private float shakeTime = 0.2f;

    private float shakeTimer;
    private CinemachineBasicMultiChannelPerlin cbmcp = null;

    void Awake()
    {

        if (instance == null)
            instance = this;
        else
            Destroy(this);

        cinemachineVirtualCamera = GetComponent<CinemachineVirtualCamera>();
        cbmcp = cinemachineVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        StopShake();
    }

    void Update()
    {
        // if (Input.GetKey(KeyCode.Space))
        //     ShakeCamera();

        if(shakeTimer > 0)
        {
            shakeTimer -= Time.deltaTime;

            if (shakeTimer <= 0)
                StopShake();
        }
    }

    public void ShakeCamera()
    {
        cbmcp.m_AmplitudeGain = shakeIntensity;

        shakeTimer = shakeTime;
    }

    private void StopShake()
    {
        cbmcp.m_AmplitudeGain = 0f;
        shakeTimer = 0f;
    }
}
