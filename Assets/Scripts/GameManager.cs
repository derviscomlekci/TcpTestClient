using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    public static GameManager Instance{ get { return _instance; } }
    private static GameManager _instance;
    public TMP_InputField playerMessage;
    public TextMeshProUGUI chatText;
    void Start()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
        //Oyuncu odaya yüklendi.
        Client.Instance.ClientLoadedGameScene();
    }

    public void ChangeChatMessage(string message)
    {
        chatText.text = message;
    }

    public void SendMessage()
    {
        if (playerMessage.text!=null)
        {
            Client.Instance.ServerSendChatMessage(playerMessage.text);
        }
    }
}
