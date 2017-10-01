using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {

    public float follow_Height = 8f;
    public float follow_Distance = 6f;

    private Transform player;

    private float target_Height;
    private float current_Height;
    private float current_Rotation;
    private PlayerHealth playerHealth;

    void Awake () {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        playerHealth = player.GetComponent<PlayerHealth>();
    }
	
    
	// Update is called once per frame
	void Update () {
        if (playerHealth.PlayerDead == false) {
            target_Height = player.position.y + follow_Height;
            current_Rotation = transform.eulerAngles.y; //euler.y will access the y rotation in degrees
            current_Height = Mathf.Lerp(transform.position.y, target_Height, 0.9f * Time.deltaTime);
            //lerp makes something move from point A to B in the time set in third parameter
            //this will make the camera zoom out in the beginning of the game


            Quaternion euler = Quaternion.Euler(0f, current_Rotation, 0f); //only interested in y rotation which is why others are 0f

            Vector3 target_Position = player.position - (euler * Vector3.forward) * follow_Distance;

            target_Position.y = current_Height;
            transform.position = target_Position;
            transform.LookAt(player);
        }
	}
}
