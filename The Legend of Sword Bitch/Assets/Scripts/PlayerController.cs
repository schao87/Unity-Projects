using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private LayerMask layerMask;

    private CharacterController characterController;
    private Vector3 currentLookTarget = Vector3.zero;
    private Animator anim;
    private BoxCollider[] swordColliders;
    private GameObject fireTrail;
    private ParticleSystem fireTrailParticles;
	// Use this for initialization
	void Start () {
        fireTrail = GameObject.FindWithTag("Fire") as GameObject;
        fireTrail.SetActive(false);
        characterController = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
        swordColliders = GetComponentsInChildren<BoxCollider>();
	}

    // Update is called once per frame
    void Update() {
        if (!GameManager.instance.GameOver) {
            Vector3 moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            //move direction from x and z axis. y is left 0.
            characterController.SimpleMove(moveDirection * moveSpeed);
            //takes input from moveDirection and multiplies it to move speed

            if (moveDirection == Vector3.zero) {
                anim.SetBool("IsWalking", false);
            }
            else {
                anim.SetBool("IsWalking", true);
            }

            if (Input.GetMouseButtonDown(0)) {
                anim.Play("DoubleChop");
            }

            if (Input.GetMouseButtonDown(1)) {
                anim.Play("SpinAttack");
            }
        }
	}

    private void FixedUpdate() {
        //fixed update is used for physics objects
        if (!GameManager.instance.GameOver){
            RaycastHit hit; //point at which the raycast hits the layermask
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            Debug.DrawRay(ray.origin, ray.direction * 500, Color.blue);

            if(Physics.Raycast(ray, out hit, 500, layerMask, QueryTriggerInteraction.Ignore)) {
                //origin, direction, distance, layermask that ray will hit, specifies if this should hit triggers
                if(hit.point != currentLookTarget) {
                    currentLookTarget = hit.point;
                }//if ray's point is not equal to current target, set current target to ray's point

                Vector3 targetPosition = new Vector3(hit.point.x, transform.position.y, hit.point.z);
                Quaternion rotation = Quaternion.LookRotation(targetPosition - transform.position);
                transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * 10f);
            }
        } 
    }

    public void BeginAttack() {
        foreach (var weapon in swordColliders) {
            weapon.enabled = true;
        }
    }

    public void EndAttack() {
        foreach (var weapon in swordColliders) {
            weapon.enabled = false;
        }
    }

    public void SpeedPowerUp() {
        StartCoroutine(fireTrailRoutine());
    }

    IEnumerator fireTrailRoutine() {
        fireTrail.SetActive(true);
        moveSpeed = 10f;
        yield return new WaitForSeconds(10f);

        moveSpeed = 6f;
        fireTrailParticles = fireTrail.GetComponent<ParticleSystem>();
        var em = fireTrailParticles.emission;
        em.enabled = false; //turns off emission of fire so it stops following hero

        yield return new WaitForSeconds(3f);
        em.enabled = true; //turns on emission to get ready for next power up
        fireTrail.SetActive(false); //sets to false so it doesn't show up before using power up
    }
}
