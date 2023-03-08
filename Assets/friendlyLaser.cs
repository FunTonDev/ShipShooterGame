using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class friendlyLaser : MonoBehaviour
{
    playerShip playerScript;
    gameMain mainScript;
    public float maxTime = 1.0f;
    private GameObject explosionObject;
    private ParticleSystem explosionParticles;

    // Start is called before the first frame update
    void Start()
    {
        playerScript = GameObject.FindWithTag("Player").GetComponent<playerShip>();
        mainScript = GameObject.Find("Main Camera").GetComponent<gameMain>();
        explosionObject = GameObject.Find("ExplosionParticles");
        explosionParticles = explosionObject.GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        Destroy(gameObject, maxTime);
    }

    //COLLISION CHECKS
    private void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject collidedWith = collision.gameObject;

        Vector3 pos = new Vector3(transform.position.x, transform.position.y, -10);
        explosionObject.transform.position = pos;
        explosionParticles.Play();

        mainScript.hazardExplosionSource.Play();
        //COLLIDING WITH ENEMY LASER
        if (collidedWith.layer == 11)
        {
            mainScript.currentScore++;
            Destroy(gameObject);
            Destroy(collidedWith);
        }
        //COLLIDING WITH ASTEROID
        else if (collidedWith.layer == 12)
        {
            mainScript.currentScore += 5;
            mainScript.asteroidsDestroyed++;
            Destroy(gameObject);
            Destroy(collidedWith);
        }
        //COLLIDING WITH ENEMY SHIP
        else if (collidedWith.layer == 9)
        {
            mainScript.currentScore += 10;
            mainScript.enemyShipsDestroyed++;
            Destroy(gameObject);
            Destroy(collidedWith);
        }

        //UPDATES SCORE ACCORDINGLY
        mainScript.currentScoreText.text = "Current Score: " + mainScript.currentScore;
        if (mainScript.currentScore > mainScript.highScore)
        {
            mainScript.highScore = mainScript.currentScore;
            mainScript.highScoreText.text = "Best Score: " + mainScript.highScore;
            PlayerPrefs.SetInt("highScore", mainScript.highScore);
        }
    }
}