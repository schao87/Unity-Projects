using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI; //holds nav mesh

public class EnemyControl2 : MonoBehaviour {

    public Transform[] walkPoints;
    private int walk_Index = 0;
    private Transform playerTarget;
    private Animator anim;
    private NavMeshAgent navAgent;
    private float walk_Distance = 8f;
    private float attack_Distance = 2f;
    private float currentAttackTime;
    private float waitAttackTime = .5f;
    private Vector3 nextDestination;
    private PlayerAttack playerAttack;
    private PlayerHealth playerHealth;
    private EnemyHealth enemyHealth;

    void Awake () {
        playerTarget = GameObject.FindGameObjectWithTag("Player").transform;
        anim = GetComponent<Animator>();
        navAgent = GetComponent<NavMeshAgent>();
        playerHealth = playerTarget.GetComponent<PlayerHealth>();
        enemyHealth = GetComponent<EnemyHealth>();
    }
	
	// Update is called once per frame
	void Update () {
       if(enemyHealth.health > 0) {
            MoveAndAttack();
        }
        else {
            anim.SetBool("Death", true);
            navAgent.enabled = false;
            if (!anim.IsInTransition(0) && anim.GetCurrentAnimatorStateInfo(0).IsName("Death")
                && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= .95f) {
                //normalizedtime 0 is start of anim. 1 is end. .95 is near the end
                Destroy(gameObject, .5f);
            }
        }
	}

    void MoveAndAttack() {
        float distance = Vector3.Distance(transform.position, playerTarget.position);
        if (playerHealth.PlayerDead == false) {
            if (distance > walk_Distance) {
                if (navAgent.remainingDistance <= 0.5f) {
                    navAgent.isStopped = false;
                    anim.SetBool("Walk", true);
                    anim.SetBool("Run", false);
                    anim.SetInteger("Atk", 0);

                    nextDestination = walkPoints[walk_Index].position;
                    navAgent.SetDestination(nextDestination);

                    if (walk_Index == walkPoints.Length - 1) {
                        walk_Index = 0; //when walkpoints counts up to index 2, reset back to 0
                    }
                    else {
                        walk_Index++;
                    }
                }
            }
            else {
                if (distance > attack_Distance) {
                    navAgent.isStopped = false;
                    anim.SetBool("Walk", false);
                    anim.SetBool("Run", true);
                    anim.SetInteger("Atk", 0);

                    navAgent.SetDestination(playerTarget.position);
                }
                else {
                    navAgent.isStopped = true;
                    anim.SetBool("Run", false);

                    Vector3 targetPosition = new Vector3(playerTarget.position.x, transform.position.y, playerTarget.position.z);

                    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(targetPosition - transform.position), 5f * Time.deltaTime);
                    //turn enemy around by rotating him spherically to face the player 

                    if (currentAttackTime >= waitAttackTime) {
                        int atkRange = Random.Range(1, 3);
                        anim.SetInteger("Atk", atkRange);
                        currentAttackTime = 0f;
                    }
                    else {
                        anim.SetInteger("Atk", 0);
                        currentAttackTime += Time.deltaTime;
                    }

                }
            }
        }
    }
}
