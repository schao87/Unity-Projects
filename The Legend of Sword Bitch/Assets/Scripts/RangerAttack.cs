using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.AI;

public class RangerAttack : MonoBehaviour {

    [SerializeField] private float range = 3f;
    [SerializeField] private float timeBetweenAttacks = 1f;
    [SerializeField] Transform fireLocation;//ranger's bow is dragged into here
    private Animator anim;
    private GameObject player;
    private bool playerInRange;
    private GameObject arrow;
    private EnemyHealth enemyHealth;

    //public NavMeshAgent nav;
    // Use this for initialization
    void Start() {
        enemyHealth = GetComponent<EnemyHealth>();
        arrow = GameManager.instance.Arrow;
        player = GameManager.instance.Player;
        anim = GetComponent<Animator>();
  
        StartCoroutine(attack());
    }

    // Update is called once per frame
    void Update() {
        if (Vector3.Distance(transform.position, player.transform.position) < range && enemyHealth.IsAlive) {
            playerInRange = true;
            anim.SetBool("PlayerInRange", true);
            RotateTowards(player.transform);
        }
        else {
            playerInRange = false;
            anim.SetBool("PlayerInRange", false);
        }
    }



    IEnumerator attack() {
        if (playerInRange && !GameManager.instance.GameOver) {
            //StopMove();
            anim.Play("Attack");
            yield return new WaitForSeconds(timeBetweenAttacks);
            //StartMove();
        }
        yield return null;
        StartCoroutine(attack());
    }

    private void RotateTowards(Transform player) {
        Vector3 direction = (player.position - transform.position).normalized; //creates direction towards player
        Quaternion lookRotation = Quaternion.LookRotation(direction); //creates a rotation
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 10f);
        //performing rotation using slerp. passing in enemy's current rotation, and where we want it to look
    }

    public void FireArrow() {//this function gets called in the ranger model's attack animation
        
        GameObject newArrow = Instantiate(arrow) as GameObject;
        newArrow.transform.position = fireLocation.position;
        newArrow.transform.rotation = transform.rotation;
        newArrow.GetComponent<Rigidbody>().velocity = transform.forward * 25f;
    }
}
