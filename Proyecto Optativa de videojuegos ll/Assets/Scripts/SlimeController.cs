using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeController : MonoBehaviour
{
    [SerializeField] private int _life;
    [SerializeField] private int _points;
    [SerializeField] private int _enemyDamage;
    [SerializeField] private Color[] _posibleColors;
    [SerializeField] private string[] _colorName;
    [SerializeField] private float _spawnItemProbability;

    //Item array that enemy can spawn
    [SerializeField] private GameObject[] _items;

    //Audio Components
    [SerializeField] private AudioClip _hitClip;
    private AudioSource _enemyAudioSource;

    //Invincibility if the enemy has a large hp
    private bool _invincibility;

    //The gameController is needed to call the function of points
    private GameController _gameController;

    //Components of the enemy
    private Rigidbody2D _rigidbody;
    private Animator _animator;
    private SpriteRenderer _spriteRenderer;

    //The players to detect
    private GameObject _alexPlayer;
    private GameObject _rufusPlayer;

    private string _enemyColorName;
    private Color _enemyColor;
    private string _playerToChase;
    private float _speed;

    void Start()
    {
        //Set the color of the slime
        SetColor();

        _invincibility = false;
        _speed = 5;

        //Get components of the enemy
        _rigidbody = GetComponent<Rigidbody2D>();
        _enemyAudioSource = GetComponent<AudioSource>();
        
        //Get the Game Controller component if the player destroys the slime
        _gameController = Camera.main.GetComponent<GameController>();

        //Detect the players
        _alexPlayer = GameObject.Find("Alex");
        _rufusPlayer = GameObject.Find("Rufus");

        InvokeRepeating("Movement", 0, 2f);
    }

    void Update()
    {
        if(_gameController.isGameOver())
        {
            CancelInvoke("Movement");
            _rigidbody.velocity = Vector2.zero;
        }
    }

    void Movement()
    {
        //Set a random number
        float _chaseProbability = Random.Range(0, 100);
        //The number transforms in a probability to chase one player or another one
        //Chase Alex
        if (_chaseProbability < 50)
        {
            _rigidbody.velocity = (_alexPlayer.transform.position - transform.position) * Time.deltaTime * _speed;
            _playerToChase = "Alex";
        }
        //Chase Rufus
        else
        {
            _rigidbody.velocity = (_rufusPlayer.transform.position - transform.position) * Time.deltaTime * _speed;
            _playerToChase = "Rufus";
        }

        //Set scale of the enemy
        if ((transform.position.x > _alexPlayer.transform.position.x && _playerToChase ==  "Alex") 
            || (transform.position.x > _rufusPlayer.transform.position.x && _playerToChase == "Rufus"))
            transform.localScale = new Vector3(1, 1, 1);
        else
            transform.localScale = new Vector3(-1, 1, 1);
    }

    void SetColor()
    {
        //Set a random color to the sprite
        int _colorIndex = Random.Range(0, _posibleColors.Length);
        GetComponent<SpriteRenderer>().color = _posibleColors[_colorIndex];
        _enemyColor = _posibleColors[_colorIndex];
        _enemyColorName = _colorName[_colorIndex];
    }

    //Set enemy audio clip according to the player actions
    void SetAudioClip(AudioClip _clip)
    {
        _enemyAudioSource.clip = _clip;
        _enemyAudioSource.Play();
    }

    void OnTriggerEnter2D(Collider2D _other)
    {
        if(!_invincibility && !_gameController.isGameOver() && _other.gameObject.tag != "Enemy" && _other.gameObject.tag != "Untagged"
            && _other.gameObject.tag != "Item" && _other.gameObject.tag != "Dash")
        {
            //Check if slime was attacked by player sword
            if(_other.gameObject.tag == "Attack")
            {
                _life -= 2;
                if(_life < 1)
                    _gameController.SetPlayerPoints(_enemyColorName, _other.transform.parent.name, _points);
            }
            
            //Check if slime was attacked by player aura
            if(_other.gameObject.tag == "Aura")
            {
                _life -= 3;
                if(_life < 1)
                {
                    _other.transform.parent.GetComponent<CharacterController>().SetPoints(_points);
                    _gameController.SetPlayerPoints(_other.transform.parent.name, _points);
                }
            }

            _invincibility = true;
            if (_life > 0)
            {
                SetAudioClip(_hitClip);
                GetComponent<SpriteRenderer>().color = new Color(_enemyColor.r, _enemyColor.g, _enemyColor.b, 0.5f);
                Invoke("EndInvincibility", 0.5f);
            }
            else
            {
                Instantiate((GameObject) Resources.Load("Prefab/SlimeDestroy", typeof(GameObject)), transform.position, Quaternion.identity);
                if(Random.Range(0, 100) <= _spawnItemProbability)
                {
                    SpawnRandomItem();
                }
                Destroy(gameObject);
            }
        }
    }

    void OnTriggerStay2D(Collider2D _other)
    {
        if(_other.gameObject.tag == "Aura")
        {
            _life--;
            _invincibility = true;
            if (_life > 0)
            {
                SetAudioClip(_hitClip);
                GetComponent<SpriteRenderer>().color = new Color(_enemyColor.r, _enemyColor.g, _enemyColor.b, 0.5f);
                Invoke("EndInvincibility", 0.5f);
            }
            else
            {
                Instantiate((GameObject)Resources.Load("Prefab/SlimeDestroy", typeof(GameObject)), transform.position, Quaternion.identity);
                if (Random.Range(0, 100) <= _spawnItemProbability)
                {
                    SpawnRandomItem();
                }
                Destroy(gameObject);
            }
        }
    }

    void EndInvincibility()
    {
        GetComponent<SpriteRenderer>().color = new Color(_enemyColor.r, _enemyColor.g, _enemyColor.b, 1);
        _invincibility = false;
    }

    void SpawnRandomItem()
    {
        int _itemIndex = Random.Range(0, _items.Length);
        Instantiate(_items[_itemIndex], transform.position, Quaternion.identity, transform.parent);
    }

    public int GetDamage()
    {
        return _enemyDamage;
    }

    public void SetGameControllerColor()
    {
        _enemyColorName = _gameController.GetColorName();
        _enemyColor = _gameController.GetColor();
        GetComponent<SpriteRenderer>().color = _enemyColor;
        _rigidbody.velocity = Vector2.zero;
    }

    public void SetSpeed(float _speed)
    {
        this._speed = _speed;
    }

    public void SetInvincibility(bool _invincibility)
    {
        this._invincibility = _invincibility;
        if(_invincibility)
            GetComponent<SpriteRenderer>().color = new Color(_enemyColor.r, _enemyColor.g, _enemyColor.b, 0.3f);
        else
            GetComponent<SpriteRenderer>().color = new Color(_enemyColor.r, _enemyColor.g, _enemyColor.b, 1);
    }

    public void IncreaseHP()
    {
        _life += 2;
    }
}
