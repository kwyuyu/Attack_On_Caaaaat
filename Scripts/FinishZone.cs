using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishZone : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log(other.name);
        //Debug.Log("Game Condition Reached!");
        if (other.name == "head") return;

        if(!GameObject.Find("/"+this.name +"_finished")){
            GameObject finishFlag = new GameObject(other.name + "_finished");
            finishFlag.transform.parent = null;
        }
    }
}
