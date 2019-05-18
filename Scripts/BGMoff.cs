using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMoff : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if(GameObject.Find("/Character1_lost") && GameObject.Find("/Character2_lost") )
            GameObject.Find("BGM").SetActive(false);
	}
}
