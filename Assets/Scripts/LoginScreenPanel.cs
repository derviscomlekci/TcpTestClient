using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LoginScreenPanel : MonoBehaviour
{
    public TMP_InputField input_username;
    public void ConnectToServer()
    {
        Client.Instance.ServerConnect(input_username.text);
    }
}
