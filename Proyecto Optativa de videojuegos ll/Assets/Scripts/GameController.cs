using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    //UI components
    [SerializeField] private Text _alexPointsText;
    [SerializeField] private Text _rufusPointsText;
    [SerializeField] private Text _timeText;
    [SerializeField] private Text _winnerNameText;
    [SerializeField] private Text _winnerPointsText;
    [SerializeField] private string[] _slimeColorsText;
    [SerializeField] private Color[] _slimeColors;
    [SerializeField] private Image _slimeImage;
    [SerializeField] private Sprite _slimeSprite;
    [SerializeField] private GameObject _resumeButton;
    [SerializeField] private GameObject _restartButton;
    [SerializeField] private GameObject _inGameCanvas;
    [SerializeField] private GameObject _pauseCanvas;
    [SerializeField] private GameObject _gameOverCanvas;
    [SerializeField] private Animator _alexImageAnimator;
    [SerializeField] private Animator _rufusImageAnimator;
    [SerializeField] private Scrollbar _alexHealthBar;
    [SerializeField] private Scrollbar _rufusHealthBar;

    //AudioComponents
    [SerializeField] private AudioClip _inGameMusic;
    [SerializeField] private AudioClip _pausedGameMusic;
    private AudioSource _musicAudioSource;

    //Spawners Script of the game objects
    [SerializeField] private SpawnerController[] _spawners;

    //The players, spawners and canvas game objects, actual color of the slime and reamaining time 
    private GameObject _alexPlayer;
    private GameObject _rufusPlayer;
    private GameObject _spawnersParentGameObject;
    private string _actualColor;
    private int _enemywave;
    private float _spawnTime;
    private int _minutes;
    private float _seconds;
    private bool _gameOver;
    private bool _isPausedGame;

    void Start()
    {
        //Set the time scale to 1 if the player quit the game during the gameplay
        if(Time.timeScale == 0)
            Time.timeScale = 1;

        //Get AudioSource component
        _musicAudioSource = GetComponent<AudioSource>();

        //Get the players and spawners game objects
        _alexPlayer = GameObject.Find("Alex");
        _rufusPlayer = GameObject.Find("Rufus");
        _spawnersParentGameObject = GameObject.Find("Spawners");

        //Set the sprite as an image in canvas
        _slimeImage.sprite = _slimeSprite;
        InvokeRepeating("SetColor", 0, 10);

        //Start the timer
        _minutes = 5;
        _seconds = 0;

        //Set game over as false
        _gameOver = false;
        _isPausedGame = false;

        _enemywave = 1;
        _spawnTime = 3f;
        InvokeRepeating("SetSlimesPatterns", 0, 37);
    }

    void Update()
    {
        //Update the timer if the game time hasn´t finished
        if ((_minutes > 0 || _seconds > 0))
            Timer();

        //The characters can't be controlled and set the winner if there is
        else
        {
            CancelInvoke("SetSlimesPatterns");
            _gameOver = true;
            _alexPlayer.GetComponent<CharacterController>().GameIsOver();
            _rufusPlayer.GetComponent<CharacterController>().GameIsOver();
            SetWinner();
            CancelInvoke("SetColor");
            _spawnersParentGameObject.GetComponentInChildren<SpawnerController>().GameOver();
            _alexPlayer.SetActive(false);
            _rufusPlayer.SetActive(false);
        }
    }

    public void SetPlayerPoints(string _slimeColor, string _playerName, int _points)
    {
        //Verify if the player destroyed the correct color of the slime
        if(_slimeColor == _actualColor)
        {
            //Then the player win the slime points
            if (_playerName == _alexPlayer.name)
            {
                _alexPointsText.GetComponent<Animator>().SetTrigger("Win");
                _alexPlayer.GetComponent<CharacterController>().SetPoints(_points);
                _alexPointsText.text = "Puntos Alex: " + _alexPlayer.GetComponent<CharacterController>().GetPoints();
            }
            else if (_playerName == _rufusPlayer.name)
            {
                _rufusPointsText.GetComponent<Animator>().SetTrigger("Win");
                _rufusPlayer.GetComponent<CharacterController>().SetPoints(_points);
                _rufusPointsText.text = "Puntos Rufus: " + _rufusPlayer.GetComponent<CharacterController>().GetPoints();
            }
        }
        else
        {
            //In other case the rival win the points
            if (_playerName == _alexPlayer.name)
            {
                _alexPointsText.GetComponent<Animator>().SetTrigger("Lose");
                _rufusPointsText.GetComponent<Animator>().SetTrigger("Win");
                _rufusPlayer.GetComponent<CharacterController>().SetPoints(_points);
                _rufusPointsText.text = "Puntos Rufus: " + _rufusPlayer.GetComponent<CharacterController>().GetPoints();
            }
            else if (_playerName == _rufusPlayer.name)
            {
                _rufusPointsText.GetComponent<Animator>().SetTrigger("Lose");
                _alexPointsText.GetComponent<Animator>().SetTrigger("Win");
                _alexPlayer.GetComponent<CharacterController>().SetPoints(_points);
                _alexPointsText.text = "Puntos Alex: " + _alexPlayer.GetComponent<CharacterController>().GetPoints();
            }
        }
    }

    public void SetPlayerPoints(string _playerName, int _points)
    {
        if(_playerName == _alexPlayer.name)
        {
            _alexPointsText.GetComponent<Animator>().SetTrigger("Win");
            _alexPointsText.text = "Puntos Alex: " + _alexPlayer.GetComponent<CharacterController>().GetPoints();
        }
        else if(_playerName == _rufusPlayer.name)
        {
            _rufusPointsText.GetComponent<Animator>().SetTrigger("Win");
            _rufusPointsText.text = "Puntos Rufus: " + _rufusPlayer.GetComponent<CharacterController>().GetPoints();
        }
    }

    //Set the hp in the text component of the canvas (is called in the CharacterController script)
    public void SetHpText(string _playerName, int _hp)
    {
        if(_playerName == _alexPlayer.name)
        {
            _alexHealthBar.size = _hp / 20f;
        }
        else if(_playerName == _rufusPlayer.name)
        {
            _rufusHealthBar.size = _hp / 20f;
        }
    }

    //Method to set the pause state and (de)activate objects in scene
    public void SetPauseState(bool _pauseState)
    {
        _spawnersParentGameObject.SetActive(!_pauseState);
        _isPausedGame = _pauseState;
        _alexPlayer.GetComponent<CharacterController>().SetPause(_isPausedGame);
        _rufusPlayer.GetComponent<CharacterController>().SetPause(_isPausedGame);

        if (!_pauseState)
        {
            Time.timeScale = 1;
            _inGameCanvas.SetActive(true);
            _pauseCanvas.SetActive(false);
            GameObject.Find("EventSystem").GetComponent<EventSystem>().SetSelectedGameObject(_restartButton);
            SetAudioClip(_inGameMusic);
        }
        else
        {
            Time.timeScale = 0;
            _inGameCanvas.SetActive(false);
            _pauseCanvas.SetActive(true);
            GameObject.Find("EventSystem").GetComponent<EventSystem>().SetSelectedGameObject(_resumeButton);
            SetAudioClip(_pausedGameMusic);
        }
    }

    //Method that deactivate pause when the player click the button
    public void DeactivatePause()
    {
        _alexPlayer.GetComponent<CharacterController>().UnSetPause();
        _rufusPlayer.GetComponent<CharacterController>().UnSetPause();
    }

    //Bool for other scripts to detect if the game's finished
    public bool isGameOver()
    {
        return _gameOver;
    }

    public void SetGameOver()
    {
        SetAudioClip(_pausedGameMusic);
        _gameOverCanvas.SetActive(true);
        _pauseCanvas.SetActive(false);
        _inGameCanvas.SetActive(false);
        GameObject.Find("EventSystem").GetComponent<EventSystem>().SetSelectedGameObject(_restartButton);
    }

    public Color GetColor()
    {
        return _slimeImage.color;
    }

    public string GetColorName()
    {
        return _actualColor;
    }

    //Method to set the music
    void SetAudioClip(AudioClip _clip)
    {
        _musicAudioSource.clip = _clip;
        _musicAudioSource.Play();
    }

    //Determines the color of the slime to destroy
    void SetColor()
    {
        int _colorIndex = Random.Range(0, _slimeColorsText.Length);
        _actualColor = _slimeColorsText[_colorIndex];
        _slimeImage.color = _slimeColors[_colorIndex];
    }

    //Decrease the time text if the game isn't in pause
    void Timer()
    {
        if(!_isPausedGame)
        {
            if (_seconds <= 0 && _minutes > 0)
            {
                _minutes--;
                _seconds = 59;
                _timeText.GetComponent<Animator>().SetTrigger("Minute");
            }
            else
                _seconds -= Time.deltaTime;
            _timeText.text = _minutes.ToString("00") + ":" + _seconds.ToString("00");

            if (_minutes == 0 && _seconds <= 10)
                _timeText.GetComponent<Animator>().SetTrigger("LastSeconds");
        }
    }

    void SetWinner()
    {
        int _alexPoints = int.Parse(_alexPlayer.GetComponent<CharacterController>().GetPoints());
        int _rufusPoints = int.Parse(_rufusPlayer.GetComponent<CharacterController>().GetPoints());
        //Set the name and points of the winner
        if(_rufusPoints > _alexPoints)
        {
            _winnerNameText.text = "RUFUS HA GANADO";
            _winnerPointsText.text = "PUNTAJE: " + _rufusPoints.ToString();
            _alexImageAnimator.SetTrigger("Lose");
        }
        else if(_alexPoints > _rufusPoints)
        {
            _winnerNameText.text = "ALEX HA GANADO";
            _winnerPointsText.text = "PUNTAJE: " + _alexPoints.ToString();
            _rufusImageAnimator.SetTrigger("Lose");
        }
        else
        {
            _winnerNameText.text = "HA SIDO UN EMPATE";
            _winnerPointsText.text = "PUNTAJE: " + _rufusPoints.ToString();
            _rufusImageAnimator.SetTrigger("Lose");
            _alexImageAnimator.SetTrigger("Lose");
        }
    }

    void SetSlimesPatterns()
    {
        int i;
        for (i = 0; i < _spawners.Length; i++)
        {
            _spawners[i].CancelInvoke("SpawnSlimes");
            _spawners[i].ClearSpawnArrays();
        }
        switch (_enemywave)
        {
            case 1:
                for (i = 0; i < _spawners.Length; i++)
                    _spawners[i].SlimeCharacteristics(0, 4);
                break;

            case 2:
                _spawnTime = 7;
                for (i = 0; i < _spawners.Length; i++)
                    _spawners[i].SlimeCharacteristics(1, 4);
                break;

            case 3:
                _spawnTime = 8;
                for (i = 0; i < _spawners.Length; i++)
                    _spawners[i].SlimeCharacteristics(2, 3);
                break;

            case 4:
                _spawnTime = 6;
                for (i = 0; i < _spawners.Length; i++)
                    _spawners[i].SlimeCharacteristics(0, 5);
                for (i = 0; i < _spawners.Length; i++)
                    _spawners[i].SlimeCharacteristics(1, 5);
                break;

            case 5:
                for (i = 0; i < _spawners.Length; i++)
                    _spawners[i].SlimeCharacteristics(0, 5);
                for (i = 0; i < _spawners.Length; i++)
                    _spawners[i].SlimeCharacteristics(2, 3);
                break;

            case 6:
                for (i = 0; i < _spawners.Length; i++)
                    _spawners[i].SlimeCharacteristics(1, 4);
                for (i = 0; i < _spawners.Length; i++)
                    _spawners[i].SlimeCharacteristics(2, 4);
                break;

            case 7:
                for (i = 0; i < _spawners.Length; i++)
                    _spawners[i].SlimeCharacteristics(0, 7);
                for (i = 0; i < _spawners.Length; i++)
                    _spawners[i].SlimeCharacteristics(1, 6);
                for (i = 0; i < _spawners.Length; i++)
                    _spawners[i].SlimeCharacteristics(2, 5);
                break;

            default:
                _spawnTime = 7;
                for (i = 0; i < _spawners.Length; i++)
                    _spawners[i].SlimeCharacteristics(0, 7);
                for (i = 0; i < _spawners.Length; i++)
                    _spawners[i].SlimeCharacteristics(1, 6);
                for (i = 0; i < _spawners.Length; i++)
                    _spawners[i].SlimeCharacteristics(2, 6);
                break;
        }
        _enemywave++;
        for (i = 0; i < _spawners.Length; i++)
            _spawners[i].InvokeRepeating("SpawnSlimes", 0, _spawnTime);
    }
}
