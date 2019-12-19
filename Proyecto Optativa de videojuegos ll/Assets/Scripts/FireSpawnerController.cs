using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireSpawnerController : MonoBehaviour
{
    [SerializeField] private GameObject _fireGameObject;
    [SerializeField] private Transform _fireSpawnerTransform;
    private float _fireSpawnProbability;

    void Start()
    {
        _fireSpawnProbability = 65;
        InvokeRepeating("SpawnDice", 0, 10);
    }

    void SpawnDice()
    {
        if(_fireSpawnProbability <= Random.RandomRange(0, 100))
        {
            GameObject _fireObjectInstance;
            _fireObjectInstance = Instantiate(_fireGameObject, _fireSpawnerTransform.position, Quaternion.identity);
            _fireObjectInstance.GetComponent<FireController>().SetFireVelocityAndDirection(transform.localScale.x, 8);
        }
    }
}
