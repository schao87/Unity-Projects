using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour {

    private Animator anim;
    private CharacterController charController;
    private CollisionFlags collisionFlags = CollisionFlags.None;
    private float moveSpeed = 5f;
    private bool canMove;
    private bool finished_Movement = true;
    private float player_ToPointDistance;
    private Vector3 target_Pos = Vector3.zero;
    private Vector3 player_Move = Vector3.zero;
    private float gravity = 9.8f;
    private float height;
	
	void Awake () {
        anim = GetComponent<Animator>();
        charController = GetComponent<CharacterController>();
	}
	
	// Update is called once per frame
	void Update () {
        CalculateHeight();
        CheckIfFinishedMovement();

    }

    bool IsGrounded() {
        return collisionFlags == CollisionFlags.CollidedBelow ? true : false; //tests if character is colliding with ground
    }

    void CalculateHeight() {
        if (IsGrounded()) {
            height = 0f;
        }
        else {
            height -= gravity * Time.deltaTime;
        }
    }

    void CheckIfFinishedMovement() {
        if (!finished_Movement) {
            //the 0s are the base layer in the animator screen
            if(!anim.IsInTransition(0) && !anim.GetCurrentAnimatorStateInfo(0).IsName("Stand") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.8f) {
                //normalized time of the animation is represented from 0 to 1. 0 is beginning. 1 is the end
                finished_Movement = true;
                //if player is not standing and animation is about the finish, finish the movement
            }
        }
        else {
            MoveThePlayer();
            player_Move.y = height * Time.deltaTime;
            collisionFlags = charController.Move(player_Move); //this moves the character in the given direction
        }
    }

    void MoveThePlayer() {
        if (Input.GetMouseButtonDown(0)){
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); //input.mouseposition is the direction
            RaycastHit hit; //this is a way to extract info from the raycast
            if (Physics.Raycast(ray, out hit)){
                if(hit.collider is TerrainCollider) {
                    player_ToPointDistance = Vector3.Distance(transform.position, hit.point);// returns distance between a and b

                    if(player_ToPointDistance >= 1.0f) {
                        canMove = true;
                        target_Pos = hit.point;
                    }
                }
            }
            
        }
        if (canMove) {
            anim.SetFloat("Walk", 1.0f); // goes into run animation
            Vector3 target_Temp = new Vector3(target_Pos.x, transform.position.y, target_Pos.z);
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(target_Temp - transform.position), 15.0f * Time.deltaTime);
            player_Move = transform.forward * moveSpeed * Time.deltaTime;

            if (Vector3.Distance(transform.position, target_Pos) <= 0.1f) {
                canMove = false;
            }
            
        }else {
            player_Move.Set(0f, 0f, 0f); //resets player_move variable
            anim.SetFloat("Walk", 0f);
        }
    } 

    public bool FinishedMovement {
        get { return finished_Movement; }
        set { finished_Movement = value; }
    }

    public Vector3 TargetPosition {
        get { return target_Pos; }
        set { target_Pos = value; }
    }
}
