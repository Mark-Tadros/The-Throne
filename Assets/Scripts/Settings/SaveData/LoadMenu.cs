// Contains the loading system commands.
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class LoadMenu : MonoBehaviour
{
    public GameManager gameManagerScript;
    public TitleScreen titleScreenScript;
    public SaveMenu saveMenuScript;
    //Loads the Save Files.
    void Start()
    {
        BinaryFormatter binaryformatter = new BinaryFormatter(); FileStream file;
        if (File.Exists(Application.persistentDataPath + "/Save.dat"))
        {
            Debug.Log("Load Save");
            file = File.Open(Application.persistentDataPath + "/Save.dat", FileMode.Open);
            gameManagerScript.saveData = (SaveData)binaryformatter.Deserialize(file);
            file.Close();
            titleScreenScript.firstLoading = true;
        }
        else { Debug.Log("New Save"); saveMenuScript.Reset(); }
    }
}