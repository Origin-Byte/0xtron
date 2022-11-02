using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class OnChainPlayer : MonoBehaviour
{
    public string ownerAddress;
    public Transform trailRendererParent;
    
    private Rigidbody2D _rb;
    private ulong _lastSyncedSequenceNumber = 0;
    private ExplosionController _explosionController;
    private bool _activatedRenderer;
    
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _explosionController = GetComponent<ExplosionController>();
        _activatedRenderer = false;
    }

    void FixedUpdate()
    {
        if (OnChainStateStore.Instance.States.ContainsKey(ownerAddress))
        {
            var playerState = OnChainStateStore.Instance.States[ownerAddress];
  
            if (playerState.SequenceNumber != _lastSyncedSequenceNumber || playerState.SequenceNumber == 0)
            {
                if (!_activatedRenderer && playerState.SequenceNumber > 0)
                {
                    trailRendererParent.gameObject.SetActive(true);
                    _activatedRenderer = true;
                }
                
                var onChainPosition = playerState.Position.ToVector2();
                var onChainVelocity = playerState.Velocity.ToVector2();

                if (onChainVelocity != _rb.velocity)
                {
                    _rb.velocity = onChainVelocity;
                    _rb.position = onChainPosition;
                    transform.rotation = Quaternion.Euler(new Vector3(0,0, Vector2.Angle(Vector2.up, _rb.velocity)));
                }

                if (playerState.IsExploded)
                {
                    OnChainStateStore.Instance.RemoveRemotePlayer(ownerAddress);
                    _explosionController.Explode();
                    Destroy(gameObject);
                }

                _lastSyncedSequenceNumber = playerState.SequenceNumber;
            }
        }
    }
}
