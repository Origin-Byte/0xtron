using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugDrawEdgeCollider : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public TrailCollider trailCollider;

    private LocalPlayer localPlayer;
    
    // Start is called before the first frame update
    void Start()
    {
        if (trailCollider.IsLocalPlayerTrail)
        {
            localPlayer = GameObject.FindObjectOfType<LocalPlayer>();
            lineRenderer.enabled = true;
            StartCoroutine(RenderEdgeCollider());
        }
    }

    IEnumerator RenderEdgeCollider()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.25f);

           // Debug.Log("trailCollider._edgeCollider.points.Count: " + trailCollider._edgeCollider.points.Length);
            
            var newPoints = new Vector3[trailCollider.EdgeCollider.points.Length+1];
            for(int i = 0; i < trailCollider.EdgeCollider.points.Length; i++)
            {
                var point = trailCollider.EdgeCollider.points[i];
                newPoints[i] = new Vector3(point.x, point.y, 0f);
            }

            newPoints[^1] = localPlayer.transform.position;
            
            lineRenderer.positionCount = newPoints.Length;
            lineRenderer.SetPositions(newPoints);

          //  Debug.Log("  lineRenderer.positionCount: " + lineRenderer.positionCount);
        }
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }
}
