using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaletteTrans : MonoBehaviour {

    GameObject pal, hand; 

	// Use this for initialization
	void Start ()
    {
		pal = GameObject.FindGameObjectWithTag("palette");

        if(Controller.control_mode == 0)
           hand = GameObject.FindGameObjectWithTag("LeftHand");
        else if (Controller.control_mode == 1)
            hand = GameObject.FindGameObjectWithTag("RightHand");
    }
	
	// Update is called once per frame
	void Update ()
    {
        pal.transform.localScale = new Vector3(0.7f, 0.7f, 0.7f);
        pal.transform.position = hand.transform.position + new Vector3(0,0.2f,0) ;
        pal.transform.localEulerAngles = hand.transform.localEulerAngles;// + new Vector3(0, -30, 0);
    }
}
