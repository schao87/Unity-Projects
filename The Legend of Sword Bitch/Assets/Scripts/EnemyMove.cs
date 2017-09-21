using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Assertions;

public class EnemyMove : MonoBehaviour {

    private Transform player;

    private NavMeshAgent nav;
    private Animator anim;
    private EnemyHealth enemyHealth;

    // Use this for initialization
    void Start () {
        player = GameManager.instance.Player.transform;
        anim = GetComponent<Animator>();
        nav = GetComponent<NavMeshAgent>();
        enemyHealth = GetComponent<EnemyHealth>();
	}
	
	// Update is called once per frame
	void Update () {
        if (!GameManager.instance.GameOver && enemyHealth.IsAlive) {
            nav.SetDestination(player.position); //makes enemy follow hero
        }
        else if((!GameManager.instance.GameOver || GameManager.instance.GameOver) && !enemyHealth.IsAlive){
            nav.enabled = false;

        }
        else {
            nav.enabled = false;
            anim.Play("Idle");
        }
        
    }
}
