using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSController : MonoBehaviour {
	private Transform firstPerson_View;
	private Transform firstPerson_Camera;
	private Vector3 firstPerson_View_Rotation = Vector3.zero;
	private Animator anim;
	private AudioSource audioManager;
	public AudioClip loadDeagleClip, loadAkClip, loadM4Clip;
	private bool canShoot = true;


	public float walkSpeed = 6.75f;
	public float runSpeed = 10f;
	public float crouchSpeed = 4f;
	public float jumpSpeed = 8f;
	public float jumpMoveSpeed = 4f;
	public float gravity = 20f;

	private float speed;
	private bool is_Moving, is_Grounded, is_Crouching, is_CrouchingBack;

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

	[SerializeField] private WeaponManager weapon_Manager;
	private FPSWeapon current_Weapon;

	private float fireRate = 15f;
	private float nextTimeToFire = 0f;

	[SerializeField] private WeaponManager handsWeapon_Manager;
	private FPSHandsWeapon current_Hands_Weapon;

	// Use this for initialization
	void Start () {
		audioManager = GetComponent<AudioSource> ();
		anim = GetComponent<Animator> ();
		firstPerson_View = transform.Find ("FPS View").transform; // transform.find goes through gameobject children to find name
		charController = GetComponent<CharacterController> ();
		speed = walkSpeed;
		is_Moving = false;

		rayDistance = charController.height * .5f + charController.radius; //detects if we are on the ground when crouching
		default_ControllerHeight = charController.height;
		default_CamPos = firstPerson_View.localPosition;

		playerAnimations = GetComponent<FPSPlayerAnimations> ();

		weapon_Manager.weapons [0].SetActive (true);
		current_Weapon = weapon_Manager.weapons [0].GetComponent<FPSWeapon> ();

		handsWeapon_Manager.weapons [0].SetActive (true); //activates deagle
		current_Hands_Weapon = handsWeapon_Manager.weapons [0].GetComponent<FPSHandsWeapon> (); //gets reference of script on current weapon
	}
	
	// Update is called once per frame
	void Update () {
		PlayerMovement ();
		SelectWeapon ();
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
//				if(Input.GetKeyDown(KeyCode.S) && Input.GetKeyDown(KeyCode.LeftControl)){
//					anim.SetBool ("CrouchBack", true);
//				}
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
//		playerAnimations.PlayerCrouchBack (is_CrouchingBack);
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
//			if(is_Crouching){
//				if(CanGetUp()){
//					is_Crouching = false;
//
//					playerAnimations.PlayerCrouch (is_Crouching);
//
//					StopCoroutine (MoveCameraCrouch ());
//					StartCoroutine (MoveCameraCrouch ());
//				}
//			}else{
				moveDirection.y = jumpSpeed;
//			}

		}


	}

	void HandleAnimations(){
		playerAnimations.Movement (charController.velocity.magnitude);
		playerAnimations.PlayerJump (charController.velocity.y);

		if(is_Crouching && charController.velocity.magnitude > 0f){ //if we are crouching and moving
			playerAnimations.PlayerCrouchWalk (charController.velocity.magnitude);

		}
		//SHOOTING
		if(Input.GetMouseButtonDown(0) && Time.time > nextTimeToFire && canShoot){
			nextTimeToFire = Time.time + 1f / fireRate;

			if(is_Crouching){
				playerAnimations.Shoot (false);
			}else{
				playerAnimations.Shoot (true);
			}
			current_Weapon.Shoot ();
			current_Hands_Weapon.Shoot ();
		}

		if(Input.GetKeyDown(KeyCode.R)){
			playerAnimations.ReloadGun ();
			current_Hands_Weapon.Reload ();
		}


	}



	void SelectWeapon(){
		if (Input.GetKeyDown (KeyCode.Alpha1)) {
			canShoot = false;
			if (!handsWeapon_Manager.weapons [0].activeInHierarchy) { //if current active weapon is not weapon 1
				for (int i = 0; i < handsWeapon_Manager.weapons.Length; i++) {
					handsWeapon_Manager.weapons [i].SetActive (false); // turn off all other weapons
				}

				current_Hands_Weapon = null;
				handsWeapon_Manager.weapons [0].SetActive (true); //set weapon 1 to active
				current_Hands_Weapon = handsWeapon_Manager.weapons [0].GetComponent<FPSHandsWeapon> (); // get script from weapon 1
			}

			if (!weapon_Manager.weapons [0].activeInHierarchy) {
				for (int i = 0; i < weapon_Manager.weapons.Length; i++) {
					weapon_Manager.weapons [i].SetActive (false);
				}
				current_Weapon = null;
				weapon_Manager.weapons [0].SetActive (true);
				current_Weapon = weapon_Manager.weapons [0].GetComponent<FPSWeapon> ();

				playerAnimations.ChangeController (true);
				StartCoroutine (loadDeagle());

			}
		}

		if (Input.GetKeyDown (KeyCode.Alpha2)) {
			canShoot = false;
			if (!handsWeapon_Manager.weapons [1].activeInHierarchy) { //if current active weapon is not weapon 1
				for (int i = 0; i < handsWeapon_Manager.weapons.Length; i++) {
					handsWeapon_Manager.weapons [i].SetActive (false); // turn off all other weapons
				}

				current_Hands_Weapon = null;
				handsWeapon_Manager.weapons [1].SetActive (true); //set weapon 1 to active
				current_Hands_Weapon = handsWeapon_Manager.weapons [1].GetComponent<FPSHandsWeapon> (); // get script from weapon 1
			}

			if (!weapon_Manager.weapons [1].activeInHierarchy) {
				for (int i = 0; i < weapon_Manager.weapons.Length; i++) {
					weapon_Manager.weapons [i].SetActive (false);
				}
				current_Weapon = null;
				weapon_Manager.weapons [1].SetActive (true);
				current_Weapon = weapon_Manager.weapons [1].GetComponent<FPSWeapon> ();
			
				playerAnimations.ChangeController (false); //set to false because it's not a pistol
				StartCoroutine (loadAk47());
			}
		}

		if (Input.GetKeyDown (KeyCode.Alpha3)) {
			canShoot = false;
			if (!handsWeapon_Manager.weapons [2].activeInHierarchy) { //if current active weapon is not weapon 1
				for (int i = 0; i < handsWeapon_Manager.weapons.Length; i++) {
					handsWeapon_Manager.weapons [i].SetActive (false); // turn off all other weapons
				}

				current_Hands_Weapon = null;
				handsWeapon_Manager.weapons [2].SetActive (true); //set weapon 1 to active
				current_Hands_Weapon = handsWeapon_Manager.weapons [2].GetComponent<FPSHandsWeapon> (); // get script from weapon 1

			}

			if (!weapon_Manager.weapons [2].activeInHierarchy) {
				for (int i = 0; i < weapon_Manager.weapons.Length; i++) {
					weapon_Manager.weapons [i].SetActive (false);
				}
				current_Weapon = null;
				weapon_Manager.weapons [2].SetActive (true);
				current_Weapon = weapon_Manager.weapons [2].GetComponent<FPSWeapon> ();

				playerAnimations.ChangeController (false);
				StartCoroutine (loadM4a1 ());
			}


		}

	}//select weapon

	IEnumerator loadM4a1(){
		yield return new WaitForSeconds (.9f);

		if(audioManager.clip != loadM4Clip){
			audioManager.clip = loadM4Clip;
		}
		audioManager.Play ();
		yield return new WaitForSeconds (.7f);
		canShoot = true;
	}
	IEnumerator loadAk47(){
		yield return new WaitForSeconds (.5f);

		if(audioManager.clip != loadAkClip){
			audioManager.clip = loadAkClip;
		}
		audioManager.Play ();
		yield return new WaitForSeconds (.5f);
		canShoot = true;
	}

	IEnumerator loadDeagle(){
		yield return new WaitForSeconds (.2f);

		if(audioManager.clip != loadDeagleClip){
			audioManager.clip = loadDeagleClip;
		}
		audioManager.Play ();
		yield return new WaitForSeconds (.7f);
		canShoot = true;
	}
}//class