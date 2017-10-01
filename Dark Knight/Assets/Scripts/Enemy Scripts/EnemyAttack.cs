using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour {

    public float damageAmount = 10f;

    private Transform playerTarget;
    private Animator anim;
    private bool finishedAttack = true;
    private float damageDistance = 2f;
    private PlayerHealth playerHealth;

	// Use this for initialization
	void Awake () {
        playerTarget = GameObject.FindGameObjectWithTag("Player").transform; //transform returns the position of the object
        anim = GetComponent<Animator>();
        playerHealth = playerTarget.GetComponent<PlayerHealth>();
	}
	
	// Update is called once per frame
	void Update () {
        if (finishedAttack) {
            if(playerTarget != null) {//checks if player gameobject exists because it gets destroyed when he dies
                DealDamage(CheckIfAttacking()); ////DealDamage will only run if CheckIfAttacking returns true
            }
            
        }
        else {
            if(!anim.IsInTransition(0) && anim.GetCurrentAnimatorStateInfo(0).IsName("Idle")) {
                finishedAttack = true;
                //if animation is not in transition and current state is idle, finished attack is true
            }

        }
	}

    bool CheckIfAttacking() { //this function returns a bool
        bool isAttacking = false;

        if (!anim.IsInTransition(0) && anim.GetCurrentAnimatorStateInfo(0).IsName("Atk1") ||
            anim.GetCurrentAnimatorStateInfo(0).IsName("Atk2")) {

            if(anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= .5f) { //in the middle of animation. .5f
                isAttacking = true;
                finishedAttack = false;
            }
        }

        return isAttacking;
    }

    void DealDamage(bool isAttacking) {
        if (isAttacking) {
            if(Vector3.Distance(transform.position, playerTarget.position) <= damageDistance) {
                playerHealth.TakeDamage(damageAmount); //goes inside player health script and runs takeDamage function to check if player is shielded
            }
        }
    }
}
