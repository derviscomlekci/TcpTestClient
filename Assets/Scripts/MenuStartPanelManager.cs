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

    
    
    private void Start()
    {
        playerName.text = Client.Instance.playerName;
    }
    
    public void SearchGame()
    {
        if (Client.Instance.IsSearchGame)
        {
            Client.Instance.ServerDeSearchGame();
            searchingGameTxt.gameObject.SetActive(false);
            searchBtn.GetComponentInChildren<TextMeshProUGUI>().text = "Search Game.";
        }
        else
        {
            Client.Instance.ServerSearchGame();
            searchingGameTxt.gameObject.SetActive(true);
            searchBtn.GetComponentInChildren<TextMeshProUGUI>().text = "Cancel Search Game.";
        }
    
    }

    public void DisconnectServer()
    {
        Client.Instance.ServerDisconnect();
    }
    
}
