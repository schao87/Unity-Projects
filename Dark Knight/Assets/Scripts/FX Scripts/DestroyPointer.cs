using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyPointer : MonoBehaviour {

    private Transform player;
	// Use this for initialization
	void Start () {
        player = GameObject.FindGameObjectWithTag("Player").transform;
	}
	
	// Update is called once per frame
	void Update () {
		if(Vector3.Distance(transform.position, player.position) <= 1f) { // if distance between pointer and player is less than 1.1
            Destroy(gameObject);
        }
	}
}
