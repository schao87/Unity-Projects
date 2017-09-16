using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    public static GameManager instance = null;

    [SerializeField] GameObject player;
    [SerializeField] GameObject[] spawnPoints;
    [SerializeField] GameObject[] powerUpSpawns;
    [SerializeField] GameObject tanker;
    [SerializeField] GameObject ranger;
    [SerializeField] GameObject soldier;
    [SerializeField] GameObject arrow;
    [SerializeField] GameObject healthPowerUp;
    [SerializeField] GameObject speedPowerUp;
    [SerializeField] Text levelText;
    [SerializeField] Text endGameText;
    [SerializeField] int finalLevel = 10;
    [SerializeField] int maxPowerUps = 4;

    private bool gameOver = false;
    private int currentLevel;
    private float generatedSpawnTime = 1;
    private float currentSpawnTime = 0;
    private float powerUpSpawnTime = 60;
    private float currentPowerUpSpawnTime = 0;
    private GameObject newEnemy;
    private int powerups = 0;
    private GameObject newPowerup;


    private List<EnemyHealth> enemies = new List<EnemyHealth>();
    private List<EnemyHealth> killedEnemies = new List<EnemyHealth>();

    public void RegisterEnemy(EnemyHealth enemy) {
        enemies.Add(enemy);
    }

    public void KilledEnemy(EnemyHealth enemy) {
        killedEnemies.Add(enemy);
    }

    public void RegisterPowerUp() {
        powerups++;
    }

    public bool GameOver {
        get { return gameOver; }
    }

    public GameObject Player {
        get { return player; }
    }

    public GameObject Arrow {
        get { return arrow; }
    }

    private void Awake() {
        if(instance == null) {
            instance = this;
        }else if (instance != this) {
            Destroy(gameObject);
        }
        // DontDestroyOnLoad(gameObject); 
        //this is not needed here because it is only needed when we want game manager to persist in every scene.
        //the main menu does not need this.
        //with it turned off, each time game is started, a fresh gamemanager is created
    }
    // Use this for initialization
    void Start () {
        currentLevel = 1;
        StartCoroutine(spawn());
        StartCoroutine(powerUpSpawn());
        endGameText.GetComponent<Text>().enabled = false;
    }
	
	// Update is called once per frame
	void Update () {
        currentSpawnTime += Time.deltaTime;
        currentPowerUpSpawnTime += Time.deltaTime;
    }

    public void PlayerHit(int currentHP) {
        if (currentHP > 0) {
            gameOver = false;
        }else {
            gameOver = true;
            StartCoroutine(endGame("you dead, bitch"));
        }
    }

    IEnumerator spawn() {
        if(currentSpawnTime > generatedSpawnTime) {
            currentSpawnTime = 0; //reset count so counter can restart
            if(enemies.Count < currentLevel) {
                int randomNumber = Random.Range(0, spawnPoints.Length - 1);
                GameObject spawnLocation = spawnPoints[randomNumber]; //makes random spawnpoint into spawnlocation
                int randomEnemy = Random.Range(0, 3);//0 is inclusive. the max is exclusive
                if(randomEnemy == 0) {
                    newEnemy = Instantiate(soldier) as GameObject;
                }else if (randomEnemy == 1) {
                    newEnemy = Instantiate(ranger) as GameObject;
                }else if (randomEnemy == 2) {
                    newEnemy = Instantiate(tanker) as GameObject;
                }
                newEnemy.transform.position = spawnLocation.transform.position; //sets new enemy to location as spawnpoint
            }
        }

        if (killedEnemies.Count == currentLevel && currentLevel != finalLevel) {//keeps track of enemies killed so it knows when to start next level
            enemies.Clear();                        //when list gets cleared, spawn() runs again
            killedEnemies.Clear();
            yield return new WaitForSeconds(3f);
            currentLevel += 1;
            levelText.text = "Level " + currentLevel;
        }

        if(killedEnemies.Count == finalLevel) {
            StartCoroutine(endGame("You a strong ass bitch"));
        }
        yield return null;
        StartCoroutine(spawn());
    }

    IEnumerator powerUpSpawn() {
        if(currentPowerUpSpawnTime > powerUpSpawnTime) {
            currentPowerUpSpawnTime = 0;
            if(powerups < maxPowerUps) {
                int randomNumber = Random.Range(0, powerUpSpawns.Length - 1);
                GameObject spawnLocation = powerUpSpawns[randomNumber];
                int randomPowerUp = Random.Range(0, 2); //0 is inclusive. the max is exclusive
                if(randomPowerUp == 0) {
                    newPowerup = Instantiate(healthPowerUp) as GameObject;
                }else if(randomPowerUp == 1) {
                    newPowerup = Instantiate(speedPowerUp) as GameObject;
                }
                newPowerup.transform.position = spawnLocation.transform.position;
            }
        }
        yield return null;
        StartCoroutine(powerUpSpawn());
    }

    IEnumerator endGame(string outcome) {
        endGameText.text = outcome;
        endGameText.GetComponent<Text>().enabled = true;
        yield return new WaitForSeconds(3f);
        SceneManager.LoadScene("GameMenu");
    }
}
