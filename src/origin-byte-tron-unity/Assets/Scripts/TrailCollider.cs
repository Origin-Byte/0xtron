using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(EdgeCollider2D))]
public class TrailCollider : MonoBehaviour
{
    public string ownerAddress;
    
    // Edgecollider needs 2 starting points
    public List<Vector2> startPoints = new List<Vector2>();
    
    private ulong _lastSyncedSequenceNumber = 0;
    public EdgeCollider2D EdgeCollider;
    private List<Vector2> _points;
    public bool IsLocalPlayerTrail;
    
    public void Start()
    {
        if (string.IsNullOrWhiteSpace(ownerAddress))
        {
            ownerAddress = SuiWallet.GetActiveAddress();
            IsLocalPlayerTrail = true;
        }
        EdgeCollider = GetComponent<EdgeCollider2D>();
        EdgeCollider.SetPoints(startPoints);
        _points = EdgeCollider.points.ToList();
        EdgeCollider.enabled = false;
    }

    void FixedUpdate()
    {
        if (OnChainStateStore.Instance.States.ContainsKey(ownerAddress))
        {
            var playerState = OnChainStateStore.Instance.States[ownerAddress];

           // if (playerState.SequenceNumber > _lastSyncedSequenceNumber)
            {
                if (playerState.SequenceNumber == 0)
                {
                    _points = startPoints;
                }

                if (!EdgeCollider.enabled && _points.Count > 2)
                {
//                    Debug.Log("enable edgecollider" );
                    EdgeCollider.enabled = true;
                }
                
                // TODO optimization: merge points over the same line
                var position = playerState.Position;
                _points.Add(position.ToVector2());
                
              //  Debug.Log($"DrawCube: {position.ToVector3()}. sequenceNumber: {sequenceNumber}. sender: {sender}. isExploded:{ isExploded} ");
               // GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
              //  cube.transform.position = position.ToVector3() + Vector3.back;
                
                EdgeCollider.SetPoints(_points);
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
