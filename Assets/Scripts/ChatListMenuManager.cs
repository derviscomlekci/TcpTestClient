using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChatListMenuManager : MonoBehaviour
{
    [SerializeField] private Transform _messageSpawnTransform;
    [SerializeField] private GameObject _messagePref;
    public static ChatListMenuManager Instance
    {
        get => _instance;
    }
    private static ChatListMenuManager _instance;
    
    
    
    private void Start()
    {
        if (_instance!=null && _instance != this)
        {
            Destroy(this);
        }
        else
        {
            _instance = this;
        }
    }

    public void SpawnMessage(string message,bool isClient)
    {
        GameObject messagePref=Instantiate(_messagePref, _messageSpawnTransform);
        messagePref.GetComponent<ChatMessage>().ChangeText(message);
        messagePref.GetComponent<ChatMessage>().ChangeColor(isClient);
    }
}
