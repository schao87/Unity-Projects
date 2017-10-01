using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour {

    public float health = 100f;
    public void TakeDamage(float amount) {
        health -= amount;
        print("enemy took damage, health is " + health);
        
        if (health <= 0) {

        }
    }
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
