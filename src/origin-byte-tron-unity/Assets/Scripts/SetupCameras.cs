using Cinemachine;
using UnityEngine;

public class SetupCameras : MonoBehaviour
{
    public Transform worldUpOverride;
    
    void Start()
    {
        CinemachineVirtualCamera virtualCamera =
            GameObject.FindWithTag("VirtualCamera").GetComponent<CinemachineVirtualCamera>();

        virtualCamera.Follow = transform;
        virtualCamera.LookAt = transform;

        CinemachineBrain cinemachineBrain = GameObject.FindWithTag("MainCamera").GetComponent<CinemachineBrain>();
        cinemachineBrain.m_WorldUpOverride = worldUpOverride;
    }
}
