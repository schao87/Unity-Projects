using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSHandsWeapon : MonoBehaviour {

	public AudioClip shootClip, reloadClipOut, reloadClipIn, loadClip;
	private AudioSource audioManager;
	private GameObject muzzleFlash;

	private Animator anim;

	private string SHOOT = "Shoot";
	private string RELOAD = "Reload";

	// Use this for initialization
	void Awake () {
		muzzleFlash = transform.Find ("Muzzle Flash").gameObject;
		muzzleFlash.SetActive (false);

		audioManager = GetComponent<AudioSource> ();
		anim = GetComponent<Animator> ();
	}
	
	public void Shoot(){
		if(audioManager.clip != shootClip){
			audioManager.clip = shootClip;
		}

		audioManager.Play ();

		StartCoroutine (TurnMuzzleFlashOn());

		anim.SetTrigger (SHOOT); //plays shoot animation
	}
	IEnumerator TurnMuzzleFlashOn(){
		muzzleFlash.SetActive(true);
		yield return new WaitForSeconds (.05f);
		muzzleFlash.SetActive(false);

	}

	public void Reload(){
		StartCoroutine (PlayReloadSound ());
		anim.SetTrigger (RELOAD);
	}

	IEnumerator PlayReloadSound(){
		yield return new WaitForSeconds (.9f);
		if(audioManager.clip != reloadClipOut){
			audioManager.clip = reloadClipOut;
		}
		audioManager.Play ();

		yield return new WaitForSeconds (1.4f);
		if(audioManager.clip != reloadClipIn){
			audioManager.clip = reloadClipIn;
		}
		audioManager.Play ();

		yield return new WaitForSeconds (1.4f);
		if(audioManager.clip != loadClip){
			audioManager.clip = loadClip;
		}
		audioManager.Play ();

	}
}
