using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuStartPanelManager : MonoBehaviour
{
    public TextMeshProUGUI searchingGameTxt;
    public TextMeshProUGUI playerName;
    public Button searchBtn;
    private static MenuStartPanelManager _instance;

    public static MenuStartPanelManager Instance{ get { return _instance; } }
    private void Start()
    {
        //Singleton
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }

        playerName.text = Client.Instance.playerName;
        //Handler.IsGameFoundedEvent += GameFounded;
    }

    private void OnDestroy()
    {
        //Handler.IsGameFoundedEvent -= GameFounded;
    }

    public void SearchGame()
    {
        if (Client.Instance.IsSearchGame)
        {
            Client.Instance.ServerDeSearchGame();
            searchingGameTxt.text = "";
            searchBtn.GetComponentInChildren<TextMeshProUGUI>().text = "Search Game.";
        }
        else
        {
            Client.Instance.ServerSearchGame();
            searchingGameTxt.text = "Searching game...";
            searchBtn.GetComponentInChildren<TextMeshProUGUI>().text = "Cancel Search Game.";
        }
    }

    public void GameFounded(bool isFounded)
    {
        Debug.Log("Oyuncu bulundu.");

        searchingGameTxt.text = "Oyuncu bulundu";
        //searchingGameTxt.gameObject.SetActive(!isFounded);
    }

    public void DisconnectServer()
    {
        Client.Instance.ServerDisconnect();
    }
}