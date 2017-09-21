using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPowerUp : MonoBehaviour {

    private GameObject player;
    private PlayerHealth playerHealth;
    // Use this for initialization
    void Start() {
        player = GameManager.instance.Player;
        playerHealth = player.GetComponent<PlayerHealth>();
        GameManager.instance.RegisterPowerUp(); //every time a power spawns, it registers and powerups counts up 1
    }

    private void OnTriggerEnter(Collider other) {
        if(other.gameObject == player) {
            playerHealth.PowerUpHealth();
            Destroy(gameObject);
        }
    }
}
