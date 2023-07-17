using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class ChatMessage : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI messageTxt;

    public void ChangeText(string message)
    {
        messageTxt.text = message;
    }

    public void ChangeColor(bool isClient)
    {
        if (isClient)
        {
            GetComponent<Image>().color=Color.green;
            messageTxt.color = Color.black;
        }
        else
        {
            GetComponent<Image>().color=Color.blue;
            messageTxt.color=Color.white;
        }
    }
}
