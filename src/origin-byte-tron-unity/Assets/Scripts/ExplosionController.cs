using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class ExplosionController : MonoBehaviour
{
    public List<GameObject> objectsToSetInactive = new List<GameObject>();
    public List<GameObject> objectsToSetChildrenInactive = new List<GameObject>();
    public List<GameObject> objectsToDestroy = new List<GameObject>();
    public Transform explosionRoot;
    public GameObject explosionPrefab;
    public float initialCollisionDetectionDelay;
    public bool useCollisionDetection;
    public float destroyAfterSeconds = 2f;
    
    private bool _isCollisionDetectionEnabled;
    
    public bool IsExploded { get; private set; }

    public void Start()
    {
        _isCollisionDetectionEnabled = false;
        if (useCollisionDetection)
        {
            StartCoroutine(EnableDetectionAfterDelay(initialCollisionDetectionDelay));
        }
    }

    private IEnumerator EnableDetectionAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        _isCollisionDetectionEnabled = true;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (_isCollisionDetectionEnabled)
        {
            Explode();
        }
    }

    public void Explode()
    {
        //Debug.Log("Explode");
        var explosionEffect = Instantiate(explosionPrefab, explosionRoot);
        explosionEffect.transform.position = transform.position;
        explosionEffect.gameObject.SetActive(true);
        IsExploded = true;

        // TODO rework explosion to event based
        StartCoroutine(SetInactiveAfter(destroyAfterSeconds));
    }

    private IEnumerator SetInactiveAfter(float delay)
    {
        foreach (var objectToSetChildrenInactive in objectsToSetChildrenInactive)
        {
            for(var i=0; i<objectToSetChildrenInactive.transform.childCount; i++)
            {
                objectToSetChildrenInactive.transform.GetChild(i).gameObject.SetActive(false);
            }
        }
        
        foreach (var objectToSetInactive in objectsToSetInactive)
        {
            objectToSetInactive.SetActive(false);
        }
        
        yield return new WaitForSeconds(delay);

        foreach (var objectToDestroy in objectsToDestroy)
        {
            Destroy(objectToDestroy);
        }
    }
}
