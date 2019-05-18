using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class carLightRotate : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	private void FixedUpdate()
	{
        transform.Rotate(Vector3.up, Random.Range(5,10));
	}
}
