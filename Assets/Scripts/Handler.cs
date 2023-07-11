using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Handler : MonoBehaviour
{
    public enum Server
    {
        Hello=1
    }
    public enum ClientEnum
    {
        Hello=1
    }

    public static void HandleData(string jsonData)
    {
        Packet mainpacket = JsonUtility.FromJson<Packet>(jsonData);
        switch (mainpacket.type)
        {
            case (int)ClientEnum.Hello://Server hellosu.
            {
                Get_Hello(JsonUtility.FromJson<Hello>(jsonData));
                break;
            }
            default:
                break;
        }
    }
    public static void Get_Hello(Hello packet)//sunucu kabul ve bizim bilgileri gönderdiğimiz yer.
    {
        Client.Instance.id = packet.id;
        //.Client.Instance.playerName = packet.name;
        Client.Instance.SendDataFromJson(JsonUtility.ToJson(Create_Hello(Client.Instance.id,(int)ClientEnum.Hello,Client.Instance.playerName)));
    }
    public class Packet
    {
        public int id;
        public int type;
    }
    public class Hello: Packet 
    {
        public string message;
        public string name;
    }
    
    public static Hello Create_Hello(int _id,int _type,string _name)
    {
        Hello packet= new Hello();
        packet.id = _id;
        packet.type = _type;
        packet.name = _name;
        return packet;
    }
}
