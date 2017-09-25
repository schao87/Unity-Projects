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
	public float jumpMoveSpeed = 4f;
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

	public LayerMask groundLayer;
	private float rayDistance;
	private float default_ControllerHeight;
	private Vector3 default_CamPos;
	private float camHeight;

	private FPSPlayerAnimations playerAnimations;


	// Use this for initialization
	void Start () {
		
		firstPerson_View = transform.Find ("FPS View").transform; // transform.find goes through gameobject children to find name
		charController = GetComponent<CharacterController> ();
		speed = walkSpeed;
		is_Moving = false;

		rayDistance = charController.height * .5f + charController.radius; //detects if we are on the ground when crouching
		default_ControllerHeight = charController.height;
		default_CamPos = firstPerson_View.localPosition;

		playerAnimations = GetComponent<FPSPlayerAnimations> ();
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
			PlayerCrouchingAndSprinting ();

			//THIS IS WHERE IT IS GETTING THE X AND Y MOVEMENT
			//THIS IS WHERE IT IS GETTING THE X AND Y MOVEMENT
			moveDirection = new Vector3 (inputX * inputModifyFactor, -antiBumpFactor, inputY * inputModifyFactor); //anitbumpfactor will smooth movement when bumping into wall.
			//THIS IS WHERE IT IS GETTING THE X AND Y MOVEMENT
			//THIS IS WHERE IT IS GETTING THE X AND Y MOVEMENT


			moveDirection = transform.TransformDirection (moveDirection) * speed; // converting direction from local space to world space

			PlayerJump ();
		}else{
//			if (charController.velocity.z == 0 && charController.velocity.x == 0) {
				//controls air jumping
				moveDirection.x = inputX * speed * inputModifyFactor;
				moveDirection.z = inputY * speed * inputModifyFactor;
				moveDirection = transform.TransformDirection(moveDirection);
//			}	

		}


		moveDirection.y -= gravity * Time.deltaTime;
		is_Grounded = (charController.Move (moveDirection * Time.deltaTime) & CollisionFlags.Below) != 0; //Move returns a collision flag. collision flags is a bit number. if collision flag is not 0, it means we are ground and returns true

		is_Moving = charController.velocity.magnitude > .15f;

		HandleAnimations ();
	}

	void PlayerCrouchingAndSprinting(){
		if(Input.GetKeyDown(KeyCode.LeftControl)){
			if(!is_Crouching){
				is_Crouching = true;
			}else{
				if(CanGetUp()){
					is_Crouching = false;
				}
			}
			StopCoroutine (MoveCameraCrouch ());
			StartCoroutine (MoveCameraCrouch ());
		}

		if(Input.GetKeyUp(KeyCode.LeftControl)){
			if(is_Crouching){
				is_Crouching = false;
			}
			StopCoroutine (MoveCameraCrouch ());
			StartCoroutine (MoveCameraCrouch ());
		}

		if(is_Crouching){
			speed = crouchSpeed;
		}else{
			if(Input.GetKey(KeyCode.LeftShift)){
				speed = crouchSpeed;
			}else{
				speed = walkSpeed;
			}
		}
		playerAnimations.PlayerCrouch (is_Crouching);
	}

	bool CanGetUp(){
		Ray groundRay = new Ray (transform.position, transform.up);
		RaycastHit groundHit;

		if(Physics.SphereCast(groundRay, charController.radius + .05f, out groundHit, rayDistance, groundLayer)){ //a sphere that is half the size of player. detects position of sphere to determine if crouching or not. if groundray hits ground layer
			if(Vector3.Distance(transform.position, groundHit.point) < 2.3f){ //if current position and the point of contact of ray case is less than 2.3f
				return false;
			}
		}
		return true;
	}

	IEnumerator MoveCameraCrouch(){
		charController.height = is_Crouching ? default_ControllerHeight / 1.5f : default_ControllerHeight;
		charController.center = new Vector3 (0f, charController.height / 2f, 0f);

		camHeight = is_Crouching ? default_CamPos.y / 1.5f : default_CamPos.y; //moves camera down when crouching or return to original height

		while(Mathf.Abs(camHeight - firstPerson_View.localPosition.y) > .01f){
			firstPerson_View.localPosition = Vector3.Lerp (firstPerson_View.localPosition, new Vector3 (default_CamPos.x, camHeight, default_CamPos.z), Time.deltaTime * 11f);
			yield return null;
		}
	}

	void PlayerJump(){
		if(Input.GetKeyDown(KeyCode.Space)){
			if(is_Crouching){
				if(CanGetUp()){
					is_Crouching = false;

					playerAnimations.PlayerCrouch (is_Crouching);

					StopCoroutine (MoveCameraCrouch ());
					StartCoroutine (MoveCameraCrouch ());
				}
			}else{
				moveDirection.y = jumpSpeed;
			}

		}


	}

	void HandleAnimations(){
		playerAnimations.Movement (charController.velocity.magnitude);
		playerAnimations.PlayerJump (charController.velocity.y);

		if(is_Crouching && charController.velocity.magnitude > 0f){ //if we are crouching and moving
			playerAnimations.PlayerCrouchWalk (charController.velocity.magnitude);
		}
	}
}	
