using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rock : movingObject
{

    [SerializeField] Vector3 topPosition;
    [SerializeField] Vector3 bottomPosition;
    [SerializeField] float speed;
    [SerializeField] float spinSpeed;
    // Use this for initialization
    void Start()
    {
        StartCoroutine(Move(bottomPosition));
    }

    
    protected override void Update() {
        //protected override lets update inherit and override Update from moving object class
        if (GameManager.instance.PlayerActive)
        {
            base.Update(); //this calls original update from movingObject.cs
        }
        
        transform.Rotate(0, Time.deltaTime * spinSpeed, 0);
    }   
    IEnumerator Move(Vector3 target)
    {
        while (Mathf.Abs((target - transform.localPosition).y) > 0.20f)
        //target is where we want to go. localPosition is where we are
        {
            Vector3 direction = target.y == topPosition.y ? Vector3.up : Vector3.down;
            //if target.y (where we want to go) is currently same as top position (already set in unity), go up. 
            //else go down
            transform.localPosition += direction * Time.deltaTime * speed;

            yield return null; //this makes it run over and over again
        }
        yield return new WaitForSeconds(0.5f);

        Vector3 newTarget = target.y == topPosition.y ? bottomPosition : topPosition;
        //if target.y currently = topPosition, it means we need to switch targets

        StartCoroutine(Move(newTarget)); //starts move function again using newTarget
        
    }
}
