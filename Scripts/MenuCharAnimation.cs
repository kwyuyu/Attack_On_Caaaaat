using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuCharAnimation : MonoBehaviour {
    private Animator m_animator;
    private float duration;
    private bool isWalking;
    private int turn;

	void Start () 
    {
        Random.seed = (int)System.DateTime.Now.Ticks;
        m_animator = GetComponent<Animator>();
        duration = 0;
        isWalking = false;
        turn = 0;
	}
	
    void FixedUpdate()
    {
        duration -= Time.deltaTime;
        if(duration <= 0f)
        {
            isWalking = (Random.Range(0, 2) == 0) ? false : true;
            duration = Random.Range(0f, 2f);
            m_animator.SetBool("isWalking", isWalking);
            if(isWalking)
            {
                transform.Rotate(Vector3.up, 90f);
            }
        }

        if(isWalking){
            transform.Translate(Vector3.left * 1.5f * Time.deltaTime);
        }
	}
}
