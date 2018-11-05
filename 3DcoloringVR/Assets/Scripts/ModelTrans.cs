using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelTrans : MonoBehaviour
{
    Vector3 past, now;

    GameObject Lhand;
    GameObject Rhand;

    // Use this for initialization
    void Start()
    {
        Rhand = GameObject.FindGameObjectWithTag("RightHand");
        Rhand.transform.localEulerAngles = new Vector3(0, 180f, 0);
        Lhand = GameObject.FindGameObjectWithTag("LeftHand");
        Rhand.transform.localEulerAngles = new Vector3(0, 180f, 0);

        past = new Vector3();
        now = new Vector3();
        now = Lhand.transform.position;

        transform.localScale = new Vector3(10f, 10f, 10f);
    }

    // Update is called once per frame
    void Update()
    {
        past = now;

        if (Controller.control_mode == 0)
            now = Lhand.transform.position;
        else if (Controller.control_mode == 1)
            now = Rhand.transform.position;


        /* 왼쪽 인덱스 트리거 : 이동 */
        if ((OVRInput.Get(Controller.toolIndex) > 0))
            model_trans();

        /* 왼쪽 핸드 트리거 : 회전 */
        if (OVRInput.Get(Controller.toolHand) > 0)
            model_rotate();

        /* 왼쪽 조이스틱 상하 : 크기 조절 */
        if (OVRInput.Get(Controller.toolThum).y > 0)
            model_scale(0);
        else if (OVRInput.Get(Controller.toolThum).y < 0)
            model_scale(1);
    }

    /* 모델 이동 */
    void model_trans()
    {
        transform.Translate(new Vector3(0.1f, 0, 0) * 1000 * (now.x - past.x), Space.World);
        transform.Translate(new Vector3(0, 0.1f, 0) * 1000 * (now.y - past.y), Space.World);
        transform.Translate(new Vector3(0, 0, 0.1f) * 1000 * (now.z - past.z), Space.World);
    }

    /* 모델 회전 */
    void model_rotate()
    {
        transform.Rotate(Vector3.up, Time.deltaTime * 50000 * (past.x - now.x), Space.World); //좌우로 회전
        transform.Rotate(Vector3.left, Time.deltaTime * 50000 * (past.y - now.y), Space.World); //상하로 회전
    }

    /* 모델 확대 축소 */
    void model_scale(int mode)
    {
        if (mode == 0) //확대
            transform.localScale = transform.localScale + new Vector3(Time.deltaTime, Time.deltaTime, Time.deltaTime) * 100f;
        else if (mode == 1)
        { //축소
            transform.localScale = transform.localScale - new Vector3(Time.deltaTime, Time.deltaTime, Time.deltaTime) * 100f;
            if (transform.localScale.x < 0) transform.localScale = new Vector3(0, 0, 0);
        }
    }
}
