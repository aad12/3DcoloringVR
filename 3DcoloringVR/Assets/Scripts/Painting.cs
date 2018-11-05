using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.VR;
using System;
using System.IO;
using Importer;
using System.Globalization;

public class Painting : MonoBehaviour
{
    public GameObject flag;

    /* 페인팅 변수 */
    Int32 x1, x2, y1, y2; //마우스 위치 (1:현재 2:이전)
    Color[] paintingArea;
    Color32[] finalColor;

    /* 모델 변수 */
    public static string file_path; // = "Assets/Resources/Object/";
    public static string file_name; // = "Temple.obj";
    Importer.Importer imp;
    Mesh model_mesh;
    Vector2[] temp_uv;
    Renderer model_renderer;

    /* 모델 텍스쳐 */
    public static bool load = false;
    static Texture2D model_tex;
    int model_tex_x = 2048;
    int model_tex_y = 2048;
    

    /* 브러시 변수 */
    byte[] brush;
    string brush_path;
    string brush_name;
    
    List<Texture2D> brush_list; //원본 브러시들
    int brush_id = 1; //0:circle 1:rough
    Texture2D brush_tex; //현재 브러시 텍스쳐
    Color32[] brush_array; //현재 브러시 텍스쳐 픽셀 배열화
    Color32 brush_color; //현재 색상
    float brush_scale = 1.0f; //현재 브러쉬 크기
    bool brush_scale_changed = false;
    float brush_alpha = 0.1f; //브러시 투명도 (1.0f : 불투명)
    int brush_mode = 0; //블렌딩 모드 (0:일반 1:곱하기)
    
    /* 팔레트 활성화 플래그 */
    GameObject palette;
    bool pal_flag = true;

    /* 팝업창 관련 변수 선언 */
    private DisplayManager displayManager;

    /* 메시지 패널 관련 변수 선언 */
    private TestMessagePanel testMessagePanel;

    void Start()
    {
        flag = GameObject.FindGameObjectWithTag("flag");
        flag.SetActive(false);

        /* 모델 메쉬 정보 임포트 */
        imp = new Importer.Importer();
        imp.Import(file_path, file_name);
        model_mesh = imp.mesh;
        model_mesh.uv = imp.uv;
        temp_uv = imp.uv;

        /* 모델 텍스쳐 세팅 */
        model_tex = new Texture2D(model_tex_x, model_tex_y, TextureFormat.ARGB32, false);

        if (load == true)
        {
            string TexName = file_name.Substring(0, file_name.Length - 4) + ".png";

            model_tex.LoadImage(File.ReadAllBytes("C:/Users/user/Desktop/3d_obj/" + TexName)); 
        }
        else
        {
        }

        model_tex.wrapMode = TextureWrapMode.Clamp;
        model_renderer = GetComponent<Renderer>();
        model_renderer.material.mainTexture = model_tex;
        model_renderer.material.color = Color.white;

        /* UV 설정 */
        model_tex.Apply();
        drawTexPlane(); //텍스쳐 플레인 그릴때만
                
        /* 현재 브러시 세팅 */
        brush_tex = new Texture2D(64, 64, TextureFormat.ARGB32, false);
        brush_tex.LoadImage(File.ReadAllBytes("C:/Users/user/Desktop/brush/RoughBrush.png"));
        brush_tex.Apply();
        brush_color = Color.red;
        brushSizeApply();

        /* 팔레트 세팅 */
        palette = GameObject.FindGameObjectWithTag("palette");

        /* 팝업창 인스턴스 생성 */
        displayManager = DisplayManager.Instance();

        /* 메시지패널 인스턴스 생성 */
        testMessagePanel = TestMessagePanel.Instance();
    }

