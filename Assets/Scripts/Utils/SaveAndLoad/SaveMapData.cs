﻿using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.Networking;

public static class SaveMapData 
{
#if UNITY_EDITOR
    public static void SaveArray(ManualMapGenerator mapGenerator, string name, out string fName)
    {
        
        BinaryFormatter formatter = new BinaryFormatter();
        int count = 0;
        string fileName = name + ".md";
        string path = Path.Combine(Application.streamingAssetsPath,fileName);
        while(File.Exists(path))
        {
            count++;
            fileName = name + count.ToString() + ".md";
            path = Path.Combine(Application.streamingAssetsPath,fileName);
        }
        FileStream stream = new FileStream(path, FileMode.Create);
        MapData mapData = new MapData(mapGenerator);
        formatter.Serialize(stream, mapData);
        stream.Close();
        fName = fileName;
    }
#endif

    public static MapData LoadMap(string name)
    {
        name = name+".md";
        string path = Path.Combine(Application.streamingAssetsPath,name);
        if(Application.platform == RuntimePlatform.Android )
        {
            var loadingRequest = UnityWebRequest.Get(path);
            loadingRequest.SendWebRequest();
            while(!loadingRequest.isDone)
            {
                if(loadingRequest.isNetworkError || loadingRequest.isHttpError)
                {
                    break;
                }
            }
            if(loadingRequest.isNetworkError || loadingRequest.isHttpError)
            {
                Debug.Log("Can't get request.");
                return null;
            }        
            BinaryFormatter formatter = new BinaryFormatter();
            MemoryStream stream = new MemoryStream(loadingRequest.downloadHandler.data);
            MapData data = formatter.Deserialize(stream) as MapData;
            return data;
        }
        else if(Application.platform == RuntimePlatform.IPhonePlayer)
        {
            if(File.Exists(path))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                Debug.Log("Getting data...");
                FileStream stream  = File.OpenRead(path);
                Debug.Log("Success!");
                MapData data = formatter.Deserialize(stream) as MapData;
                stream.Close();
                return data;
            }
            else
            {
                Debug.Log("Binary data file not Found");
                return null;
            }
        }
        else
        {
            if(File.Exists(path))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                //Debug.Log("Getting data...");
                FileStream stream  = new FileStream(path, FileMode.Open);
                //Debug.Log("Success!");
                MapData data = formatter.Deserialize(stream) as MapData;
                stream.Close();
                return data;
            }
            else
            {
                Debug.Log("Binary data file not Found");
                return null;
            }
        }
        
    }

}
