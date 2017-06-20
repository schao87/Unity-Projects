using UnityEngine;
using System.Collections;
using System.Collections.Generic;


	
public class movingObject : MonoBehaviour {
    [SerializeField] private float objectSpeed = 1;
    [SerializeField] private float resetPosition = -5.21f; //position when rock reaches left screen
    [SerializeField] private float startPosition = 13.4f; //position that rock respawns on right screen
    //make it private so other classes can't manipulate it
    // Use this for initialization


    void Start()
    {

    }

    // Update is called once per frame
    protected virtual void Update() //this allows other objects to inherit this
    {
        if (!GameManager.instance.GameOver && GameManager.instance.PlayerActive)
        {
            transform.Translate(Vector3.left * (objectSpeed * Time.deltaTime), Space.World);

            if (transform.localPosition.x <= resetPosition)
            {
                Vector3 newPos = new Vector3(startPosition, transform.position.y, transform.position.z); // keep current y and z position
                transform.position = newPos;
            }
        }
    }
}