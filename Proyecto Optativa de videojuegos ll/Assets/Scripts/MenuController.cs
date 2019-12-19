using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{

    [SerializeField] private GameObject _menuCanvas;
    [SerializeField] private GameObject _creditCanvas;
    [SerializeField] private GameObject _startGameButton;
    [SerializeField] private GameObject _backMenuButton;

    public void StartGame()
    {
        SceneManager.LoadScene("MainGameScene");
    }

    public void Exit()
    {
        Application.Quit();
    }

    public void Credits()
    {
        _menuCanvas.SetActive(false);
        _creditCanvas.SetActive(true);
        GameObject.Find("EventSystem").GetComponent<EventSystem>().SetSelectedGameObject(_backMenuButton);
    }

    public void GoBackToMenu()
    {
        _menuCanvas.SetActive(true);
        _creditCanvas.SetActive(false);
        GameObject.Find("EventSystem").GetComponent<EventSystem>().SetSelectedGameObject(_startGameButton);
    }
}