    // Update is called once per frame
    void Update()
    {
        /* 플래그값 기본값으로 상시 업뎃 */
        flag.GetComponent<Renderer>().material.SetColor("_Color", Color.gray);
        
        /* 저장 버튼(버튼 A) 눌렀을 때, 텍스쳐 Export 및 팝업창 생성 */
        if (OVRInput.GetDown(OVRInput.Button.One, Controller.brushController))
        {
            Debug.Log("저장 버튼 클릭");
            ExportTex();
            displayManager.DisplayMessage("저장이 완료되었습니다.");
        }
        
        /* 종료 버튼(버튼 B) 눌렀을 때, 메시지패널 생성*/
        if (OVRInput.GetDown(OVRInput.Button.Two, Controller.brushController))
        {
            Debug.Log("종료 버튼 클릭");
            testMessagePanel.TestYN(2);
        }

        /* 팔레트 활성/비활성 */
        if (OVRInput.GetDown(OVRInput.Button.One, Controller.toolController))
            hidePalette();
        
        /* 오른쪽 조이스틱 상하 : 브러시 크기 조절 */
        if (OVRInput.Get(Controller.brushThum).y > 0)
            brushSizeCal(0);
        else if (OVRInput.Get(Controller.brushThum).y < 0)
            brushSizeCal(1);
        if (OVRInput.Get(Controller.brushThum).y == 0 && brush_scale_changed)
            brushSizeApply();
        
        /* 페인팅 */
        if(Controller.control_mode == 0)
           CheckTarget(Controller.rhit);
        if (Controller.control_mode == 1)
            CheckTarget(Controller.lhit);
    }
    
    /* hit 있을때 태그로 대상 확인 */
    public void CheckTarget(RaycastHit hit)
    {
        try {
            switch (hit.transform.tag)
            {
                case "model":
                    flag.GetComponent<Renderer>().material.SetColor("_Color", Color.yellow);
                    stroke(hit);
                    break;

                case "palette":
                    flag.GetComponent<Renderer>().material.SetColor("_Color", Color.cyan);

                    brush_color = ColorSelector.GetColor();

                    for (int i = 0; i < brush_array.Length; i++)
                    {
                        if (brush_array[i] != Color.clear)
                            brush_array[i] = brush_color;
                    }

                    break;

                case "":
                    break;
            }
        }
        catch (NullReferenceException) {
        }
    }

    //브러시 - 디폴트 블렌딩
    void draw_default(int x, int y)
    {
        Color finColor = new Color(model_tex.GetPixel(x, y).r * (1 - brush_color.a) + brush_color.r * brush_color.a,
                                   model_tex.GetPixel(x, y).g * (1 - brush_color.a) + brush_color.g * brush_color.a,
                                   model_tex.GetPixel(x, y).b * (1 - brush_color.a) + brush_color.b * brush_color.a);
        model_tex.SetPixel(x, y, finColor);
    }

    /* 브러시 그리기 */
    void stroke(RaycastHit hit)
    {
        x2 = x1; //이전 위치
        y2 = y1;
        x1 = (Int32)(model_tex.width * hit.textureCoord.x); //현재 위치
        y1 = (Int32)(model_tex.height * hit.textureCoord.y);
        Debug.Log("aaaaaaaaaa");
        if (OVRInput.Get(Controller.brushIndex) > 0 && (x1 != x2) && (y1 != y2))
        {
            Debug.Log("!!!!!!!!");
            paintingArea = model_tex.GetPixels((Int32)(x1 - brush_tex.width / 2), (Int32)(y1 - brush_tex.height / 2), (Int32)brush_tex.width, (Int32)brush_tex.height);

            for (int i = 0; i < brush_array.Length; i++)
            {
                if (brush_array[i] == Color.clear)
                    finalColor[i] = paintingArea[i];
                else
                    finalColor[i] = Color32.Lerp(paintingArea[i], brush_array[i], brush_alpha);
            }

            model_tex.SetPixels32((Int32)(x1 - brush_tex.width / 2), (Int32)(y1 - brush_tex.height / 2), (Int32)brush_tex.width, (Int32)brush_tex.height, finalColor);
            
            model_tex.Apply();
        }
    }

    /* 메쉬 UV 설정 */
    void setModelUV(Mesh mesh)
    {
        mesh.uv = temp_uv;
    }

