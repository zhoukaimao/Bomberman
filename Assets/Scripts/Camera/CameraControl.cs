using UnityEngine;
using System.Collections;

public class CameraControl : MonoBehaviour {
    public float rotate_Speed = 50.0f;//旋转速度
    private Transform camTrans;
	// Use this for initialization
	void Start () {
        camTrans = Camera.main.transform;
	}
	
	// Update is called once per frame
	void Update () {
        float mouseX = Input.GetAxis("Horizontal");
        camTrans.RotateAround(new Vector3(0, 0, 0), Vector3.up, rotate_Speed * mouseX * Time.deltaTime);
        float mouseY = Input.GetAxis("Vertical");
        camTrans.Rotate(Vector3.right, mouseY * rotate_Speed * Time.deltaTime);
	}
}
