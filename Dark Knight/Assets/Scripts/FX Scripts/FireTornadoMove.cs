using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireTornadoMove : MonoBehaviour {

    public LayerMask enemyLayer; // this layer prevents hits from counting as a hit with other colliders that aren't enemies
    public float radius = .5f;
    public float damageCount = 10f;
    public GameObject fireExplosion;

    private EnemyHealth enemyHealth;
    private bool collided;

    private float speed = 3f;
    // Use this for initialization
    void Start () {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        transform.rotation = Quaternion.LookRotation(player.transform.forward);
    }
	
	// Update is called once per frame
	void Update () {
        Move();
        CheckForDamage();
	}
    
    void Move() {
        transform.Translate(Vector3.forward * (speed * Time.deltaTime));
    }
    void CheckForDamage() {
        Collider[] hits = Physics.OverlapSphere(transform.position, radius, enemyLayer);
        //transform.position is position of spell animation. radius is the size of the sphere. enemy layer mask is layer
        foreach (Collider c in hits) {
            //if (c.isTrigger) {
            //    continue;
            //}
            enemyHealth = c.gameObject.GetComponent<EnemyHealth>();
            collided = true;
        }

        if (collided) { //when attack fx collides with enemy layer is true
            enemyHealth.TakeDamage(damageCount);
            Vector3 temp = transform.position;
            temp.y += 2f;
            Instantiate(fireExplosion, temp, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}
