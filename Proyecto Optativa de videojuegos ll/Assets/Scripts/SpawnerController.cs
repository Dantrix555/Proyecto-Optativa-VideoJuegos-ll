using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerController : MonoBehaviour
{
    [SerializeField] private GameObject[] _slimesGameObject;

    private GameController _gameController;
    private GameObject _slimeInstance;
    private List<GameObject> _waveSlimes = new List<GameObject>();
    private List<float> _speedWaveSlimes = new List<float>();

    void Start()
    {
        _gameController = Camera.main.GetComponent<GameController>();
        _waveSlimes = new List<GameObject>();
        _speedWaveSlimes = new List<float>();
    }

    void SpawnSlimes()
    {
        //Select a random slime and instantiate it
        int _slimeIndex = Random.Range(0, _waveSlimes.Count);
        _slimeInstance = Instantiate(_waveSlimes[_slimeIndex], transform.position, Quaternion.identity, transform);
        _slimeInstance.GetComponent<SlimeController>().SetSpeed(_speedWaveSlimes[_slimeIndex]);
    }

    public void SlimeCharacteristics(int _slimeIndex, float _slimeSpeed)
    {
        _waveSlimes.Add(_slimesGameObject[_slimeIndex]);
        _speedWaveSlimes.Add(_slimeSpeed);
    }

    public void ClearSpawnArrays()
    {
        int i;
        for(i = 0; i < _waveSlimes.Count; i++)
        {
            _waveSlimes.RemoveAt(i);
            _speedWaveSlimes.RemoveAt(i);
        }
    }

    public void GameOver()
    {
        _gameController.SetGameOver();
        Destroy(transform.parent.gameObject);
    }
}
