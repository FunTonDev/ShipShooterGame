using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour {
    public static GameManager instance;

    public GameObject currentPlayer;
    public bool playerAlive {
        get {
            return currentPlayer != null;
        }
    }
    public List<GameObject> currentHazards = new List<GameObject>();
    private int currentScore;
    private int highScore;
    private int lives;
    private int roundNumber;
    private int enemyShipTotal;
    private int asteroidTotal;
    private GameObject playerPrefab;
    private GameObject enemyShipPrefab;
    private GameObject debrisPrefab;
    private GameObject playerbuffPrefab;
    private GameObject explosionPrefab;
    private AudioSource gameAudiosource;
    private AudioClip roundClip;
    private AudioClip playerDestroyClip;
    private AudioClip hazardDestroyClip;
    private TextMeshProUGUI highScoreText;
    private TextMeshProUGUI scoreText;
    private TextMeshProUGUI lifeText;
    private TextMeshProUGUI roundText;
    private TextMeshProUGUI respawnText;

    public void Start() {
        instance = this;
        currentScore = 0;
        highScore = PlayerPrefs.GetInt("highScore", 0);
        lives = 3;
        roundNumber = 0;
        enemyShipTotal = 0;
        asteroidTotal = 0;

        playerPrefab = Resources.Load<GameObject>("Prefabs/Prefab_Player");
        enemyShipPrefab = Resources.Load<GameObject>("Prefabs/Prefab_Enemy");
        debrisPrefab = Resources.Load<GameObject>("Prefabs/Prefab_Debris");
        playerbuffPrefab = Resources.Load<GameObject>("Prefabs/Prefab_Buff");
        explosionPrefab = Resources.Load<GameObject>("Prefabs/Prefab_Explosion");
        gameAudiosource = gameObject.AddComponent<AudioSource>();
        roundClip = Resources.Load<AudioClip>("Audio/announceRound");
        playerDestroyClip = Resources.Load<AudioClip>("Audio/destroyPlayer");
        hazardDestroyClip = Resources.Load<AudioClip>("Audio/destroyHazard");
        Transform canvas = GameObject.Find("Canvas").transform;
        scoreText = canvas.Find("text_score").GetComponent<TextMeshProUGUI>();
        highScoreText = canvas.Find("text_highScore").GetComponent<TextMeshProUGUI>();
        lifeText = canvas.Find("text_lives").GetComponent<TextMeshProUGUI>();
        roundText = canvas.Find("text_rounds").GetComponent<TextMeshProUGUI>();
        respawnText = canvas.Find("text_respawn").GetComponent<TextMeshProUGUI>();

        updateStatsUI();
        StartCoroutine(respawnPlayer());
        StartCoroutine(checkRoundStatus());
    }

    public void Update() {
        if (Input.GetKeyDown(KeyCode.Return) && !playerAlive) {
            StartCoroutine(respawnPlayer());
        }
    }

    private IEnumerator spawnHazards() {
        //Asteroids
        for (int i = 0; i < asteroidTotal; i++) {
            yield return new WaitUntil(() => playerAlive);
            GameObject newAsteroid = Instantiate<GameObject>(debrisPrefab, newNonOverlapPosition(3.0f), Quaternion.Euler(0, 0, Random.Range(0, 360)));
            currentHazards.Add(newAsteroid);
        }

        //EnemyShips
        for (int i = 0; i < enemyShipTotal; i++) {
            Vector3 enemyshipPosition = newNonOverlapPosition(4.5f);
            Vector3 playerToEnemyship = currentPlayer.transform.position - enemyshipPosition;
            GameObject newEnemyShip = Instantiate<GameObject>(enemyShipPrefab, enemyshipPosition, Quaternion.Euler(playerToEnemyship.x, playerToEnemyship.y, playerToEnemyship.z));
            currentHazards.Add(newEnemyShip);
        }
    }

    private IEnumerator respawnPlayer() {
        respawnText.color = new Color(255f, 255f, 255f, 0f);
        currentPlayer = Instantiate<GameObject>(playerPrefab, Vector3.zero, Quaternion.Euler(0, 0, 0));
        StartCoroutine(Common.lerpAlpha(currentPlayer.transform.Find("Sprite").GetComponent<SpriteRenderer>(), 3f));
        yield return null;
    }

    private IEnumerator checkRoundStatus() {
        yield return new WaitUntil(() => playerAlive);
        if (currentHazards.Count == 0) {
            roundNumber++;
            enemyShipTotal = roundNumber / 2;
            asteroidTotal = roundNumber;
            roundText.text = "Round " + roundNumber;
            roundText.color = new Color(255f, 255f, 255f, 1f);
            StartCoroutine(Common.lerpAlpha(roundText, 5.0f, false));
            gameAudiosource.PlayOneShot(roundClip);
            StartCoroutine(spawnHazards());
        }
    }

    private void updateStatsUI() {
        scoreText.text = "Score: " + currentScore;
        highScoreText.text = "High Score: " + highScore;
        lifeText.text = "Lives: " + lives;
    }

    public void discardHazard(GameObject hazard) {
        currentHazards.Remove(hazard);
    }
    
    private Vector3 newNonOverlapPosition(float playerRadius) {
        Vector3 position;
        do {
            position = new Vector3(Random.Range(-8.2f, 8.2f), Random.Range(-4.7f, 4.7f), 0);
        } while ((currentPlayer.transform.position - position).magnitude < playerRadius);
        return position;
    }    

    public void playerHazardCollision(Collision2D collision, bool playerDestroyed) {
        lives -= playerDestroyed ? 1 : 0;
        if (lives == 0) {
            SceneManager.LoadScene("Game");
        }

        discardHazard(collision.gameObject);
        lifeText.text = "Lives: " + lives;
        respawnText.color = new Color(255f, 255f, 255f, 1f);

        if (playerDestroyed) {
            gameAudiosource.PlayOneShot(playerDestroyClip);
            placeNewExplosion(collision.otherRigidbody.gameObject.transform.position);
            currentPlayer = null;
        }
        placeNewExplosion(collision.gameObject.transform.position);
        StartCoroutine(checkRoundStatus());
    }

    public void friendlyProjectileCollision(Collision2D collision) {
        string collisionTag = collision.gameObject.tag;
        if (collisionTag != "Hazard") {
            Debug.Log(string.Format("?: FriendlyProjectile collided with a {0} GameObject... ignored handling", collisionTag));
            return;
        }

        GameObject collisionObject = collision.gameObject;
        discardHazard(collisionObject);
        currentScore += (collisionObject.name == "Prefab_Projectile(Clone)" ? 1 : 0);
        currentScore += (collisionObject.name == "Prefab_Debris(Clone)" ? 5 : 0);
        if (collisionObject.name == "Prefab_Enemy(Clone)") {
            currentScore += 10;
            if (Random.value < 0.5f) {
                Instantiate<GameObject>(playerbuffPrefab, collisionObject.transform.position, Quaternion.Euler(0, 0, 0));
            }
        }

        if (currentScore > highScore) {
            highScore = currentScore;
            PlayerPrefs.SetInt("highScore", highScore);
        }

        updateStatsUI();
        placeNewExplosion(collision.gameObject.transform.position);
        StartCoroutine(checkRoundStatus());
    }

    //Psuedo-collision to compensate for missing collider on prefab
    public void shockwaveCollision(Vector3 shockwavePosition) {
        foreach (GameObject hazard in currentHazards) {
            bool hazardWithinRadius = Vector2.Distance(shockwavePosition, hazard.transform.position) < 4.5f;
            if (hazard.name == "Prefab_Debris(Clone)" && hazardWithinRadius) {
                discardHazard(hazard);
                Destroy(hazard);
            }
        }
        StartCoroutine(checkRoundStatus());
    }

    private void placeNewExplosion(Vector3 explosionPosition) {
        GameObject explosionObject = Instantiate<GameObject>(explosionPrefab, explosionPosition, Quaternion.Euler(0, 0, 0));
        gameAudiosource.PlayOneShot(hazardDestroyClip);
        Destroy(explosionObject, 0.6f);
    }
}
