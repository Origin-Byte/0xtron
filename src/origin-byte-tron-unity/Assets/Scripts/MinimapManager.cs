using UnityEngine;

public class MinimapManager : MonoBehaviour
{
    public GameObject minimap;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            if (minimap != null)
            {
                minimap.SetActive(!minimap.activeSelf);
            }
        }
    }
}
