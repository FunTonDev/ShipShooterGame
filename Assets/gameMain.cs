using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class gameMain : MonoBehaviour
{
    public Text highScoreText;
    public Text currentScoreText;
    public Text currentRoundText;
    public Text respawnText;
    public Text livesText;

    public AudioSource hazardExplosionSource;
    public AudioSource playerExplosionSource;
    public AudioSource newRoundSource;

    public GameObject playerShipPrefab;
    public GameObject enemyShipPrefab;
    public GameObject debris1Prefab;
    public GameObject debris2Prefab;

    public int enemyShipsDestroyed = 0;
    public int asteroidsDestroyed = 0;
    public int currentScore = 0;
    public int highScore = 0;
    public int lives = 3;
    public bool waitingForRespawn = false;

    private int roundNumber = 0;
    private int enemyShipTotal = 0;
    private int asteroidTotal = 0;

    playerShip playerScript;

    // Start is called before the first frame update
    void Start()
    {
        highScore = PlayerPrefs.GetInt("highScore", 0);
        currentScoreText.text = "Current Score: " + currentScore;
        highScoreText.text = "High Score: " + highScore;
        livesText.text = "Lives: " + lives;
        playerScript = GameObject.FindWithTag("Player").GetComponent<playerShip>();
    }

    // Update is called once per frame
    void Update()
    {
        //CHECKS FOR CLEARED SCREEN BEFORE BEGINNING OF A NEW ROUND
        if (enemyShipTotal + asteroidTotal == enemyShipsDestroyed + asteroidsDestroyed)
        {
            roundNumber++;
            enemyShipsDestroyed = 0;
            asteroidsDestroyed = 0;
            enemyShipTotal = roundNumber/2;
            asteroidTotal = roundNumber;
            currentRoundText.text = "Round " + roundNumber;
            StartCoroutine(fadeAway());
            newRoundSource.Play();
            spawnAsteroids(asteroidTotal);
            spawnEnemyShips(enemyShipTotal);
        }

        //CHECK FOR RESPAWN
        if (Input.GetKeyDown(KeyCode.Return) && waitingForRespawn && GameObject.FindWithTag("Player") == null)
        {
            StartCoroutine(respawnPlayer());
        }
    }

    private void spawnAsteroids(int asteroids)
    {
        for (int i = 0; i < asteroids;i++)
        {
            GameObject newAsteroid;
            if (Random.value < 0.5f)
            {
                newAsteroid = Instantiate<GameObject>(debris1Prefab);
            }
            else
            {
                newAsteroid = Instantiate<GameObject>(debris2Prefab);
            }

            Vector3 pos = new Vector3(Random.Range(-8.2f, 8.2f), Random.Range(-4.7f, 4.7f), 0);
            GameObject playerShip = GameObject.FindWithTag("Player");
            Vector3 newUp;
            if (playerShip != null)
            {
                newUp = playerShip.transform.position - pos;
                while (newUp.magnitude < 3)
                {
                    pos = new Vector3(Random.Range(-8.2f, 8.2f), Random.Range(-4.7f, 4.7f), 0);
                    newUp = playerShip.transform.position - pos;
                }
                newAsteroid.transform.up = newUp;
            }
            newAsteroid.transform.position = pos;
            newAsteroid.transform.rotation = Quaternion.Euler(0, 0, Random.Range(0, 360));

            Vector3 startingDirection = new Vector3(Random.Range(-100, 100), Random.Range(-100, 100), 0);
            Rigidbody2D asteroidRigidbody = newAsteroid.GetComponent<Rigidbody2D>();
            asteroidRigidbody.AddForce(startingDirection);
            asteroidRigidbody.AddTorque(Random.Range(-50f, 50f));
            StartCoroutine(fadeIn(newAsteroid, 1.0f));
        }
    }

    private void spawnEnemyShips(int enemyShips)
    {
        for (int i = 0; i < enemyShips; i++)
        {
            GameObject newEnemyShip = Instantiate<GameObject>(enemyShipPrefab);

            Vector3 pos = new Vector3(Random.Range(-8.2f, 8.2f), Random.Range(-4.7f, 4.7f), 0);
            GameObject playerShip = GameObject.FindWithTag("Player");
            Vector3 newUp;
            if (playerShip != null)
            {
                newUp = playerShip.transform.position - pos;
                while (newUp.magnitude < 4.5f)
                {
                    pos = new Vector3(Random.Range(-8.2f, 8.2f), Random.Range(-4.7f, 4.7f), 0);
                    newUp = playerShip.transform.position - pos;
                }
                newEnemyShip.transform.up = newUp;
                newEnemyShip.transform.rotation = Quaternion.Euler(newUp.x, newUp.y, newUp.z);
            }
            newEnemyShip.transform.position = pos;

            
            //Vector3 startingDirection = new Vector3(Random.Range(-100, 100), Random.Range(-100, 100), Random.Range(0, 360));
            StartCoroutine(fadeIn(newEnemyShip, 1.0f));
        }
    }

    private IEnumerator fadeAway()
    {
        GameObject playerCheck = GameObject.FindWithTag("Player");
        currentRoundText.color = new Color(255f, 255f, 255f, 1f);
        while (currentRoundText.color.a > 0.0f && playerCheck != null)
        {
            playerCheck = GameObject.FindWithTag("Player");
            currentRoundText.color = new Color(255.0f, 255.0f, 255.0f, currentRoundText.color.a - (Time.deltaTime / 5.0f));
            yield return null;
        }
    }

    private IEnumerator fadeIn(GameObject hazard, float time)
    {
        SpriteRenderer renderer = hazard.GetComponent<SpriteRenderer>();
        renderer.color = new Color(255f, 255f, 255f, 0f);
        while (renderer.color.a < 1.0f)
        {
            renderer.color = new Color(255.0f, 255.0f, 255.0f, renderer.color.a + (Time.deltaTime / time));
            yield return null;
        }
    }

    private IEnumerator respawnPlayer()
    {
        GameObject newPlayer = Instantiate<GameObject>(playerShipPrefab);
        Collider2D tempColl = newPlayer.GetComponent<Collider2D>();
        tempColl.enabled = false;
        Vector3 pos = Vector3.zero;
        newPlayer.transform.position = pos;
        respawnText.color = new Color(255f, 255f, 255f, 0f);
        StartCoroutine(fadeIn(newPlayer, 4f));
        yield return new WaitForSeconds(5);
        tempColl.enabled = true;
        waitingForRespawn = false;
    }
}
