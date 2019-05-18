using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FinalStep : MonoBehaviour {

    private GameObject WhoWin;
    private Camera mainCamera;
    private GameObject RemainInfo;

	// Use this for initialization
	void Start () {

        WhoWin = GameObject.Find("Canvas/WhoWin");
        mainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
        RemainInfo = GameObject.Find("Remain_info");

        WhoWin.GetComponent<Text>().text = "Congratulation!\n" + RemainInfo.transform.Find("Winner").gameObject.GetComponent<Text>().text;
		
	}
	
	// Update is called once per frame
	void Update () {
        WhoWin.transform.localPosition = new Vector3(0f, mainCamera.pixelHeight / 4f, 0f);
		
	}
}
