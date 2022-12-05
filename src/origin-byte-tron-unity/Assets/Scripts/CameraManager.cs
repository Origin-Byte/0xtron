using Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public CinemachineVirtualCamera topDownVirtualCamera;
    public CinemachineVirtualCamera followVirtualCamera;
    private bool _isTopDownCameraMode;
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            _isTopDownCameraMode = !_isTopDownCameraMode;
            var topDownCameraPriority = 1;
            var followCameraPriority = 2;
            if (_isTopDownCameraMode)
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
