using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Handler : MonoBehaviour
{
    public static Action<bool> IsGameFoundedEvent;

    public enum ServerEnum
    {
        /*
        Hello=1,
        ServerGame=2,
        ConnectRoom=3,
        ChatMessage=4,
        */
    }

    public enum ClientEnum
    {
        RegisterUser = 1,
        LoginUser = 2,
        LoginUserResponse = 4,
        RegisterUserResponse = 5,
        GetProduct = 6,
        GetUser = 7,
        SetUser = 8,

        /*
        Hello=1,
        SearchGame=2,
        ConnectRoom=3,
        ChatMessage=4,
        Register=5,
        */
    }

    private static SynchronizationContext Context { get; set; }

    private void Awake()
    {
        Context = SynchronizationContext.Current;
    }

    public static void HandleData(string jsonData)
    {
        Packet mainpacket = JsonUtility.FromJson<Packet>(jsonData);
        switch (mainpacket.opcode)
        {
            case (int)ClientEnum.LoginUser:
                //Kullanıcı giris isteği attı.
                break;
            case (int)ClientEnum.RegisterUser:
                //Kullanıcı üye oldu.
                break;
            case (int)ClientEnum.RegisterUserResponse:
                //Kullanıcı üye oldu ve serverdan dönüş geldi.
                GetRegisterResponse(JsonUtility.FromJson<RegisterResponsePacket>(jsonData));
                break;
            case (int)ClientEnum.LoginUserResponse:
                //Kullanıcıya girişi hakkında bilgi geldi.
                
                break;

            /*
            case (int)ServerEnum.Hello://Server hellosu.
            {
                Get_Hello(JsonUtility.FromJson<Hello>(jsonData));
                break;
            }
            case (int)ServerEnum.ServerGame:
            {
                GetSearch(JsonUtility.FromJson<SearchPacket>(jsonData));
                break;
            }
            case (int)ServerEnum.ChatMessage:
            {
                Get_ChatMessage(JsonUtility.FromJson<ChatMessage>(jsonData));
                break;
            }
            */
            default:
                break;
        }
    }

    public static void Get_Hello(Hello packet) //sunucu kabul ve bizim bilgileri gönderdiğimiz yer.
    {
        //Client.Instance.id = packet.id;
        //.Client.Instance.playerName = packet.name;
        //Client.Instance.SendDataFromJson(JsonUtility.ToJson(Create_Hello(Client.Instance.id,(int)ClientEnum.Hello,Client.Instance.playerName)));//ismimizi gönderdik.

        UnityThread.executeInFixedUpdate(() => { SceneManager.LoadScene("Menu"); });
        //SceneManager.LoadScene("Menu");
    }

    public class Packet
    {
        //public int id;
        public int opcode;
    }

    public class Hello : Packet
    {
        public string message;
        public string name;
    }

    public static Hello Create_Hello(int _id, int _type, string _name)
    {
        Hello packet = new Hello();
        //.id = _id;
        packet.opcode = _type;
        packet.name = _name;
        return packet;
    }

    public class RegisterPacket : Packet
    {
        public string username;
        public string password;
        public string tckno;
        public string address;
    }

    public static RegisterPacket Create_RegisterPacket(int _type, string _username, string _password, string _tckno,
        string _address)
    {
        RegisterPacket packet = new RegisterPacket();
        //packet.id = _id;
        packet.opcode = _type;
        packet.username = _username;
        packet.password = _password;
        packet.tckno = _tckno;
        packet.address = _address;
        return packet;
    }

    public class RegisterResponsePacket : Packet
    {
        public bool IsRegistered { get; set; }
        public string Message { get; set; }
    }

    public static void GetRegisterResponse(RegisterResponsePacket data)
    {
        if (data.IsRegistered)
        {
            LoginScreenPanel.Instance.ChangeRegisterReceiveText(Color.green, data.Message);
            return;
        }
        LoginScreenPanel.Instance.ChangeRegisterReceiveText(Color.red, data.Message);
    }

    public class LoginResponsePacket : Packet
    {
        public bool IsLoggedIn { get; set; }
        public string ErrorMessage;
    }

    public static void GetLoginResponse(LoginResponsePacket data)
    {
        if (data.IsLoggedIn)
        {
            Debug.Log("Oyuncu içeri girdi.");
            //Oyuncuyu içeriye sok
        }
    }

    public class SearchPacket : Packet
    {
        public bool search; //Bulunduysa false

        public bool found; //Bulunduysa true
        //Bulunduysa search:false, found:true, aranıyorsa search:true, found:false, bulunamadı search:false, found:true
    }

    public static SearchPacket CreateSearch(int _id, int _type, bool _search)
    {
        SearchPacket searchPacket = new SearchPacket();
        //searchPacket.id = _id;
        searchPacket.opcode = _type;
        searchPacket.search = _search;
        return searchPacket;
    }

    public static void GetSearch(SearchPacket packet)
    {
        if (packet.search && !packet.found) //aranma işlemi başladı haberi geliyor  serverdan
        {
            // arama texti görünür olacak.
            Client.Instance.IsSearchGame = true;
            Debug.Log("Oyun aranıyor.");
        }
        else if (!packet.search && packet.found) // oyun bulunduysa
        {
            //IsGameFoundedEvent?.Invoke(true);
            Context.Post(_ => MenuStartPanelManager.Instance.GameFounded(true), null);
            Context.Post(_ => Client.Instance.ServerLoadGameScene(), null);
            Debug.Log("Oyun bulundu.");
        }
        else //arama iptal edildiyse
        {
            Client.Instance.IsSearchGame = false;
            Debug.Log("Oyun arama iptal edildi.");
        }
    }

    public class ConnectRoom : Packet
    {
        public bool isConnect;
    }

    public static ConnectRoom CreateConnectRoom(int _id, int _type, bool _connect)
    {
        ConnectRoom packet = new ConnectRoom();
        //packet.id = _id;
        packet.opcode = _type;
        packet.isConnect = _connect;
        return packet;
    }

    public class ChatMessage : Packet
    {
        public string message;
        public int senderId;
    }

    public static ChatMessage CreateChatMessage(int _id, int _type, string _message, int _senderId)
    {
        ChatMessage packet = new ChatMessage();
        //packet.id = _id;
        packet.opcode = _type;
        packet.message = _message;
        packet.senderId = _senderId;
        return packet;
    }

    public static void Get_ChatMessage(ChatMessage packet)
    {
        //Context.Post(_ => GameManager.Instance.ChangeChatMessage(packet.message), null);
        bool isClientMessage = false;
        if (packet.senderId == Client.Instance.id)
        {
            isClientMessage = true;
        }

        Context.Post(_ => ChatListMenuManager.Instance.SpawnMessage(packet.message, isClientMessage), null);
    }
}