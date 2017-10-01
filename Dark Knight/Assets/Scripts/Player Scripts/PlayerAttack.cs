using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerAttack : MonoBehaviour {

    public Image fillWaitImage_1;
    public Image fillWaitImage_2;
    public Image fillWaitImage_3;
    public Image fillWaitImage_4;
    public Image fillWaitImage_5;
    public Image fillWaitImage_6;

    private int[] fadeImages = new int[] { 0, 0, 0, 0, 0, 0 }; //six 0's are six elements. when value of 0 changes to 1. the fade image will show

    private Animator anim;
    private bool canAttack = true;
    private PlayerMove playerMove;

    

    // Use this for initialization
    void Start () {
        anim = GetComponent<Animator>();
        playerMove = GetComponent<PlayerMove>();
	}
	public bool CanAttack {
        get { return canAttack; }
        set { canAttack = value; }
    }
    // Update is called once per frame
    void Update () {
		if(!anim.IsInTransition(0) && anim.GetCurrentAnimatorStateInfo(0).IsName("Stand")) {
            canAttack = true; //if not in transition and standing. you can attack
        }
        else {
            canAttack = false;
        }

        CheckToFade();
        CheckInput();

	}

    void CheckInput() {
        if(anim.GetInteger("Atk") == 0) { //if 0, not attacking. so player is moving
            playerMove.FinishedMovement = false;

            if(!anim.IsInTransition(0) && anim.GetCurrentAnimatorStateInfo(0).IsName("Stand")) { 
                //isintransition(0) 0 is base layer. if animation is not in transition and animator state is stand
                playerMove.FinishedMovement = true; //animation finished movement
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha1)) {
            playerMove.TargetPosition = transform.position; //set target position to current position so it doesn't move

            if(playerMove.FinishedMovement && fadeImages[0] != 1 && canAttack) { //fadeimages[0] is index 0 image. first image
                fadeImages[0] = 1;
                anim.SetInteger("Atk", 1);
            }
        }else if (Input.GetKeyDown(KeyCode.Alpha2)) {
            playerMove.TargetPosition = transform.position; 

            if(playerMove.FinishedMovement && fadeImages[1] != 1 && canAttack) { //fadeimages[1] is index 1 image. second image
                fadeImages[1] = 1;
                anim.SetInteger("Atk", 2);
            }
        }else if (Input.GetKeyDown(KeyCode.Alpha3)) {
            playerMove.TargetPosition = transform.position; 

            if(playerMove.FinishedMovement && fadeImages[2] != 1 && canAttack) { 
                fadeImages[2] = 1;
                anim.SetInteger("Atk", 3);
            }
        }else if (Input.GetKeyDown(KeyCode.Alpha4)) {
            playerMove.TargetPosition = transform.position; 

            if(playerMove.FinishedMovement && fadeImages[3] != 1 && canAttack) { 
                fadeImages[3] = 1;
                anim.SetInteger("Atk", 4);
            }
        }else if (Input.GetKeyDown(KeyCode.Alpha5)) {
            playerMove.TargetPosition = transform.position; 

            if(playerMove.FinishedMovement && fadeImages[4] != 1 && canAttack) {
                fadeImages[4] = 1;
                anim.SetInteger("Atk", 5);
            }
        }else if (Input.GetMouseButtonDown(1)) {
            playerMove.TargetPosition = transform.position;

            if (playerMove.FinishedMovement && fadeImages[5] != 1 && canAttack) {
                fadeImages[5] = 1;
                anim.SetInteger("Atk", 6);
            }
        }
        else {
            anim.SetInteger("Atk", 0);
        }

        if (Input.GetKey(KeyCode.Space)) { //getkey is when you hold the button
            Vector3 targetPos = Vector3.zero;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if(Physics.Raycast(ray, out hit)) {
                targetPos = new Vector3(hit.point.x, transform.position.y, hit.point.z);
            }
            //holding space, the player can rotate
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(targetPos - transform.position), 15f * Time.deltaTime);
        }
    }
    
    void CheckToFade() {
        if(fadeImages[0] == 1) {
            if(FadeAndWait(fillWaitImage_1, 1.0f)) {//once fade has finished going away, reset fadeimages array to 0
                fadeImages[0] = 0;
            }
        }
        if (fadeImages[1] == 1) {
            if (FadeAndWait(fillWaitImage_2, .7f)) {
                fadeImages[1] = 0;
            }
        }
        if (fadeImages[2] == 1) {
            if (FadeAndWait(fillWaitImage_3, .1f)) {
                fadeImages[2] = 0;
            }
        }
        if (fadeImages[3] == 1) {
            if (FadeAndWait(fillWaitImage_4, .2f)) {
                fadeImages[3] = 0;
            }
        }
        if (fadeImages[4] == 1) {
            if (FadeAndWait(fillWaitImage_5, .3f)) {
                fadeImages[4] = 0;
            }
        }
        if (fadeImages[5] == 1) {
            if (FadeAndWait(fillWaitImage_6, .08f)) {
                fadeImages[5] = 0;
            }
        }

    }
    bool FadeAndWait(Image fadeImg, float fadeTime) {
        bool faded = false;
        if(fadeImg == null) {
            return faded;
        }
        if (!fadeImg.gameObject.activeInHierarchy) {
            fadeImg.gameObject.SetActive(true);
            fadeImg.fillAmount = 1f; //black fill is full black
        }
        fadeImg.fillAmount -= fadeTime * Time.deltaTime; //subtracting black fill until it's zero

        if(fadeImg.fillAmount <= 0f) { //if fill amout is finished, deactivate fade image
            fadeImg.gameObject.SetActive(false);
            faded = true;
        }
        return faded;
        //faded returns true here which makes the if statements true in CheckToFade() 
        //which activates the fade resetting to 0
    }
}
