using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heal : MonoBehaviour {

    public float healAmount = 20f;
	// Use this for initialization
	void Start () {
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealth>().health += healAmount;
        print("Players health is " + GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealth>().health);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
