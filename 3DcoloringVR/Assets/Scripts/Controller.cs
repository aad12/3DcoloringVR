using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Controller : MonoBehaviour
{
    public static OVRInput.Controller brushController;
    public static OVRInput.Axis1D brushIndex;
    public static OVRInput.Axis1D brushHand;
    public static OVRInput.Axis2D brushThum;//.SecondaryThumbstick

    public static OVRInput.Controller toolController;
    public static OVRInput.Axis1D toolIndex;
    public static OVRInput.Axis1D toolHand;
    public static OVRInput.Axis2D toolThum;

    /* 0:오른손잡이, 1: 왼손잡이 */
    public static int control_mode;

    /* 0:오른쪽 컨트롤러, 1: 왼쪽 컨트롤러 */
    public static int brush_cont = 0;
    public static int tool_cont = 1;

    //public static int control_num = 0;

    /* 커서 변수 */
    public static LineRenderer lr;
    public static GameObject Rhand;
    public static GameObject Lhand;
    public static RaycastHit rhit;
    public static RaycastHit lhit;
    public static Ray rray;
    public static Ray lray;

    /* 스틱 변수 */
    public static GameObject stick;

    static Controller myslf;

    void Awake()
    {
        myslf = this;
    }

    void Start()
    {
        changeControlMode();
        
        lr = GameObject.FindGameObjectWithTag("control").GetComponent<LineRenderer>();
        
        /* 오른쪽 컨트롤러 세팅*/
        Rhand = GameObject.FindGameObjectWithTag("RightHand");
        Rhand.transform.localEulerAngles = new Vector3(0, 180f, 0);

        /* 왼쪽 컨트롤러 세팅*/
        Lhand = GameObject.FindGameObjectWithTag("LeftHand");
        Lhand.transform.localEulerAngles = new Vector3(0, 180f, 0);
        
        /* 레이저빔 세팅 */
        rray = new Ray();
        lray = new Ray();

        /* 스틱 세팅*/
        stick = GameObject.CreatePrimitive(PrimitiveType.Cube);
        stick.transform.localScale = new Vector3(0.025f, 0.025f, 0.3f);
        stick.GetComponent<Renderer>().material.color = Color.red;
    }

    void Update()
    {
        rray.origin = Rhand.transform.position;
        rray.direction = Rhand.transform.eulerAngles;

        lray.origin = Lhand.transform.position;
        lray.direction = Lhand.transform.eulerAngles;

        RayCast(control_mode);
        drawStick(control_mode);
    }

    public static void changeControlMode()
    {
        /* 오른손잡이: 브러시 오른쪽 툴 왼쪽 */
        if (control_mode == 0)
        {
            brushController = OVRInput.Controller.RTouch;
            brushIndex = OVRInput.Axis1D.SecondaryIndexTrigger;
            brushHand = OVRInput.Axis1D.SecondaryHandTrigger;
            brushThum = OVRInput.Axis2D.SecondaryThumbstick;

            toolController = OVRInput.Controller.LTouch;
            toolIndex = OVRInput.Axis1D.PrimaryIndexTrigger;
            toolHand = OVRInput.Axis1D.PrimaryHandTrigger;
            toolThum = OVRInput.Axis2D.PrimaryThumbstick;
        }
        /* 왼손잡이: 브러시 왼쪽 툴 오른쪽 */
        else if (control_mode == 1)
        {
            toolController = OVRInput.Controller.RTouch;
            toolIndex = OVRInput.Axis1D.SecondaryIndexTrigger;
            toolHand = OVRInput.Axis1D.SecondaryHandTrigger;
            toolThum = OVRInput.Axis2D.SecondaryThumbstick;

            brushController = OVRInput.Controller.LTouch;
            brushIndex = OVRInput.Axis1D.PrimaryIndexTrigger;
            brushHand = OVRInput.Axis1D.PrimaryHandTrigger;
            brushThum = OVRInput.Axis2D.PrimaryThumbstick;
        }
    }

    /* 컨트롤러 레이캐스팅 */
    public static void RayCast(int control_num)
    {
        /* 오른쪽 컨트롤러 */
        if (control_num == 0) {

            /* hit 없을때 허공에 레이저 */
            if (!Physics.Raycast(Rhand.transform.position, Rhand.transform.forward, out rhit, Mathf.Infinity))
            {
                
                drawLaser(rray);
                return;
            }
            /* hit 있을때 그 지점에 레이저 & 충돌 물체 확인 */
            else
            {
                drawLaser(rhit, rray);
            }
        }
        /* 왼쪽 컨트롤러 */
        else if (control_num == 1) {

            /* hit 없을때 허공에 레이저 */
            if (!Physics.Raycast(Lhand.transform.position, Lhand.transform.forward, out lhit, Mathf.Infinity))
            {
                drawLaser(lray);
                return;
            }
            /* hit 있을때 그 지점에 레이저 & 충돌 물체 확인 */
            else
            {
                drawLaser(lhit, lray);
            }
        }
        return;
    }
    

    /* 스틱 그리기 */
    public static void drawStick(int control_num)
    {
        /* 오른쪽 컨트롤러에 */
        if (control_num == 0) {
            stick.transform.position = Rhand.transform.position;
            stick.transform.localEulerAngles = Rhand.transform.localEulerAngles;
        }
        /* 왼쪽 컨트롤러에 */
        else if (control_num == 1)
        {
            stick.transform.position = Lhand.transform.position;
            stick.transform.localEulerAngles = Lhand.transform.localEulerAngles;
        }
    }

    /* 레이저 그리기 : 충돌지점 있을때 */
    public static void drawLaser(RaycastHit hit, Ray ray)
    {
        lr.startColor = Color.red;
        lr.startWidth = 0.005f;
        lr.endColor = Color.blue;
        lr.endWidth = 0.005f;

        lr.SetPosition(0, ray.origin);
        lr.SetPosition(1, hit.point);
    }

    /* 레이저 그리기 : 충돌지점 없을때 */
    public static void drawLaser(Ray ray)
    {
        lr.startColor = Color.red;
        lr.startWidth = 0.005f;
        lr.endColor = Color.blue;
        lr.endWidth = 0.005f;

        lr.SetPosition(0, ray.origin);
        if(control_mode == 0)
            
            lr.SetPosition(1, ray.origin + 50 * Rhand.transform.forward);
        else
            lr.SetPosition(1, ray.origin + 50 * Lhand.transform.forward);
    }
}
