using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour {

    public float health = 100f;
    private bool isShielded;
    private Animator anim;
    private bool playerDead = false;
	// Use this for initialization
	void Awake () {
        anim = GetComponent<Animator>();
	}
	public bool PlayerDead {
        get { return playerDead; }
        set { playerDead = value; }
    }


    public bool Shielded {
        get { return isShielded; }
        set { isShielded = value; }
    }

    public void TakeDamage(float amount) {
        if (!isShielded) {
            health -= amount;
            //anim.SetTrigger("PlayerHit");
            
            print("Player took damage. health is " + health);
            if(health <= 0f) {
                anim.SetBool("Death", true);
                playerDead = true;
               
                if (!anim.IsInTransition(0) && anim.GetCurrentAnimatorStateInfo(0).IsName("Death")
                    && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= .95f) { //test is it is at end of animation
                    Destroy(gameObject, .2f);
                    print("player dead yo");
                }
            }
        }
    }
}
