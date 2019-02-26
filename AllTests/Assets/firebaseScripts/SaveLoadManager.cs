using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System;


public  static class SaveLoadManager {






    public static void saveData(String email, string password) 
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream fs = new FileStream(Application.persistentDataPath+ "/user.sav", FileMode.Create);

        SaveData sve = new SaveData(email,password);

        bf.Serialize(fs,sve);
        fs.Close();

    }

    public static string[] LoadData()
    {

        BinaryFormatter bf = new BinaryFormatter();
        FileStream fs = new FileStream(Application.persistentDataPath + "/user.sav", FileMode.Open);

        SaveData sve = bf.Deserialize(fs) as SaveData;
        
        fs.Close();
        return sve.data;
    }

    public static Boolean checkFile()
    {
        FileInfo fl = new FileInfo(Application.persistentDataPath + "/user.sav");
        Debug.Log(Application.persistentDataPath + "/user.sav");
        if (fl.Exists) return true;
        return false;

    }


}


[Serializable]
public class SaveData{
    public string[] data;

   public SaveData(string email,string password) {
        data = new string[2];
        data[0] = email;
        data[1] = password;

    }

}

