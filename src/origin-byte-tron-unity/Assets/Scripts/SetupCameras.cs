using Cinemachine;
using UnityEngine;

public class SetupCameras : MonoBehaviour
{
    public Transform worldUpOverride;
    
    void Start()
    {
        var virtualCamera = GameObject.FindWithTag("VirtualCamera").GetComponent<CinemachineVirtualCamera>();
        virtualCamera.Follow = transform;
        virtualCamera.LookAt = transform;

        var cinemachineBrain = GameObject.FindWithTag("MainCamera").GetComponent<CinemachineBrain>();
        cinemachineBrain.m_WorldUpOverride = worldUpOverride;
        
        virtualCamera = GameObject.FindWithTag("TopDownVirtualCamera").GetComponent<CinemachineVirtualCamera>();
        virtualCamera.Follow = transform;
        
        virtualCamera = GameObject.FindWithTag("MinimapCamera").GetComponent<CinemachineVirtualCamera>();
        virtualCamera.Follow = transform;
    }
}
