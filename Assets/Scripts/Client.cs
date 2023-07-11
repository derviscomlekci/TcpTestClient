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

public class Client : MonoBehaviour
{
    //Instance
    public TMP_InputField input_username;
    public static Client Instance;

    
    //Tcp client
    public TcpClient socket;
    public NetworkStream stream;
    public byte[] buffer;
    public int dataBufferSize = 4096;
    public int id;
    public string playerName;
    
    
    
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
    }

    private void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }
    


    //

    
    public void ServerConnect()
    {
        if (input_username.text!=null && socket==null)
        {
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
        playerName = input_username.text;
        stream = socket.GetStream();
        buffer = new byte[dataBufferSize];
        stream.BeginRead(buffer, 0, dataBufferSize, ReceiveCallback, null);
    }

    private void ReceiveCallback(IAsyncResult asyncResult)//Gelen verinin işlendiği bölüm burası.
    {
        try
        {
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

    public void SendCallback(IAsyncResult asyncResult)
    {
        stream.EndRead(asyncResult);
    }

    public void ServerDisconnect()
    {
        
    }
}
