using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseScript : MonoBehaviour {
    public Texture2D cursorTexture;

    public GameObject mousePoint;
    private GameObject instantiatedMouse;
    private Transform playerTarget;
    private CursorMode mode = CursorMode.ForceSoftware;
    private Vector2 hotSpot = Vector2.zero;
    private PlayerAttack playerAttack;
    private PlayerHealth playerHealth;
    // Use this for initialization
    void Start () {
        playerTarget = GameObject.FindGameObjectWithTag("Player").transform;
        playerAttack = GetComponent<PlayerAttack>();
        playerHealth = playerTarget.GetComponent<PlayerHealth>();
    }

    // Update is called once per frame
    void Update () {
        Cursor.SetCursor(cursorTexture, hotSpot, mode);

        if (Input.GetMouseButtonUp(0) && playerHealth.PlayerDead == false) {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit; //gathers info from the ray hitting something

            if(Physics.Raycast( ray, out hit)) { //raycasthit infro goes to out hit
                if(hit.collider is TerrainCollider) {//if the raycast hit has a collider and is terraincollider
                    Vector3 temp = hit.point;
                    temp.y = .25f;
                    

                    if(instantiatedMouse == null) {
                        instantiatedMouse = Instantiate(mousePoint) as GameObject;
                        instantiatedMouse.transform.position = temp;
                    } else {
                        Destroy(instantiatedMouse); //destroys duplicate mousepoints
                        instantiatedMouse = Instantiate(mousePoint) as GameObject;
                        instantiatedMouse.transform.position = temp;
                    }
                }
            }
        }
	}
}
