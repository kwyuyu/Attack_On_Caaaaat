using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Traffic : MonoBehaviour {

    private Transform start;
    private Transform end;
    private GameObject Car;

    private int count;
    public float speed = 0.5f;
    public int appear_chance = 50;

    private float cur_speed = 0f;
    private string[] cars = new string[] { "Interceptor", "Car_1", "Car_2", "Car_3", "Car_4", "Car_5", "Car_6", "Constructor_run", "Constructor_jump"};
    private float[] car_speeds = new float[] { 0.3f, 0.1f, 0f, -0.2f, 0f, -0.5f, 0.3f, 0f, 0f };


	// Use this for initialization
	void Start () {
        Random.seed = (int)System.DateTime.Now.Ticks;
        start = this.transform.Find("Start");
        end = this.transform.Find("End");
        count = 0;
	}
	
    // Update is called once per frame
    void Update()
    {
        if (count == 0)
        {
            if (Random.Range(0, 100) >= appear_chance) return;

            int selected_id = Random.Range(0, cars.Length);
            string car_name = string.Format("/SceneItems/Template/Cars/{0}", cars[selected_id]);
            cur_speed = speed + car_speeds[selected_id];

            GameObject CarTemplate = GameObject.Find(car_name).gameObject;

            start.position = new Vector3(start.position.x, CarTemplate.transform.position.y, start.position.z);
            end.position = new Vector3(end.position.x, CarTemplate.transform.position.y, end.position.z);
            Vector3 heading_vec = (end.position - start.position).normalized;

            Quaternion heading = Quaternion.FromToRotation(CarTemplate.transform.forward, heading_vec);


            Car = Instantiate(CarTemplate, start.position, heading, this.transform);
            Car.SetActive(true);
            count += 1;
        }
    }

    private void FixedUpdate()
    {
        if (count != 0)
        {
            Car.transform.position += (end.position - start.position).normalized * cur_speed;

            if ((Car.transform.position - end.position).magnitude <= 1f)
            {
                Destroy(Car);
                count = 0;
            }
        }
    }
}
