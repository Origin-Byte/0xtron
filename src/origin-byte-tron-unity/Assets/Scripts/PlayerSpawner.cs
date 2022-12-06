using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerSpawner : MonoBehaviour
{
    public Transform playersParent;
    public Transform trailColliderParent;
    public Transform explosionsParent;

    public LocalPlayer playerPrefab;
    public TrailCollider trailColliderPrefab;

    public void SpawnPlayer()
    {
        var player = Instantiate(playerPrefab, playersParent); 
        var explosionController =  player.GetComponent<ExplosionController>();
        explosionController.explosionRoot = explosionsParent;

        var randomPosition = GetRandomPosition();
        
        var trailCollider = Instantiate(trailColliderPrefab, trailColliderParent);
       // trailCollider.startPoints.Add(randomPosition);
      //  trailCollider.startPoints.Add(randomPosition + Vector2.up*0.001f);

        explosionController.objectsToDestroy.Add(player.gameObject);
        explosionController.objectsToDestroy.Add(trailCollider.gameObject);
        explosionController.objectsToSetInactive.Add(trailCollider.gameObject);
        explosionController.objectsToSetChildrenInactive.Add(player.gameObject);
        
        player.transform.position = randomPosition;
        player.gameObject.SetActive(true);
        trailCollider.gameObject.SetActive(true);
    }

    private Vector2 GetRandomPosition()
    {
        // for testing
        const int MAX_POSITION_VALUE = 120;
        //const int MAX_POSITION_VALUE = 400;
        return new Vector2(Random.Range(-MAX_POSITION_VALUE, MAX_POSITION_VALUE),
            Random.Range(-MAX_POSITION_VALUE, MAX_POSITION_VALUE));
    }
}
