using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.Networking;
public static class SavePlayerData 
{
    public static void Save(PlayerData player)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "player.bruh";
        FileStream stream = new FileStream(path, FileMode.Create);
        formatter.Serialize(stream, player);
        stream.Close();
    }

    public static void SaveId(PlayerID player)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "player.id";
        FileStream stream = new FileStream(path, FileMode.Create);
        formatter.Serialize(stream, player);
        stream.Close();
    }
    public static PlayerData LoadPlayerData()
    {
        string path = Application.persistentDataPath + "player.bruh";
        if(File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path,FileMode.Open);
            PlayerData data = formatter.Deserialize(stream) as PlayerData;
            stream.Close();
            return data;
        }
        else
        {
            string fakeName = "KEKW";
            PlayerData player = new PlayerData(fakeName);
            Save(player);
            Debug.LogError("I havent thought this through, no file found at: " +path);
            return player;
        }
    }

    public static string GetPlayerID(out bool exist)
    {
        string path = Application.persistentDataPath + "player.id";
        if(File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path,FileMode.Open);
            PlayerID data = formatter.Deserialize(stream) as PlayerID;
            stream.Close();
            exist =true;
            return data.ID;
        }
        else
        {
            exist = false;
            return "";
        }
    }
}
