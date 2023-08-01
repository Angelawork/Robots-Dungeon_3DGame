using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance {get;private set;}

    private CinemachineVirtualCamera camera;
    private float ShakeIntensity = 1.3f;
    private float Startintensity;
    private float ShakeTime = 0.3f;
    private float shaketimerTotal;
    private float Timer;
    private CinemachineBasicMultiChannelPerlin _cbmcp;

    private void Awake() {
        Instance = this;
        camera = GetComponent<CinemachineVirtualCamera>();
    }
    void Start()
    {
        StopShake();
    }

    // Update is called once per frame
    void Update()
    {
        if(Timer>0){
            Timer -=Time.deltaTime;
            CinemachineBasicMultiChannelPerlin _cbmcp = camera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            _cbmcp.m_AmplitudeGain = Mathf.Lerp(Startintensity, 0f,1 - (Timer / shaketimerTotal));
        }
    }
    public void ShakeCamera(){
        CinemachineBasicMultiChannelPerlin _cbmcp = camera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        _cbmcp.m_AmplitudeGain = ShakeIntensity;

        Startintensity = ShakeIntensity;
        shaketimerTotal = ShakeTime;
        Timer = ShakeTime;
    }
    void StopShake(){
        CinemachineBasicMultiChannelPerlin _cbmcp = camera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        _cbmcp.m_AmplitudeGain = 0f;

        Timer = 0;
    }
}
