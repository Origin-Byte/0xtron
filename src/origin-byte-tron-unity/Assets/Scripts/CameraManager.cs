using Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public GameObject camera;
    private Transform _defaultTransform;
    private bool _is2dCamera;
    private CinemachineBrain _brain;
    
    void Awake()
    {
        _defaultTransform = camera.transform;
        _brain = camera.GetComponent<CinemachineBrain>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            if (_is2dCamera)
            {
                _brain.enabled = true;
            }
            else
            {
                _brain.enabled = false;
                camera.transform.position = _defaultTransform.position;
                camera.transform.rotation = _defaultTransform.rotation;
            }

            _is2dCamera = !_is2dCamera;
        }
    }
}
