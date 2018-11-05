using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class TestMessagePanel : MonoBehaviour
{
    private MessagePanel messagePanel;
    private DisplayManager displayManager;

    private UnityAction mySettingYesAction;
    private UnityAction mySettingNoAction;
    private UnityAction myPaintingCloseYesAction;
    private UnityAction myPaintingCloseNoAction;
    private UnityAction myPaintingSaveYesAction;
    private UnityAction myPaintingSaveNoAction;

    private static TestMessagePanel testMessagePanel;

    public static TestMessagePanel Instance()
    {
        if (!testMessagePanel)
        {
            testMessagePanel = FindObjectOfType(typeof(TestMessagePanel)) as TestMessagePanel;

            if (!testMessagePanel)
                Debug.LogError("오브젝트에 TestMessagePanel 스크립트 없음");
        }

        return testMessagePanel;
    }

    // Use this for initialization
    void Start()
    {
        messagePanel = MessagePanel.Instance();
        displayManager = DisplayManager.Instance();

        mySettingYesAction = new UnityAction(SettingYesFunction);
        mySettingNoAction = new UnityAction(SettingNoFunction);
        myPaintingCloseYesAction = new UnityAction(PaintingCloseYesFunction);
        myPaintingCloseNoAction = new UnityAction(PaintingCloseNoFunction);
        myPaintingSaveYesAction = new UnityAction(PaintingSaveYesFunction);
        myPaintingSaveNoAction = new UnityAction(PaintingSaveNoFunction);
    }

    // 메시지패널에 텍스트와 버튼 함수 전달
    public void TestYN(int tag)
    {
        switch (tag)
        {
            // Setting Scene
            case 0:
                messagePanel.Choice("왼손 모드로 컨트롤러 설정을 완료하시겠습니까?", mySettingYesAction, mySettingNoAction);
                break;
            case 1:
                messagePanel.Choice("오른손 모드로 컨트롤러 설정을 완료하시겠습니까?", mySettingYesAction, mySettingNoAction);
                break;
            // Painting Scene
            case 2:
                messagePanel.Choice("종료하시겠습니까?", myPaintingCloseYesAction, myPaintingCloseNoAction);
                break;
            case 3:
                messagePanel.Choice("변경 내용을 저장하시겠습니까?", myPaintingSaveYesAction, myPaintingSaveNoAction);
                break;
        }
    }

    // Setting Scene 팝업창에서의 yes 함수
    public void SettingYesFunction()
    {
        displayManager.DisplayMessage("컨트롤러 설정이 완료되었습니다.");
        messagePanel.ClosePanel();
        Invoke("BackToMain", 1); // 1초 뒤 BackToMain 함수 호출
    }

    // Setting Scene 팝업창에서의 no 함수
    public void SettingNoFunction()
    {
        displayManager.DisplayMessage("컨트롤러 설정이 취소되었습니다.");
        messagePanel.ClosePanel();
    }

    // Painting Scene 종료 팝업창에서의 yes 함수
    public void PaintingCloseYesFunction()
    {
        TestYN(3);
    }

    // Painting Scene 종료 팝업창에서의 no 함수
    public void PaintingCloseNoFunction()
    { 
        displayManager.DisplayMessage("취소되었습니다.");
        messagePanel.ClosePanel();
    }

    // Painting Scene 저장 팝업창에서의 yes 함수
    public void PaintingSaveYesFunction()
    {
        displayManager.DisplayMessage("저장이 완료되었습니다.");
        Invoke("BackToMain", 1); // 1초 뒤 BackToMain 함수 호출
    }

    // Painting Scene 저장 팝업창에서의 no 함수
    public void PaintingSaveNoFunction()
    {
        BackToMain();
    }

    public void BackToMain()
    {
        SceneManager.LoadScene("MainScene");
    }
}
