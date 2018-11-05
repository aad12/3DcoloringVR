using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.VR;
using System;
using System.IO;
//using UnityEditor;

public class UIInteraction : MonoBehaviour
{
    public UnityEngine.UI.Text outText;

    protected Material oldHoverMat;
    public Material yellowMat;

    private TestMessagePanel testMessagePanel;

    private static int LorR = 0;
    private int flag = 0;
    private bool tri = false;

    // Use this for initialization
    void Start()
    {
        testMessagePanel = TestMessagePanel.Instance();
    }

    private void Update()
    {
        try
        {
            if(Controller.control_mode == 0)
                OnSelected(Controller.rhit.transform);
            else if (Controller.control_mode == 1)
                OnSelected(Controller.lhit.transform);
        }
        catch (NullReferenceException)
        {
            Debug.Log("null");
        }
    }
    
    public void OnSelected(Transform t)
    {
        if (OVRInput.Get(Controller.brushIndex) > 0)
        {
            tri = true;
        }

        if (OVRInput.Get(Controller.brushIndex) == 0 && tri == true)
        {
            if (t.tag == "btn_new")
            {
                Painting.load = false;
                SceneManager.LoadScene("NewProjectScene");
            }
            else if (t.gameObject.name == "btn_load")
            {
                Painting.load = true;
                SceneManager.LoadScene("LoadProjectScene");
            }
            else if (t.gameObject.name == "btn_setting")
            {
                SceneManager.LoadScene("SettingScene");
            }
            /////////////////////////////////////////////////////////////////////
            /////////////////////////////////////////////////////////////////////
            else if (t.gameObject.name.StartsWith("btn_ex"))
            {
                Debug.Log(t.gameObject.name.Split('x')[1]);
                Painting.file_path = ImportToMenu.file_path + "/";
                Painting.file_name = ImportToMenu.file_name_arr[System.Convert.ToInt32(t.gameObject.name.Split('x')[1])];
                SceneManager.LoadScene("Painting");
            }
            /////////////////////////////////////////////////////////////////////
            /////////////////////////////////////////////////////////////////////
            else if (t.gameObject.name == "btn_left")
            {
                LorR = 1;
                testMessagePanel.TestYN(0);
            }
            else if (t.gameObject.name == "btn_right")
            {
                LorR = 0;
                testMessagePanel.TestYN(1);
            }
            /////////////////////////////////////////////////////////////////////
            /////////////////////////////////////////////////////////////////////
            else if (t.tag == "btn_back")
            {
                SceneManager.LoadScene("MainScene");
            }
            /////////////////////////////////////////////////////////////////////
            /////////////////////////////////////////////////////////////////////
            else if (t.tag == "yes")
            {
                Debug.Log("yes");
                testMessagePanel.SettingYesFunction();
                Controller.control_mode = LorR;
                Controller.changeControlMode();
            }
            else if (t.tag == "no")
            {
                Debug.Log("no");
                testMessagePanel.SettingNoFunction();
            }
            else if (t.tag == "yes_exit")
            {
                Debug.Log("yes");
                if (flag == 0) // 종료?
                {
                    testMessagePanel.PaintingCloseYesFunction();
                    flag = 1;
                }
                else if (flag == 1) // 저장?

                {
                    testMessagePanel.PaintingSaveYesFunction();
                    Painting.ExportTex();
                }
            }
            else if (t.tag == "no_exit")
            {
                Debug.Log("no");
                if (flag == 0)
                {
                    testMessagePanel.PaintingCloseNoFunction();
                }
                else if (flag == 1)
                {
                    testMessagePanel.PaintingSaveNoFunction();
                }
            }
            /////////////////////////////////////////////////////////////////////
            /////////////////////////////////////////////////////////////////////
            else
            {

            }
            tri = false;
        }
    }
}
