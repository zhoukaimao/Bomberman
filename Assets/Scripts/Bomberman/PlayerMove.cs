using UnityEngine;
using System.Collections;

public class PlayerMove : MonoBehaviour {
    Animator anim;
    Rigidbody rigid;
    Vector3 moveDirection = Vector3.zero;
    CharacterController controller;
    public float gravity = 1.0f;
	// Use this for initialization
	void Start () {
        anim = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody>();

        controller = GetComponent<CharacterController>();
	}
	
	// Update is called once per frame
	void Update () {
        float walkSpeed = GetComponent<Bomberman>().walkSpeed;
        moveDirection = new Vector3(Input.GetAxis("Vertical"), 0, -Input.GetAxis("Horizontal"));
        moveDirection *= walkSpeed;
        if (controller.isGrounded)
        {
        }
        else
        {
            moveDirection.y -= gravity * Time.deltaTime; //这里就相当于增加了重力
        }
        controller.Move(moveDirection * Time.deltaTime);
        anim.SetFloat("Speed", controller.velocity.magnitude);
        //anim.SetFloat("TurnAngle",60.0f);
        float rotateY = Quaternion.FromToRotation(transform.forward, moveDirection).eulerAngles.y;
        if (Mathf.Abs(rotateY) > 0.1f)
        {
            anim.SetBool("Turn",true);
            if (rotateY > 180) rotateY = rotateY - 360;
            anim.SetFloat("TurnAngle",rotateY);
            //controller.transform.forward = Vector3.Lerp(transform.forward, moveDirection, 0.1f);  
            controller.transform.forward = moveDirection;
        }
        else
        {
            anim.SetBool("Turn",false);
        }
        Debug.Log("RotateY:"+rotateY);
        //if (rotateY > 0.1f)
        //{
        //    anim.SetFloat("Turn", rotateY/360);
        //}
        
        //Vector3 angle_temp = transform.eulerAngles;
        //angle_temp.y += Input.GetAxis("Horizontal");
        //transform.eulerAngles = angle_temp;
	}
    public static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360F)
            angle += 360F;
        if (angle > 360F)
            angle -= 360F;
        return Mathf.Clamp(angle, min, max);
    }
}
