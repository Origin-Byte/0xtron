using UnityEngine;

public class MinimapManager : MonoBehaviour
{
    public GameObject minimap;
    public Camera minimapCamera;
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            if (minimap != null)
            {
                var isMinimapActive = minimap.activeSelf;
                minimap.SetActive(!isMinimapActive);

                if (minimapCamera != null)
                {
                    minimapCamera.gameObject.SetActive(!isMinimapActive);
                }
            }
        }
    }
}
