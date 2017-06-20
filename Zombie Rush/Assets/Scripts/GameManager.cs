using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class GameManager : MonoBehaviour {
    public static GameManager instance = null;

    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject replayMenu;
    

    private bool playerActive = false; //private variables that other scripts can't change
    private bool gameOver = false;
    private bool gameStarted = false;
    

    public bool PlayerActive
    {
        get { return playerActive; } //public accessors. others can get data, but can't manipulate
    }
    public bool GameOver
    {
        get { return gameOver; }
    }
    public bool GameStarted
    {
        get { return gameStarted; }
    }
    public void Restart()
    {
        Awake(); 
    }
    private void Awake(){
        if (instance == null)
        {
            instance = this;
        }else if (instance != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
        Assert.IsNotNull(mainMenu);
        replayMenu.SetActive(false);
    }
    // Use this for initialization
    void Start () {
       
    }
	
	// Update is called once per frame
	void Update () {
		
	}
    public void PlayerCollided()
    {
        gameOver = true;
        Invoke("ReplayMenu", 1);
        
    }
    public void ReplayMenu()
    {
        replayMenu.SetActive(true);
    }
    public void PlayerStartedGame()
    {
        playerActive = true;
    }
    public void EnterGame()
    {
        mainMenu.SetActive(false);
        replayMenu.SetActive(false);
        gameStarted = true;
    }
   
}
