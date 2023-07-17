using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.UI;
using System.Net;
using System.Net.Sockets;
using System.Text;
using TMPro;
using UnityEngine.SceneManagement;
using System.Threading;
using System.Threading.Tasks;


public class Client : MonoBehaviour
{
    //Instance
    
    public static Client Instance;
    SynchronizationContext _context;

    
    //Tcp client
    public TcpClient socket;
    public NetworkStream stream;
    public byte[] buffer;
    public int dataBufferSize = 4096;
    public int id;
    public string playerName;
    public bool IsSearchGame = false;
    
    

    private void Awake()
    {
        if (Instance==null)
        {
            Instance = this;
        }
        else if (Instance!=this)
        {
            Destroy(this.gameObject);
        }
        UnityThread.initUnityThread();
        _context = SynchronizationContext.Current;
    }

    private void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }
    


    //

    
    public void ServerConnect(string name)
    {
        if (name!=null && socket==null)
        {
            playerName = name;
            socket = new TcpClient();
            socket.ReceiveBufferSize = dataBufferSize;
            socket.SendBufferSize = dataBufferSize;
            try
            {
                socket.BeginConnect(ServerSettings.HOST,ServerSettings.PORT,ConnectCallbak,null);
            }
            catch (Exception e)
            {
                Debug.Log($"An error occured while connecting :{e} ");
            }
        }
    }

    private void ConnectCallbak(IAsyncResult asyncResult)
    {
        socket.EndConnect(asyncResult );//Bağlantı tamamlanıyor
        if (!socket.Connected)
        {
            //Bağlanılamadıysa
            return;
        }
        Debug.Log("Connection succesfully..");
        //playerName = input_username.text;
        stream = socket.GetStream();
        buffer = new byte[dataBufferSize];
        stream.BeginRead(buffer, 0, dataBufferSize, ReceiveCallback, null);
    }

    private void ReceiveCallback(IAsyncResult asyncResult)//Gelen verinin işlendiği bölüm burası.
    {
        try
        {
            if (stream==null)
            {
                return;
            }
            int receivedDataLeng = stream.EndRead(asyncResult);
            if (receivedDataLeng<=0)
            {
                ServerDisconnect();
                return;
            }
            byte[] _data = new byte[receivedDataLeng];
            Array.Copy(buffer,_data,receivedDataLeng);
            string jsonData = Encoding.UTF8.GetString(_data);
            //Gelen veri handle edilecek.
            Debug.Log($"Gelen veri: {jsonData}");
            Handler.HandleData(jsonData);//<-- veriyi burada işliyoruz.
            stream.BeginRead(buffer, 0, dataBufferSize, ReceiveCallback, null);//Burada eğer tekrar veri gelirse diye bir read daha açıyoruz.
            
        }
        
        catch (Exception e)
        {
            ServerDisconnect();
            //Disconnect olmalı
            Debug.Log($"Okurken patladı: {e}");
            return;
        }
    }

    public void SendDataFromJson(string jsonData)
    {
        byte[] _data = Encoding.UTF8.GetBytes(jsonData);
        try
        {
            stream.BeginWrite(_data, 0, _data.Length, SendCallback, null);
            Debug.Log($"Giden veri: {jsonData}");
        }
        catch (Exception e)
        {
            ServerDisconnect();
            return;
        }
    }

    public void ClearAllUserData()
    {
        playerName = "";
        IsSearchGame = false;
    }

    public void SendCallback(IAsyncResult asyncResult)
    {
        stream.EndRead(asyncResult);
    }

    public void ServerDisconnect()
    {
        if (socket!=null)
        {
            socket.Close();
        }
        if (stream!=null)
        {
            stream.Close();
        }
        socket = null;
        stream = null;
        buffer = null;
        ClearAllUserData();
        //deneme();
        _context.Post(_ =>LoadLoginScene(), this);
    }

    public void LoadLoginScene()
    {
        _context.Post(_=>SceneManager.LoadSceneAsync("Login"),null);
        //SceneManager.LoadSceneAsync("Login"); 
    }

    public void ServerSearchGame()
    {
        //arama yaptığı bilgisini yolluyor.
        SendDataFromJson(JsonUtility.ToJson(Handler.CreateSearch(id,(int)Handler.ClientEnum.SearchGame,true)));
    }

    public void ServerDeSearchGame()
    {
        //aramayı iptal ediyor.
        SendDataFromJson(JsonUtility.ToJson(Handler.CreateSearch(id,(int)Handler.ClientEnum.SearchGame,false)));
    }

    public void ServerLoadGameScene()
    {
        //Oyun  sahnesini  yükle
        //SendDataFromJson(JsonUtility.ToJson(Handler.CreateConnectRoom()));
        SceneManager.LoadSceneAsync("Game");
    }

    public void ClientLoadedGameScene()
    {
        //Sunucuya sahneyi  yükledim diyecek oyunuc bilgilerini alocaz sunucudan.
        SendDataFromJson(JsonUtility.ToJson(Handler.CreateConnectRoom(id,(int)Handler.ClientEnum.ConnectRoom,true)));
    }

    public void ServerSendChatMessage(string _message)
    {
        SendDataFromJson(JsonUtility.ToJson(Handler.CreateChatMessage(id,(int)(Handler.ClientEnum.ChatMessage),_message,Client.Instance.id)));
    }
    
}