    /* 텍스쳐 내 폴리곤 경계선 그리기 */
    void drawTexLine(Texture2D tex, float u1, float v1, float u2, float v2, Color color)
    {
        int x1, x2, y1, y2;

        x1 = (int)(u1 * tex.width);
        y1 = (int)(v1 * tex.height);
        x2 = (int)(u2 * tex.width);
        y2 = (int)(v2 * tex.height);

        if (Math.Abs(x1 - x2) >= Math.Abs(y1 - y2))
        {//x 증가량이 더 많을때...p는 x q는 y
            if (x1 >= x2)
                for (int p = x2; p <= x1; p++)
                {
                    int q = (int)((float)(y2 - y1) / (x2 - x1) * (p - x1) + y1);
                    tex.SetPixel(p, q, color);
                }
            else
                for (int p = x2; p >= x1; p--)
                {
                    int q = (int)((float)(y2 - y1) / (x2 - x1) * (p - x1) + y1);
                    tex.SetPixel(p, q, color);
                }
        }
        else
        { //y 증가량이 더 많을때...//p는 y q는 x
            if (y1 >= y2)
                for (int p = y2; p <= y1; p++)
                {
                    int q = (int)((float)(x2 - x1) / (y2 - y1) * (p - y1) + x1);
                    tex.SetPixel(q, p, color);
                }
            else
                for (int p = y2; p >= y1; p--)
                {
                    int q = (int)((float)(x2 - x1) / (y2 - y1) * (p - y1) + x1);
                    tex.SetPixel(q, p, color);
                }
        }
    }

    /* 텍스쳐 평면 그리기 */
    void drawTexPlane()
    {
        GameObject TexPlane = GameObject.CreatePrimitive(PrimitiveType.Plane);
        TexPlane.transform.localScale = new Vector3(5, 5, 5);
        TexPlane.transform.Rotate(-90, 0, 0);
        TexPlane.transform.Translate(40, 0, 0);
        TexPlane.GetComponent<Renderer>().material.mainTexture = model_tex;
        TexPlane.GetComponent<Renderer>().material.color = Color.white;
        model_tex.wrapMode = TextureWrapMode.Clamp;

        for (int i = 0; i < temp_uv.Length; i += 3)
        {
            drawTexLine(model_tex, temp_uv[i].x, temp_uv[i].y, temp_uv[i + 1].x, temp_uv[i + 1].y, Color.gray);
            drawTexLine(model_tex, temp_uv[i + 1].x, temp_uv[i + 1].y, temp_uv[i + 2].x, temp_uv[i + 2].y, Color.gray);
            drawTexLine(model_tex, temp_uv[i + 2].x, temp_uv[i + 2].y, temp_uv[i].x, temp_uv[i].y, Color.gray);
        }
        model_tex.Apply();
        
        TexPlane.SetActive(false);
    }

    /* 텍스쳐 png로 익스포트 */
    public static void ExportTex()
    {
        byte[] exportedTex = model_tex.EncodeToPNG();
        string texName = file_name.Split('.')[0];

        File.WriteAllBytes(file_path + texName + ".png", exportedTex);
    }

    /* 팔레트 활성화 비활성화 */
    void hidePalette()
    {
        if (pal_flag)
        {
            palette.SetActive(false);
            pal_flag = false;
        }
        else
        {
            palette.SetActive(true);
            pal_flag = true;
        }
    }

    /* 브러시 스케일 값 변경 */
    void brushSizeCal(int mode)
    {
        if (mode == 0) //확대
            brush_scale += Time.deltaTime;
        else if (mode == 1)//축소
            brush_scale -= Time.deltaTime;
        if (brush_scale <= 0)
            brush_scale = 0.2f;

        brush_scale_changed = true;
    }
    
    /* 브러시 텍스쳐 사이즈 변경 */
    void brushSizeApply()
    {
        brush_tex = new Texture2D(64, 64, TextureFormat.ARGB32, false);
        brush_tex.LoadImage(File.ReadAllBytes("C:/Users/user/Desktop/brush/RoughBrush.png"));

        if ((int)(brush_tex.width * brush_scale) < 5 || (int)(brush_tex.height * brush_scale) < 5)
            TextureScale.Bilinear(brush_tex, 5, 5);
        else
            TextureScale.Bilinear(brush_tex, (int)(brush_tex.width * brush_scale), (int)(brush_tex.height * brush_scale));

        brush_array = brush_tex.GetPixels32();

        for (int i = 0; i < brush_array.Length; i++)
        {
            if (brush_array[i] != Color.clear)
                brush_array[i] = brush_color;
        }

        finalColor = new Color32[brush_array.Length];
        brush_scale_changed = false;
    }
}
