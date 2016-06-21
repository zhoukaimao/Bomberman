using UnityEngine;
using System.Collections;

public class BombermanMove : MonoBehaviour {
    private float walkSpeed;
    private CharacterController _controller;
    private Animator _animator;
    private Bomberman _bomberman;
    private float _rotateY;
    public float rotateY
    {
        get
        {
            return _rotateY;
        }
        set
        {
            _rotateY = value;
        }
    }
	// Use this for initialization
	void Start () {
        _animator = GetComponent<Animator>();
        _controller = GetComponent<CharacterController>();
        _bomberman = GetComponent<Bomberman>();
	}
	
	// Update is called once per frame
	void Update () {
        walkSpeed = _bomberman.walkSpeed;
        _animator.SetFloat("Speed", _controller.velocity.magnitude);
        _animator.SetFloat("HP",_bomberman.HP);
	}
    /// <summary>
    /// return if arrived
    /// </summary>
    /// <param name="dest"></param>
    /// <returns></returns>
    public bool MoveTo(Vector2 dest)
    {
        Vector3 moveDirection = Util.Map2World(dest) - transform.position;
        moveDirection.y = 0;
        if (moveDirection.magnitude < 0.01f) return true;
        if (!_controller.isGrounded)
        {
            moveDirection.y = -1 * Time.deltaTime;
        }
        _controller.Move(moveDirection.normalized*walkSpeed*Time.deltaTime);
        float rotateY = Quaternion.FromToRotation(transform.forward, moveDirection).eulerAngles.y;
        //Debug.Log("RotateY:"+rotateY);
        if (rotateY >= 45 && rotateY <= 135) TurnRight();
        //else if (rotateY <= 225) TurnBack();
        else if (rotateY > 225&&rotateY<=315) TurnLeft();
        return false;
    }
    public void TurnLeft()
    {
        _controller.transform.forward = -transform.right;
    }
    public void TurnRight()
    {
        _controller.transform.forward = transform.right;
    }
    public void TurnBack()
    {
        _controller.transform.forward = -transform.forward;
    }
    public void Move(Vector3 moveDirection)
    {
        if (!_controller.isGrounded)
        {
            moveDirection.y = -1*Time.deltaTime;
        }
        //moveDirection *= walkSpeed;
        _controller.Move(moveDirection.normalized*walkSpeed * Time.deltaTime);
        moveDirection.y = 0;
        _controller.transform.forward = moveDirection;
    }
}
