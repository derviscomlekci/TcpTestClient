using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class LoginScreenPanel : MonoBehaviour
{
    public TMP_InputField input_username;
    public GameObject loginPanel;
    public GameObject registerPanel;
    public TMP_InputField registerUsernameInput;
    public TMP_InputField registerPasswordInput;
    public TMP_InputField registerTckNoInput;
    public TMP_InputField registerAddressInput;
    public TextMeshProUGUI registerReceiveTxt;

    private static LoginScreenPanel _instance;
    public static LoginScreenPanel Instance{ get { return _instance; } }

    private void Start()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }


    public void PanelController()
    {
        if (loginPanel.activeSelf)
        {
            loginPanel.SetActive(false);
            registerPanel.SetActive(true);
        }
        else
        {
            loginPanel.SetActive(true);
            registerPanel.SetActive(false);
        }
    }
    
    public void ConnectToServer()
    {
        Client.Instance.ServerConnect(input_username.text);
    }

    public void Register()
    {
        if (registerUsernameInput!=null && registerPasswordInput!=null && registerPasswordInput!=null  && registerAddressInput!=null)
        { 
            Client.Instance.RegisterServer(Opcodes.RegisterUser,registerUsernameInput.text,registerPasswordInput.text,registerTckNoInput.text,registerAddressInput.text);
        }
    }

    public void ChangeRegisterReceiveText(string message)
    {
        registerReceiveTxt.text = message;
    }
}
