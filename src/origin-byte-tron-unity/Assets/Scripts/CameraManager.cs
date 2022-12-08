using System;
using Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public CinemachineVirtualCamera topDownVirtualCamera;
    public CinemachineVirtualCamera followVirtualCamera;
    public static bool IsTopDownCameraMode { private set; get; }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            IsTopDownCameraMode = !IsTopDownCameraMode;

            var topDownCameraPriority = 1;
            var followCameraPriority = 2;
            if (IsTopDownCameraMode)
            {
                topDownCameraPriority = 3;
            }

            if (topDownVirtualCamera != null)
            {
                topDownVirtualCamera.Priority = topDownCameraPriority;
            }

            if (followVirtualCamera != null)
            {
                followVirtualCamera.Priority = followCameraPriority;
            }
        }
    }
}
