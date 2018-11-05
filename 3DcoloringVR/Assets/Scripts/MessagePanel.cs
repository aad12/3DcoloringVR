using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class MessagePanel : MonoBehaviour
{
    public Text txt;
    public Button btn_yes;
    public Button btn_no;
    public GameObject messagePanelObject;

    private static MessagePanel messagePanel;

    public static MessagePanel Instance()
    {
        if (!messagePanel)
        {
            messagePanel = FindObjectOfType(typeof(MessagePanel)) as MessagePanel;
            if (!messagePanel)
                Debug.LogError("오브젝트에 MessagePanel 스크립트 없음");
        }

        return messagePanel;
    }

    public void Choice(string txt, UnityAction yesEvent, UnityAction noEvent)
    {
        messagePanelObject.SetActive(true);
        
        this.txt.text = txt;
        btn_yes.gameObject.SetActive(true);
        btn_no.gameObject.SetActive(true);
     }

    public void ClosePanel()
    {
       messagePanelObject.SetActive(false);
    }
}
