using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class CharacterMotion : MonoBehaviour
{
    public int player = 1;
    public string controller;
    public LayerMask selectable;
    public GameObject Character;

    private GameObject canvas_aim;
    private GameObject head;
    private GameObject anchor_template;
    private GameObject anchor;
    private GameObject character_prefab;
    private Collider head_collider;
    private Rigidbody rb;

    private Transform neck;
    private Transform characterObj;
    private Camera mainCamera;

    private Animator m_animator;
    private AudioSource shoot_sound;

    // Constants
    private Vector3 NULL = -Vector3.one;
    enum State { START, STRETCHING, UNSTRETCHING_SUCCESS, UNSTRETCHING_FAIL, NECK_STILL, FAILED };

    // Read once variable
    private float original_neck_length;
    private Vector3 original_neck_vector;
    private Quaternion original_cat_quaternion;

    // Variables
    private Vector3 mid_point;
    private string difficulty;

    private State cur_state;
    private Vector3 velocity_forward;
    private Vector3 velocity_fall;
    private Vector3 neck_vector; // magnitude is the length, normal vector is the direction
    private Vector3 body_dir;
    private Vector3 target;
    private Vector3 new_target;
    private Vector3 Target_oncanvas;
    private bool hasnext = false;
    private bool collWithPlane = false;

    private bool start_timer;
    private bool end_game;

    // Changable parameters
    private float MAX_STRETCH = 32f;
    private float stretch_speed = 52f;
    private float unstretching_failed_speed = 42f;
    private float unstretching_success_speed = 30f; // This is also the speed that fly away, Should not be larger than stretch
    private Vector3 falling_accel = new Vector3(0f, -3.8f, 0f);


    void Start()
    {
        // find object
        mainCamera = Character.transform.Find("Pivot/Camera").gameObject.GetComponent<Camera>();
        character_prefab = Character.transform.Find("Cat").gameObject;
        m_animator = character_prefab.GetComponent<Animator>();
        characterObj = Character.transform.Find("Cat/Rig_Cat_Lite");
        neck = Character.transform.Find("Cat/Rig_Cat_Lite/Master/BackBone_03/BackBone_02/BackBone_01/Neck");
        canvas_aim = Character.transform.Find("Canvas/Aim").gameObject;
        head = Character.transform.Find("Cat/Rig_Cat_Lite/Master/BackBone_03/BackBone_02/BackBone_01/Neck/Head").gameObject;
        anchor_template = Character.transform.Find("AnchorTemplate").gameObject;
        head_collider = head.GetComponent<Collider>();
        rb = GetComponent<Rigidbody>();
        original_cat_quaternion = characterObj.transform.rotation;
        shoot_sound = character_prefab.transform.GetComponent<AudioSource>();

        // set difficulty
        difficulty = GameObject.Find("Remain_info/Difficulty_value").GetComponent<Text>().text;
        if (difficulty == "easy")
        {
            falling_accel = new Vector3(0f, -3.8f, 0f);
            //unstretching_failed_speed = 37f;
            //unstretching_success_speed = 21f;
            //stretch_speed = 46f;

        }
        else if (difficulty == "normal")
        {
            falling_accel = new Vector3(0f, -12f, 0f);
            //unstretching_failed_speed = 56f;
            //unstretching_success_speed = 35f;
            //stretch_speed = 65f;

        }
        else if (difficulty == "hard")
        {
            falling_accel = new Vector3(0f, -14f, 0f);
            //unstretching_failed_speed = 56f;
            //unstretching_success_speed = 35f;
            //stretch_speed = 65f;

        }


        canvas_aim.transform.localPosition = GetCanvasAimPosition();
        canvas_aim.GetComponent<Text>().text = "+";

        UpdateMidPoint();
        Target_oncanvas = mid_point;


        Vector3 dir = (head.transform.position - neck.position).normalized;
        float dist = (head.transform.position - neck.position).magnitude;
        original_neck_length = dist;
        neck_vector = dir * dist;
        original_neck_vector = neck_vector;


        body_dir = Vector3.left; // -1, 0, 0

        cur_state = State.START;
        new_target = NULL;
    }


    // ========================= //
    //  Head Collision Handle    //
    // ========================= //
    void OnCollisionEnter(Collision collision)
    {
        collWithPlane = false;
        foreach (ContactPoint contact in collision.contacts)
        {
            //Debug.Log(contact.thisCollider);
            //Debug.Log(collision.collider.name);
            //Debug.Log(cur_state);

            collWithPlane |= collision.collider.name == "Plane";

            if (contact.thisCollider == head_collider && cur_state == State.STRETCHING)
            {
                cur_state = State.UNSTRETCHING_SUCCESS;
                GameObject ObjectAttached = GameObject.Find(collision.collider.name); // should be fixed: collision.collider.name
                anchor = Instantiate(anchor_template, contact.point, Quaternion.identity, ObjectAttached.transform);
            }
        }
    }


    // ================= //
    //  Update State     //
    // ================= //
    void Update()
    {
        start_timer = Character.transform.Find("Canvas/Score").GetComponent<Score>().start_timer;
        end_game = Character.transform.Find("Canvas/Score").GetComponent<Score>().end_game;
        UpdateMidPoint();

        // tartget coordinate on canvas
        Target_oncanvas = mid_point;

        // character state control
        UpdateState(Time.deltaTime);
    }

    void UpdateState(float dt)
    {
        m_animator.SetBool("isFlying", false);
        //Debug.Log(cur_state);
        switch (cur_state)
        {
            case State.START:
                velocity_fall = Vector3.zero;
                velocity_forward = Vector3.zero;

                if (!end_game && start_timer && ((controller == "mouse" && Input.GetKeyDown("space")) || (controller == "direction" && Input.GetKeyDown("m")) || (controller == "wasd" && Input.GetKeyDown("z"))))
                {
                    shoot_sound.Play();
                    cur_state = State.STRETCHING;

                    target = FindHitPointFrom(Target_oncanvas);
                    new_target = NULL;

                    rotate_body_along_with_target();

                    m_animator.SetBool("started", true);
                }

                break;
            case State.STRETCHING:
                if (target == NULL)
                {
                    //Debug.Log("unexpected error in State.STRETCHING");
                }

                // speed
                //velocity_forward don't change
                //velocity_gravity = falling_speed;

                // neck
                float next_neck_length = get_cur_neck_length() + stretch_speed * dt;
                Vector3 dir = (target - neck.position).normalized;
                float dist = (target - neck.position).magnitude; // unknown


                if (!end_game && start_timer && ((controller == "mouse" && Input.GetKeyDown("space")) || (controller == "direction" && Input.GetKeyDown("m")) || (controller == "wasd" && Input.GetKeyDown("z"))))
                {
                    shoot_sound.Play();
                    cur_state = State.UNSTRETCHING_FAIL;
                    new_target = FindHitPointFrom(Target_oncanvas);
                    hasnext = true;

                    rotate_body_along_with_target();
                }
                else if (next_neck_length >= MAX_STRETCH)
                {
                    cur_state = State.UNSTRETCHING_FAIL;
                }
                else
                {
                    neck_vector = dir * next_neck_length;
                }

                break;
            case State.UNSTRETCHING_FAIL:
                // speed
                //velocity_fall = falling_speed;

                // neck
                next_neck_length = get_cur_neck_length() - unstretching_failed_speed * dt;
                dir = (target - neck.position).normalized;

                if (!end_game && start_timer && ((controller == "mouse" && Input.GetKeyDown("space")) || (controller == "direction" && Input.GetKeyDown("m")) || (controller == "wasd" && Input.GetKeyDown("z"))))
                {
                    shoot_sound.Play();
                    new_target = FindHitPointFrom(Target_oncanvas);
                    hasnext = true;

                    rotate_body_along_with_target();
                }
                else if (next_neck_length <= original_neck_length)
                {
                    cur_state = State.NECK_STILL;

                    target = NULL;
                }
                else
                {
                    neck_vector = dir * next_neck_length;
                }

                break;
            case State.UNSTRETCHING_SUCCESS:
                
                // check if anchor is destroied
                if (anchor == null)
                {
                    target = NULL;
                    cur_state = State.NECK_STILL;
                }
                    

                // speed
                Vector3 speed_dir = (target - neck.position).normalized;
                velocity_fall = Vector3.zero;
                velocity_forward = unstretching_success_speed * speed_dir;

                // neck
                Vector3 neck_dir = (anchor.transform.position - neck.position).normalized; // anchor will stick on object hit
                next_neck_length = get_cur_neck_length() - unstretching_success_speed * dt;

                if (!end_game && start_timer && ((controller == "mouse" && Input.GetKeyDown("space")) || (controller == "direction" && Input.GetKeyDown("m")) || (controller == "wasd" && Input.GetKeyDown("z"))))
                {
                    shoot_sound.Play();
                    Destroy(anchor);
                    new_target = FindHitPointFrom(Target_oncanvas);
                    cur_state = State.UNSTRETCHING_FAIL;

                    hasnext = true;

                    rotate_body_along_with_target();
                }
                else if (next_neck_length <= original_neck_length || InDistance2D(target, neck.position, 0.2f))
                {
                    Destroy(anchor);
                    cur_state = State.NECK_STILL;

                    // when character return to still state, don't keep flying up.
                    velocity_forward = new Vector3(velocity_forward.x, 0f, velocity_forward.z);

                    target = NULL;
                }
                else
                {
                    neck_vector = neck_dir * next_neck_length;
                }

                break;
            case State.NECK_STILL:
                m_animator.SetBool("isFlying", true);

                // speed
                //velocity_forward don't change
                neck_vector = original_neck_vector; // reset neck

                if (target != NULL) Debug.Log("error in still");

                if (!end_game && start_timer && ((controller == "mouse" && Input.GetKeyDown("space")) || (controller == "direction" && Input.GetKeyDown("m")) || (controller == "wasd" && Input.GetKeyDown("z"))))
                {
                    shoot_sound.Play();
                    target = FindHitPointFrom(Target_oncanvas);
                    cur_state = State.STRETCHING;

                    rotate_body_along_with_target();
                }
                else if (hasnext)
                {
                    target = new_target;
                    cur_state = State.STRETCHING;
                    hasnext = false;
                }

                break;
        }
    }


    // ================= //
    //  Update physics   //
    // ================= //
    void FixedUpdate()
    {

        if(collWithPlane && !GameObject.Find("/" +this.name + "_finished") && !GameObject.Find("/" +this.name + "_lost")){
            GameObject lostFlag = new GameObject(this.name + "_lost");
            lostFlag.transform.parent = null;
            m_animator.SetBool("done", false);
            m_animator.SetBool("isFlying", true);
            m_animator.SetBool("started", false);
        }

        if (cur_state == State.NECK_STILL && collWithPlane) {
            characterObj.transform.rotation = original_cat_quaternion;
            characterObj.transform.Rotate(0f, 90f, 0f);
            return;
        }

        // update neck direction and length
        UpdateNeck(Time.deltaTime);

        // update whole character position with velocity
        UpdateCharater(Time.deltaTime);
    }

    void UpdateCharater(float dt)
    {
        rb.velocity = velocity_forward;

        // use gravity only in these state
        if(cur_state == State.STRETCHING || cur_state == State.UNSTRETCHING_FAIL || cur_state == State.NECK_STILL){
            Vector3 nextPos = Character.transform.position + velocity_fall * dt;
            if (nextPos.y <= 0)
            {
                collWithPlane = true;
                velocity_fall = Vector3.zero;
                rb.velocity = Vector3.zero;
            }
            else
            {
                Character.transform.position += velocity_fall * dt;
                velocity_fall += falling_accel * dt; // update fall speed with falling_accel
            }
        }

        // align body with assigned direction
        float body_rotate_speed = 0.1f;
        RotateWithAnimation(characterObj, -characterObj.up, body_dir, body_rotate_speed); // right is because cat right is forward
    }

    void UpdateNeck(float dt)
    {
        head.transform.position = neck.position + neck_vector;
    }


    // ========== //
    //  Utility   //
    // ========== //
    void rotate_body_along_with_target()
    {
        if (hasnext)
        {
            body_dir = (new_target - characterObj.transform.position).normalized;
        }
        else
        {
            body_dir = (target - characterObj.transform.position).normalized;
        }
    }

    float get_cur_neck_length()
    {
        return (head.transform.position - neck.position).magnitude;
    }

    void RotateWithAnimation(Transform obj_transform, Vector3 ref_vecter, Vector3 direction_vector, float speed)
    {
        Quaternion to = Quaternion.FromToRotation(ref_vecter, direction_vector);
        obj_transform.rotation = Quaternion.Slerp(obj_transform.rotation, to * obj_transform.rotation, speed);
    }

    Vector3 FindHitPointFrom(Vector3 input)
    {
        Vector3 res = NULL;

        Ray ray = mainCamera.ScreenPointToRay(input);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, selectable))
        {
            res = hit.point;
        }
        else
        {
            //Debug.Log("hit nothing");
        }
        return res;
    }

    bool InDistance2D(Vector3 a, Vector3 b, float dist)
    {
        Vector2 vec = new Vector2((a.x - b.x), (a.y - b.y));
        return vec.magnitude < dist;
    }


    private Vector3 GetCanvasAimPosition()
    {
        Vector3 aim_pos = Vector3.zero;

        if (GameObject.Find("Remain_info/Split_method") &&
            GameObject.Find("Remain_info/Split_method").GetComponent<Text>().text == "LR")
        {
            if (player == 1)
            {
                mainCamera.rect = new Rect(0f, 0f, 0.5f, 1f);
                //aim_pos = new Vector3(mainCamera.pixelWidth / 2f, mainCamera.pixelHeight / 2f, 0f);
                aim_pos = new Vector3(-mainCamera.pixelWidth / 2f, 0f, 0f);
            }
            else if (player == 2)
            {
                mainCamera.rect = new Rect(0.5f, 0f, 0.5f, 1f);
                //aim_pos = new Vector3(mainCamera.pixelWidth * 3 / 2f, mainCamera.pixelHeight / 2f, 0f);
                aim_pos = new Vector3(mainCamera.pixelWidth / 2f, 0f, 0f);
            }
        }
        else if (GameObject.Find("Remain_info/Split_method") &&
                 GameObject.Find("Remain_info/Split_method").GetComponent<Text>().text == "UD")
        {
            if (player == 1)
            {
                mainCamera.rect = new Rect(0f, 0.5f, 1f, 0.5f);
                //aim_pos = new Vector3(mainCamera.pixelWidth / 2f, mainCamera.pixelHeight * 3 / 2f, 0f);
                aim_pos = new Vector3(0f, mainCamera.pixelHeight / 2f, 0f);
            }
            else if (player == 2)
            {
                mainCamera.rect = new Rect(0f, 0f, 1f, 0.5f);
                //aim_pos = new Vector3(mainCamera.pixelWidth / 2f, mainCamera.pixelHeight / 2f, 0f);
                aim_pos = new Vector3(0f, -mainCamera.pixelHeight / 2f, 0f);
            }
        }
        else
        {
            mainCamera.rect = new Rect(0f, 0f, 1f, 1f);
            aim_pos = Vector3.zero;
        }

        return aim_pos;
    }


    private void UpdateMidPoint()
    {

        if (GameObject.Find("Remain_info/Split_method") && 
            GameObject.Find("Remain_info/Split_method").GetComponent<Text>().text == "LR")
        {
            if (player == 1)
            {
                mainCamera.rect = new Rect(0f, 0f, 0.5f, 1f);
                mid_point = new Vector3(mainCamera.pixelWidth / 2f, mainCamera.pixelHeight / 2f, 0f);
            }
            else if (player == 2)
            {
                mainCamera.rect = new Rect(0.5f, 0f, 0.5f, 1f);
                mid_point = new Vector3(mainCamera.pixelWidth * 3 / 2f, mainCamera.pixelHeight / 2f, 0f);
            }
        }
        else if (GameObject.Find("Remain_info/Split_method") &&
                 GameObject.Find("Remain_info/Split_method").GetComponent<Text>().text == "UD")
        {
            if (player == 1)
            {
                mainCamera.rect = new Rect(0f, 0.5f, 1f, 0.5f);
                mid_point = new Vector3(mainCamera.pixelWidth / 2f, mainCamera.pixelHeight * 3 / 2f, 0f);
            }
            else if (player == 2)
            {
                mainCamera.rect = new Rect(0f, 0f, 1f, 0.5f);
                mid_point = new Vector3(mainCamera.pixelWidth / 2f, mainCamera.pixelHeight / 2f, 0f);
            }
        }
        else
        {
            mainCamera.rect = new Rect(0f, 0f, 1f, 1f);
            mid_point = new Vector3(mainCamera.pixelWidth / 2f, mainCamera.pixelHeight / 2f, 0f);
        }

    }
}
