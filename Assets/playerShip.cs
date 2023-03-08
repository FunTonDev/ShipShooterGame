using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class playerShip : MonoBehaviour
{
    public Rigidbody2D shipBody;
    public GameObject friendlyLaserPrefab;
    public GameObject shockwavePrefab;
    private SpriteRenderer shieldRenderer;
    public ParticleSystem rocketParticles;
    private GameObject explosionObject;
    private ParticleSystem explosionParticles;

    public AudioSource shootSource;
    public AudioSource thrusterSource;
    public AudioSource machinegunSource;
    public AudioSource shieldSource;
    public AudioSource shockwaveSource;
    public AudioSource emptyPowerSource;

    gameMain mainScript;

    private string powerUp = "";
    public bool shieldActive = false;
    private float laserEnergy = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        shieldRenderer = GameObject.FindWithTag("Shield").GetComponent<SpriteRenderer>();
        mainScript = GameObject.Find("Main Camera").GetComponent<gameMain>();
        explosionObject = GameObject.Find("ExplosionParticles");
        explosionParticles = explosionObject.GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        //CHECKS FOR MOVEMENT
        if (Input.GetKey("up") || Input.GetKey("w"))
        {
            shipBody.AddForce(transform.up * 6);
            thrusterSource.volume = 1;
            rocketParticles.Play();
        }
        if(Input.GetKeyUp("up") || Input.GetKeyUp("w"))
        {
            thrusterSource.volume = 0;
        }
        if (Input.GetKey("down") || Input.GetKey("s"))
        {
            shipBody.AddForce(-transform.up * 6);
            thrusterSource.volume = 1;
        }
        if (Input.GetKeyUp("down") || Input.GetKeyUp("s"))
        {
            thrusterSource.volume = 0;
        }
        if (Input.GetKey("left") || Input.GetKey("a"))
        {
            shipBody.AddTorque(-Input.GetAxis("Horizontal") * 6);
        }
        if (Input.GetKey("right") || Input.GetKey("d"))
        {
            shipBody.AddTorque(-Input.GetAxis("Horizontal") * 6);
        }
        
        //CHECKS FOR BORDER CONTACT
        if (Mathf.Abs(transform.position.x) > 8.2f)
        {
            Vector3 newPos = transform.position;
            newPos.x *= -1;
            transform.position = newPos;
        }
        if (Mathf.Abs(transform.position.y) > 4.7f)
        {
            Vector3 newPos = transform.position;
            newPos.y *= -1;
            transform.position = newPos;
        }

        //CHECK FOR SHOOTING
        if (Input.GetKey("space"))
        {
            laserEnergy += Time.deltaTime;
        }
        if (Input.GetKeyUp("space"))
        {
            GameObject[] activeLasers = GameObject.FindGameObjectsWithTag("friendlyLaser");
            if (activeLasers.Length < 2)
            {
                shootSource.Play();
                useLaser();
            }
        }

        //CHECK FOR POWERUP
        if (Input.GetKeyDown("left shift"))
        {
            usePowerUp();
        }
    }
    
    //COLLISION CHECKS
    private void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject collidedWith = collision.gameObject;

        //COLLIDING WITH ENEMYSHIP, ENEMYLASER, OR ASTEROID
        if ((collidedWith.layer == 9 || collidedWith.layer == 11 || collidedWith.layer == 12) && !mainScript.waitingForRespawn)
        {
            if (collidedWith.layer == 12)
            {
                mainScript.asteroidsDestroyed++;
            }
            else if (collidedWith.layer == 9)
            {
                mainScript.enemyShipsDestroyed++;
            }

            if (shieldActive)
            {
                shieldActive = false;
                mainScript.hazardExplosionSource.Play();
                shieldRenderer.color = new Color(255f, 255f, 255f, 0f);
            }
            else
            {
                mainScript.playerExplosionSource.Play();
                mainScript.respawnText.color = new Color(255f, 255f, 255f, 1f);
                mainScript.lives--;
                mainScript.livesText.text = "Lives: " + mainScript.lives;
                Destroy(gameObject);
                mainScript.waitingForRespawn = true;

                Vector3 pos = new Vector3(transform.position.x, transform.position.y, -10);
                explosionObject.transform.position = pos;
                explosionParticles.Play();
            }
            Destroy(collidedWith);
        }
        //COLLIDING WITH POWERUP CONTAINER
        else if (collidedWith.layer == 13)
        {
            if (collidedWith.tag == "Shockwave Power")
            {
                powerUp = "SHOCKWAVE";
                Debug.Log("Current Powerup: Shockwave");
            }
            else if (collidedWith.tag == "Machinegun Power")
            {
                powerUp = "MACHINEGUN";
                Debug.Log("Current Powerup: Machinegun");
            }
            else if (collidedWith.tag == "Shield Power")
            {
                powerUp = "SHIELD";
                Debug.Log("Current Powerup: Shield");
            }
            Destroy(collidedWith);
        }

        if (mainScript.lives == 0)
        {
            SceneManager.LoadScene("SampleScene");
        }
    }

    private void useLaser()
    {
        //SHOOT REGULAR LASER
        if (laserEnergy <= 0.75)
        {
            GameObject newLaser = Instantiate<GameObject>(friendlyLaserPrefab);
            Vector3 pos = Vector3.zero;
            pos.x = transform.position.x;
            pos.y = transform.position.y;
            newLaser.transform.position = pos;
            newLaser.transform.rotation = transform.rotation;

            friendlyLaser script = newLaser.GetComponent<friendlyLaser>();
            script.maxTime = 1.0f;

            Rigidbody2D laserRigidbody = newLaser.GetComponent<Rigidbody2D>();
            laserRigidbody.AddForce(transform.up * 400);
            Debug.Log("Regular Laser!");
        }
        
        //CHARGED SHOT TO BE IMPLEMENTED LATER
        else
        {
            GameObject newLaser = Instantiate<GameObject>(friendlyLaserPrefab);
            newLaser.transform.localScale = new Vector3(3, 7, 1);
            
            Vector3 pos = Vector3.zero;
            pos.x = transform.position.x;
            pos.y = transform.position.y;
            newLaser.transform.position = pos;
            newLaser.transform.rotation = transform.rotation;

            friendlyLaser script = newLaser.GetComponent<friendlyLaser>();
            script.maxTime = 1.0f;

            Rigidbody2D laserRigidbody = newLaser.GetComponent<Rigidbody2D>();
            laserRigidbody.AddForce(transform.up * 700);
            Debug.Log("Charged Laser!");
        }
        laserEnergy = 0.0f;
    }

    private void usePowerUp()
    {
        if (powerUp == "SHOCKWAVE")
        {
            shockwaveSource.Play();
            powerUp = "";
            GameObject newShockwave = Instantiate<GameObject>(shockwavePrefab);
            Vector3 pos = Vector3.zero;
            pos.x = transform.position.x;
            pos.y = transform.position.y;
            newShockwave.transform.position = pos;
            destroyWithinPlayerRadius();
            Destroy(newShockwave, 3.1f);
            StartCoroutine(fadeOut(newShockwave));
        }
        else if (powerUp == "MACHINEGUN")
        {
            machinegunSource.Play();
            powerUp = "";
            StartCoroutine(rapidFire());
        }
        else if (powerUp == "SHIELD")
        {
            shieldSource.Play();
            powerUp = "";
            shieldActive = true;
            shieldRenderer.color = new Color(255f, 255f, 255f, 1f);
        }
        else
        {
            emptyPowerSource.Play();
        }
        Debug.Log("Current Powerup: ");
    }

    private void destroyWithinPlayerRadius()
    {
        GameObject[] hazards = GameObject.FindGameObjectsWithTag("Hazard");
        GameObject[] enemyLasers = GameObject.FindGameObjectsWithTag("enemyLaser");
        Vector2 shipPosition = transform.position;
        Vector2 otherPosition;
        foreach (GameObject hazard in hazards)
        {
            otherPosition = hazard.transform.position;
            if (Vector2.Distance(shipPosition, otherPosition) < 4.5f)
            {
                Destroy(hazard);
                mainScript.enemyShipsDestroyed++;
            }
        }
    }

    private IEnumerator rapidFire()
    {
        for (int i = 0; i < 20; i++)
        {
            GameObject newLaser = Instantiate<GameObject>(friendlyLaserPrefab);
            Vector3 pos = Vector3.zero;
            pos.x = transform.position.x;
            pos.y = transform.position.y;
            newLaser.transform.position = pos;
            newLaser.transform.rotation = transform.rotation;

            friendlyLaser script = newLaser.GetComponent<friendlyLaser>();
            script.maxTime = 1.0f;

            Rigidbody2D laserRigidbody = newLaser.GetComponent<Rigidbody2D>();
            laserRigidbody.AddForce(transform.up * 400);
            yield return new WaitForSeconds(0.05f);
        }
    }

    private IEnumerator fadeOut(GameObject thing)
    {
        SpriteRenderer renderer = thing.GetComponent<SpriteRenderer>();
        renderer.color = new Color(255f, 255f, 255f, 1f);
        while (renderer.color.a > 0.0f)
        {
            renderer.color = new Color(255.0f, 255.0f, 255.0f, renderer.color.a - (Time.deltaTime / 3.0f));
            yield return null;
        }
    }
}

