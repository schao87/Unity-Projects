using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSMouse : MonoBehaviour {

	public enum RotationAxes { MouseX, MouseY}
	public RotationAxes axes = RotationAxes.MouseY; //by default it will rotate around mouse Y

	private float currentSensitivity_X = 1.5f;
	private float currentSensitivity_Y = 1.5f;

	private float sensitivity_X = 1.5f;
	private float sensitivity_Y = 1.5f;

	private float rotation_X, rotation_Y;

	private float minimum_X = -360f;
	private float maximum_X = 360f;

	private float minimum_Y = -60f;
	private float maximum_Y = 60f;

	private Quaternion originalRotation;

	private float mouseSensitivity = 1.7f;

	// Use this for initialization
	void Start () {
		originalRotation = transform.rotation;
	}

	void LateUpdate(){ //late update is called after updated and fixed update
		HandleRotation();
	}
	float ClampAngle(float angle, float min, float max){
		if(angle < -360f){
			angle -= 360f;
		}

		if(angle > 360f){
			angle -= 360f;
		}
		return Mathf.Clamp(angle, min, max);
	}
	
	void HandleRotation(){
		if(currentSensitivity_X != mouseSensitivity || currentSensitivity_Y != mouseSensitivity){
			currentSensitivity_X = currentSensitivity_Y = mouseSensitivity; //this is a precaution to test sensitivity
		}

		sensitivity_X = currentSensitivity_X;
		sensitivity_Y = currentSensitivity_Y;

		if(axes == RotationAxes.MouseX){
			rotation_X += Input.GetAxis ("Mouse X") * sensitivity_X; //mouse x is horizontal movement of mouse

			rotation_X = ClampAngle (rotation_X, minimum_X, maximum_X);
			Quaternion xQuaternion = Quaternion.AngleAxis (rotation_X, Vector3.up); // uses mouse x degrees and rotates it around the world y axis
			transform.localRotation = originalRotation * xQuaternion;
		}

		if(axes == RotationAxes.MouseY){
			rotation_Y += Input.GetAxis ("Mouse Y") * sensitivity_Y; //mouse y is verticle movement of mouse

			rotation_Y = ClampAngle (rotation_Y, minimum_Y, maximum_Y);
			Quaternion yQuaternion = Quaternion.AngleAxis (-rotation_Y, Vector3.right);// uses mouse y degrees and rotates it around the world x axis
			transform.localRotation = originalRotation * yQuaternion;
		}
	}
}
