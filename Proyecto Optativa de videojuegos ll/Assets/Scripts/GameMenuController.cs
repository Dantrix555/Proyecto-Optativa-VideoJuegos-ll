using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class GameMenuController : MonoBehaviour
{
    [SerializeField] private GameController _gameController;

    public void UnPauseGame()
    {
        _gameController.SetPauseState(false);
        _gameController.DeactivatePause();
    }

    public void ExitMenu()
    {
        SceneManager.LoadScene("TitleScene");
    }

    public void RestartGame()
    {
        SceneManager.LoadScene("MainGameScene");
    }
}
