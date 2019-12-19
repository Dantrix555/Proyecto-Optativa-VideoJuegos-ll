using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterController : MonoBehaviour
{
    private float _horizontal, _vertical;

    //Character detection and attack variables
    [SerializeField] private bool _playerAlex;
    [SerializeField] private bool _playerRufus;
    [SerializeField] private float _speed;
    [SerializeField] private float _dashImpulse;

    //Dash and special game objects
    [SerializeField] private GameObject _dashGameObject;
    [SerializeField] private GameObject _specialGameObject;
    [SerializeField] private Transform _specialSpawnTransform;

    //Character clip sounds
    [SerializeField] private AudioClip _attackClip;
    [SerializeField] private AudioClip _auraClip;
    [SerializeField] private AudioClip _hitClip;
    [SerializeField] private AudioClip _stunnedClip;

    //Attacks UI images references
    [SerializeField] private GameObject _attackImage;
    [SerializeField] private GameObject _auraImage;
    [SerializeField] private GameObject _dashImage;
    [SerializeField] private GameObject _specialImage;

    //Player components to get
    private Rigidbody2D _rigidbody;
    private Animator _animator;
    private SpriteRenderer _spriteRenderer;
    private AudioSource _playerAudioSource;

    //Player stamina to determine when the player is stunned
    private int _playerStamina;
    private int _points;
    private bool _invincibility;
    private bool _playerIsStunned;
    private bool _playerCanUseAura;
    private bool _playerIsUsingDash;
    private bool _playerActivateSpecial;

    private bool _gamePaused;
    private bool _gameOver;
    private GameController _gameController;

    void Start()
    {
        //Get the player components
        _animator = GetComponent<Animator>();
        _rigidbody = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _playerAudioSource = GetComponent<AudioSource>();

        //Get the Game Controller of the camera Game Object
        _gameController = Camera.main.GetComponent<GameController>();

        _gameOver = false;
        _gamePaused = false;
        _points = 0;
        _playerStamina = 20;
        _invincibility = false;
        _playerCanUseAura = true;
        _playerIsUsingDash = false;
        _playerIsStunned = false;
        _playerActivateSpecial = false;
    }

    void Update()
    {
        //Allow the player the control of the character if the game isn't ended and if isn't stunned
        if(!_gameOver && !_playerIsStunned && !_gamePaused && !_playerIsUsingDash)
        {
            Movement();
        }
        else if(_playerIsStunned || _gameOver || _gamePaused)
        {
            _rigidbody.velocity = Vector2.zero;
            _animator.SetFloat("Speed", 0);
        }
    }

    //Methods for the control specific inputs (look in the folder input and file called controller)
    #region ControlTriggers

        //Set the horizontal and vertical movement acording to control stick position
        void OnMoveHorizontal(InputValue value)
        {
            _horizontal = value.Get<float>() + Input.GetAxis("Horizontal" + gameObject.name);
        }
        void OnMoveVertical(InputValue value)
        {
            _vertical = value.Get<float>() + Input.GetAxis("Vertical" + gameObject.name);
        }

        //Set the normal attack with the down button of the controller
        void OnAttack1()
        {
            if(!_playerIsUsingDash)
            {
                _animator.SetTrigger("Attack");
                SetAudioClip(_attackClip);
                _attackImage.GetComponent<Animator>().SetTrigger("Cooldown");
            }
        }
        
        //Set the aura attack with the left button of the controller
        void OnAttack2()
        {
            if (_playerCanUseAura && !_playerIsUsingDash)
            {
                SetAudioClip(_auraClip);
                _animator.SetTrigger("Aura");
                _auraImage.GetComponent<Animator>().SetTrigger("Cooldown");
                _playerCanUseAura = false;
                //Player can use aura again 10 seconds after animate it
                Invoke("AuraCoolDownEnd", 10);
            }
        }
        void OnAttack3()
        {
            if(!_playerIsUsingDash && _horizontal != 0)
            {
                _dashImage.GetComponent<Animator>().SetTrigger("Cooldown");
                _playerIsUsingDash = true;
                _rigidbody.velocity = new Vector3(_horizontal * _dashImpulse, 0, 0);
                _dashGameObject.SetActive(true);
                Invoke("DashCoolownEnd", 0.5f);
            }
        }
        void OnAttack4()
        {
            if(!_playerActivateSpecial)
            {
                _specialImage.GetComponent<Animator>().SetTrigger("Cooldown");
                _playerActivateSpecial = true;
                _animator.SetTrigger("Attack");
                GameObject _specialSpawn;
                _specialSpawn = Instantiate(_specialGameObject, _specialSpawnTransform.position, Quaternion.identity);
                _specialSpawn.GetComponent<SpecialController>().SetSpecialVelocity(transform.localScale.x);
                Destroy(_specialSpawn, 3);
                Invoke("SpecialCooldownEnd", 60);
            }
        }

        void OnPause()
        {
            _gamePaused = !_gamePaused;
            _gameController.SetPauseState(_gamePaused);
        }
    #endregion

    void Movement()
    {
        //Set the velocity of the player according to the controller inputs
        _rigidbody.velocity = new Vector2(_horizontal, _vertical) * _speed;
        _animator.SetFloat("Speed", Mathf.Abs(_horizontal) + Mathf.Abs(_vertical));

        //Set the scale
        if (_horizontal > 0)
            transform.localScale = new Vector3(-1, 1, 1);
        else if (_horizontal < 0)
            transform.localScale = new Vector3(1, 1, 1);
    }
    
    //Finish the invincibility of the player
    void InvincibilityEnd()
    {
        _invincibility = false;
        _spriteRenderer.color = new Color(1, 1, 1, 1);
    }

    //Finish the aura cooldown
    void AuraCoolDownEnd()
    {
        _playerCanUseAura = true;
    }

    //Finish dash cooldown and player can control the movement again
    void DashCoolownEnd()
    {
        _playerIsUsingDash = false;
        _dashGameObject.SetActive(false);
        _rigidbody.velocity = Vector3.zero;
    }

    void SpecialCooldownEnd()
    {
        _playerActivateSpecial = false;
    }

    //The character can be controlled again by the player
    void UnStunPlayer()
    {
        //Deactivate the animaion and stun
        _animator.SetBool("Stun", false);
        _playerIsStunned = false;
        //Refill stamina and update the text
        _playerStamina = 20;
        _gameController.SetHpText(gameObject.name, _playerStamina);
    }

    //Set player sound clip according to the player actions
    void SetAudioClip(AudioClip _clip)
    {
        _playerAudioSource.clip = _clip;
        _playerAudioSource.Play();
    }

    public void BodyCollisions(Collider2D _other)
    {
        if((_other.gameObject.tag == "Attack" || _other.gameObject.tag == "Enemy" || _other.gameObject.tag == "Special" 
            || _other.gameObject.tag == "Fire" || (_other.gameObject.tag == "Item" 
            && _other.gameObject.GetComponent<ItemController>().GetItemName() == "EvilPlant")) 
            && !_invincibility && !_playerIsStunned && !_gameOver && !_playerIsUsingDash)
        {
            //Decrease the stamina of the player and gives invincibility to the player
            if (_other.gameObject.tag == "Attack")
                _playerStamina -= 3;
            else if (_other.gameObject.tag == "Enemy")
                _playerStamina -= _other.gameObject.GetComponent<SlimeController>().GetDamage();
            else if (_other.gameObject.tag == "Item")
            {
                Destroy(_other.transform.parent.gameObject);
                _other.gameObject.GetComponent<ItemController>().ItemEffect(this);
            }
            else if (_other.gameObject.tag == "Special")
                _playerStamina = 0;
            else if (_other.gameObject.tag == "Fire")
                _playerStamina -= 2;

            //If the player has 0 hp the player is stunned for 3 seconds
            if (_playerStamina < 1)
            {
                SetAudioClip(_stunnedClip);
                _playerStamina = 0;
                _animator.SetBool("Stun", true);
                Invoke("UnStunPlayer", 3f);
                _playerIsStunned = true;
            }
            else
            {
                SetAudioClip(_hitClip);
            }
            _invincibility = true;
            _spriteRenderer.color = new Color(1, 1, 1, 0.5f);
            _gameController.SetHpText(gameObject.name, _playerStamina);
            
            //Set hurt animation
            _animator.SetTrigger("Hurt");

            //The player is invincible for 2 seconds, then the player can be damaged again
            Invoke("InvincibilityEnd", 1f);
        }

        if(_other.gameObject.tag == "Item")
        {
            Destroy(_other.transform.parent.gameObject);
            _other.gameObject.GetComponent<ItemController>().ItemEffect(this);
        }
    }

    //Set the points to the player in the game controller
    public void SetPoints(int _points)
    {
        this._points += _points;
    }

    //return the player points to the controller to set it in the text component
    public string GetPoints()
    {
        return _points.ToString();
    }

    //Set stamina if the player hasn't full stamina
    public void SetStamina(int _stamina)
    {
        _playerStamina += _stamina;
    }

    public void StunPlayer()
    {
        SetAudioClip(_stunnedClip);
        _playerStamina = 0;
        _animator.SetBool("Stun", true);
        Invoke("UnStunPlayer", 3f);
        _playerIsStunned = true;
    }

    //Set default stamina when the player get a potion with hp full
    public void SetDefaultStamina()
    {
        _playerStamina = 20;
    }

    //Get the player staima to check if it's full in the item script
    public int GetStamina()
    {
        return _playerStamina;
    }

    //Get the gameController to access to the required method
    public GameController GetGameController()
    {
        return _gameController;
    }

    //Un set pause if the player press start button
    public void UnSetPause()
    {
        _gamePaused = false;
    }

    //Set the pause in the game controller
    public void SetPause(bool _pause)
    {
        _gamePaused = _pause;
    }

    //The game is ended in the game controller
    public void GameIsOver()
    {
        _gameOver = true;
    }
}
