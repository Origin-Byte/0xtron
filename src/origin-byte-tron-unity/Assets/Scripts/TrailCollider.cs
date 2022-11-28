using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EdgeCollider2D))]
public class TrailCollider : MonoBehaviour
{
    public string ownerAddress;
    public bool IsLocalPlayerTrail;
    public EdgeCollider2D EdgeCollider;

    private ulong _lastSyncedSequenceNumber;
    private List<Vector2> _colliderPoints;
 
    public void Start()
    {
        if (string.IsNullOrWhiteSpace(ownerAddress))
        {
            ownerAddress = SuiWallet.GetActiveAddress();
            IsLocalPlayerTrail = true;
        }
        EdgeCollider = GetComponent<EdgeCollider2D>();
        EdgeCollider.enabled = false;
        _colliderPoints = new List<Vector2>();
        _lastSyncedSequenceNumber = 0;
    }

    void FixedUpdate()
    {
        if (OnChainStateStore.Instance.States.ContainsKey(ownerAddress))
        {
            var playerState = OnChainStateStore.Instance.States[ownerAddress];

            if (playerState.SequenceNumber > _lastSyncedSequenceNumber)
            {
                // TODO optimization: merge points over the same line
                var position = playerState.Position;

                // don't allow diagonal collider edges, add a corner point if required
                var posVector2 = position.ToVector2();
                if (_colliderPoints.Count > 0 
                    && !Mathf.Approximately(posVector2.x, _colliderPoints[^1].x) 
                    && !Mathf.Approximately(posVector2.y, _colliderPoints[^1].y))
                {
                    //Debug.Log("Adding corner point");
                    var velocityVector2 = playerState.Velocity.ToVector2();

                    var cornerVector = new Vector2();
                    if (Mathf.Abs(velocityVector2.x) > 0f)
                    {
                        cornerVector.x = _colliderPoints[^1].x;
                        cornerVector.y = posVector2.y;
                    }
                    else
                    {
                        cornerVector.x = posVector2.x;
                        cornerVector.y = _colliderPoints[^1].y;
                    }
                    
                    _colliderPoints.Add(cornerVector);
                    
                    // GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    // cube.transform.position = new Vector3(cornerVector.x, cornerVector.y) + Vector3.back;
                    // cube.transform.localScale *= 0.5f;
                    // cube.GetComponent<BoxCollider>().enabled = false;
                    // cube.GetComponent<Renderer>().material.color = Color.red;
                }
                
                _colliderPoints.Add(posVector2);

                //Debug.Log($"DrawCube: {position.ToVector3()}. sequenceNumber: {playerState.SequenceNumber}. sender: {ownerAddress}. ");
                // GameObject cube2 = GameObject.CreatePrimitive(PrimitiveType.Cube);
                // cube2.transform.position = position.ToVector3() + Vector3.back;
                // cube2.transform.localScale *= 0.5f;
                // cube2.GetComponent<BoxCollider>().enabled = false;

                EdgeCollider.SetPoints(_colliderPoints);

                if (!EdgeCollider.enabled && _colliderPoints.Count >= 2)
                {
                    EdgeCollider.enabled = true;
                }
                
                _lastSyncedSequenceNumber = playerState.SequenceNumber;

                if (playerState.IsExploded && !IsLocalPlayerTrail)
                {
                    Destroy(gameObject);
                }
            }
        }
        else if (!IsLocalPlayerTrail)
        {
            Destroy(gameObject);
        }
    }
}
