using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyHealth : MonoBehaviour {

    [SerializeField] private int startingHealth = 20;
    [SerializeField] private float timeSinceLastHit = .5f;
    [SerializeField] private float disappearSpeed = 2f;

    private new AudioSource audio;
    private float timer = 0f;
    private Animator anim;
    private NavMeshAgent nav;
    private bool isAlive;
    private Rigidbody rigidBody;
    private CapsuleCollider capsuleCollider;
    private bool disappearEnemy = false;
    private int currentHealth;
    private EnemyAttack enemyAttack;
    private ParticleSystem blood;

    public bool IsAlive {
        get { return isAlive; }
    }

	// Use this for initialization
	void Start () {
        GameManager.instance.RegisterEnemy(this); //every enemy spawned to be registered to the RegisterEnemy List
        rigidBody = GetComponent<Rigidbody>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        nav = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        audio = GetComponent<AudioSource>();
        isAlive = true;
        currentHealth = startingHealth;
        enemyAttack = GetComponent<EnemyAttack>();
        blood = GetComponentInChildren<ParticleSystem>();
    }
	
	// Update is called once per frame
	void Update () {
        timer += Time.deltaTime;

        if (disappearEnemy) {
            transform.Translate(-Vector3.up * disappearSpeed * Time.deltaTime);
        }
	}

    private void OnTriggerEnter(Collider other) {
        if ( timer > timeSinceLastHit && !GameManager.instance.GameOver) {
            
            if (other.tag == "PlayerWeapon") {
                takeHit();
                blood.Play();
                timer = 0f;
                //enemyAttack.EnemyEndAttack();
            }
        }
    }

    void takeHit() {
        if(currentHealth > 0) {
            audio.PlayOneShot(audio.clip);
            anim.Play("Hurt");
            currentHealth -= 10;
        }

        if(currentHealth <= 0) {
            isAlive = false;
            KillEnemy();
        }
    }

    void KillEnemy() {
        GameManager.instance.KilledEnemy(this); // each time enemy is killed, it is added to killedenemy list
        capsuleCollider.enabled = false;
        nav.enabled = false;
        anim.SetTrigger("EnemyDie");
        rigidBody.isKinematic = true;
        StartCoroutine(removeEnemy());
    }

    IEnumerator removeEnemy() {
        //wait 4 seconds after enemy dies
        yield return new WaitForSeconds(4f);
        //start to sink enemy
        disappearEnemy = true;
        //after 2 seconds destroy game object
        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
    }
}
