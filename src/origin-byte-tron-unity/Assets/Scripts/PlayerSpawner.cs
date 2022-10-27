using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerSpawner : MonoBehaviour
{
    public Transform playersParent;
    public Transform trailColliderParent;
    public Transform explosionsParent;

    public LocalPlayer playerPrefab;
    public TrailCollider trailColliderPrefab;

    private void Awake()
    {
        SpawnPlayer();
    }

    private void SpawnPlayer()
    {
        var player = Instantiate(playerPrefab, playersParent);
        player.GetComponent<ExplosionController>().explosionRoot = explosionsParent;
        player.transform.position = GetRandomPosition();
        player.gameObject.SetActive(true);
        
        var trailColliderGo =  Instantiate(trailColliderPrefab, trailColliderParent);
        trailColliderGo.gameObject.SetActive(true);
    }

    private Vector2 GetRandomPosition()
    {
        // for testing
        const int MAX_POSITION_VALUE = 30;
        //const int MAX_POSITION_VALUE = 400;
        return new Vector2(Random.Range(-MAX_POSITION_VALUE, MAX_POSITION_VALUE),
            Random.Range(-MAX_POSITION_VALUE, MAX_POSITION_VALUE));
    }
}
