using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class Player : MonoBehaviour {
    [SerializeField] private float jumpForce = 100f;
    [SerializeField] private AudioClip sfxJump;
    [SerializeField] private AudioClip sfxDeath;
    [SerializeField] private AudioClip sfxGameOver;

    private Animator anim;
    private Rigidbody rigidBody;
    private bool jump = false;
    private AudioSource audioSource;

    private void Awake()//always goes before start()
    {
        //Assert.IsNotNull(sfxGameOver);
        Assert.IsNotNull(sfxJump); //assertions check for empty serialize fields
        Assert.IsNotNull(sfxDeath);
        
    }

    // Use this for initialization
    void Start () {
        anim = GetComponent<Animator>();
        rigidBody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
      
    }
	
	// Update is called once per frame
	void Update () {
        Vector3 currentY = transform.position; 
        currentY.y = Mathf.Clamp(currentY.y, 0f, 14f); //limits his max and min y position movement
        transform.position = currentY;
        
        if (!GameManager.instance.GameOver && GameManager.instance.GameStarted)
        {
            if (Input.GetMouseButtonDown(0))
            {
                GameManager.instance.PlayerStartedGame();
                anim.Play("jump");
                audioSource.PlayOneShot(sfxJump);
                rigidBody.useGravity = true;
                jump = true;
               
            }
        } 
	}

    //anything with physics should use FixedUpdate.
    //different computers process things at different speeds
    private void FixedUpdate()
    {
        if (jump == true)
        {
            jump = false;
            rigidBody.velocity = new Vector2(0, 0);
            
            rigidBody.AddForce(new Vector2(0, jumpForce), ForceMode.Impulse);
        }

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "obstacle")
        {
            rigidBody.AddForce(new Vector2(-50, 20), ForceMode.Impulse);
            rigidBody.detectCollisions = false;
            audioSource.PlayOneShot(sfxDeath);
            GameManager.instance.PlayerCollided();
            Invoke("gameOverBitch", 2);


        }
       
    }
    private void gameOverBitch()
    {
        audioSource.PlayOneShot(sfxGameOver);
    }
}
