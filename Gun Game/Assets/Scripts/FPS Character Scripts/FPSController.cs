using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSController : MonoBehaviour {
	private Transform firstPerson_View;
	private Transform firstPerson_Camera;
	private Vector3 firstPerson_View_Rotation = Vector3.zero;

	public float walkSpeed = 6.75f;
	public float runSpeed = 10f;
	public float crouchSpeed = 4f;
	public float jumpSpeed = 8f;
	public float gravity = 20f;

	private float speed;
	private bool is_Moving, is_Grounded, is_Crouching;

	private float inputX, inputY;
	private float inputX_Set, inputY_Set;
	private float inputModifyFactor;

	private bool limitDiagonalSpeed = true;
	private float antiBumpFactor = .75f;
	private CharacterController charController;
	private Vector3 moveDirection = Vector3.zero;

	// Use this for initialization
	void Start () {
		firstPerson_View = transform.Find ("FPS View").transform; // transform.find goes through gameobject children to find name
		charController = GetComponent<CharacterController> ();
		speed = walkSpeed;
		is_Moving = false;

	}
	
	// Update is called once per frame
	void Update () {
		PlayerMovement ();
	}

	void PlayerMovement(){
		if (Input.GetKey (KeyCode.W) || Input.GetKey (KeyCode.S)) {
			if(Input.GetKey(KeyCode.W)){
				inputY_Set = 1f;
			}else{
				inputY_Set = -1f;
			}
		}else{
			inputY_Set = 0f;
		}

		if(Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D)){
			if(Input.GetKey(KeyCode.A)){
				inputX_Set = -1f;
			}else{
				inputX_Set = 1f;
			}
		}else{
			inputX_Set = 0f;
		}

		inputY = Mathf.Lerp (inputY, inputY_Set, Time.deltaTime * 19f);
		inputX = Mathf.Lerp (inputX, inputX_Set, Time.deltaTime * 19f);

		inputModifyFactor = Mathf.Lerp(inputModifyFactor, (inputY_Set != 0 && inputX_Set != 0 && limitDiagonalSpeed) ? .75f : 1.0f, Time.deltaTime * 19f); //limits the speed of diagonal movement

		firstPerson_View_Rotation = Vector3.Lerp (firstPerson_View_Rotation, Vector3.zero, Time.deltaTime * 5f);
		firstPerson_View.localEulerAngles = firstPerson_View_Rotation; //fps cam rotation angles are relative to parent object fps player. we need to use local euler angles to affect angles relative to parent object and not relative to the world

		if(is_Grounded){
			moveDirection = new Vector3 (inputX * inputModifyFactor, -antiBumpFactor, inputY * inputModifyFactor); //anitbumpfactor will smooth movement when bumping into wall
			moveDirection = transform.TransformDirection (moveDirection) * speed; // converting direction from local space to world space
		}
		moveDirection.y -= gravity * Time.deltaTime;
		is_Grounded = (charController.Move (moveDirection * Time.deltaTime) & CollisionFlags.Below) != 0; //Move returns a collision flag. collision flags is a bit number. if collision flag is not 0, it means we are ground and returns true

		is_Moving = charController.velocity.magnitude > .15f;
	}
}	
