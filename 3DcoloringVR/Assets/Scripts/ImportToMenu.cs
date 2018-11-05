using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class ImportToMenu : MonoBehaviour {

    int i = 1;
    public string FileName, FileDate, FileTime, i_txt;
    Text txt;
    Button btn;
    public static string file_path = "C:/Users/user/Desktop/3d_obj";
    public static string[] file_name_arr = new string[4];
    
    // Use this for initialization
    void Start()
    {
        Debug.Log("file");
        System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(file_path);

        foreach (System.IO.FileInfo File in di.GetFiles())
        {
            if (File.Extension.ToLower().CompareTo(".obj") == 0)
            {
                FileName = File.Name.Substring(0, File.Name.Length - 4);
                file_name_arr[i] = FileName + ".obj";
               

                FileDate = File.LastWriteTime.ToShortDateString();
                FileTime = File.LastWriteTime.ToShortTimeString();
                FileTime = FileTime.Substring(0, FileTime.Length - 2);
                
                i_txt = i.ToString();
                txt = GameObject.Find("Text" + i_txt).GetComponent<Text>();
                Debug.Log("text");
                txt.text = FileName + "\n" + FileDate + " " + FileTime;
                i++;
            }
        }
    }

    // Update is called once per frame
    void Update ()
    {
	}
}
