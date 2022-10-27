using UnityEngine;

public class ActivateChildrenOnDisable : MonoBehaviour
{
    public string[] parentTags;

    private void OnDisable()
    {
        foreach (var tag in parentTags)
        {
            var go = GameObject.FindWithTag(tag);
            if (go != null)
            {
                for(var i=0; i< go.transform.childCount; i++)
                {
                    go.transform.GetChild(i).gameObject.SetActive(true);
                }
            }
        }
    }
}
