using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillDamage : MonoBehaviour {

    public LayerMask enemyLayer; // this layer prevents hits from counting as a hit with other colliders that aren't enemies
    public float radius = .5f;
    public float damageCount = 10f;

    private EnemyHealth enemyHealth;
    private bool collided;
	
	
	// Update is called once per frame
	void Update () {
        Collider[] hits = Physics.OverlapSphere(transform.position, radius, enemyLayer);
        //transform.position is position of spell animation. radius is the size of the sphere. enemy layer mask is layer
        foreach(Collider c in hits) {
            //if (c.isTrigger) {
            //    continue;
            //}
            enemyHealth = c.gameObject.GetComponent<EnemyHealth>();
            collided = true;
        }

        if (collided) { //when attack fx collides with enemy layer is true
            enemyHealth.TakeDamage(damageCount);
            enabled = false; //disable it after hit so it doesn't do infinite hits
        }
	}
}
