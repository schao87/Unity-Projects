using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class CameraFollow : MonoBehaviour {

    [SerializeField] Transform target; //hero gameObject will be dragged into here
    [SerializeField] float smoothing = 5f; //creates bungie cord effect on camera follow

    Vector3 offset;

    private void Awake() {
        Assert.IsNotNull(target);
    }
    // Use this for initialization
    void Start () {
        offset = transform.position - target.position; //offset of cam position and hero position
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 targetCamPos = target.position + offset;
        transform.position = Vector3.Lerp(transform.position, targetCamPos, smoothing * Time.deltaTime);
        //lerp(from, to, time it takes)
        //lerp makes the transition between two positions look smooth. without it, the camera would just snap to positions
	}
}
