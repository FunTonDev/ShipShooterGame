using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyShip : MonoBehaviour
{
    public Rigidbody2D shipBody;
    public GameObject enemyLaserPrefab;
    public GameObject powerUpPrefab;
    public AudioSource shootSource;
    gameMain mainScript;
    playerShip playerScript;

    // Start is called before the first frame update
    void Start()
    {
        Vector3 beginningVec = new Vector3(Random.value, Random.value, 0);
        shipBody.velocity = beginningVec;
        transform.up = beginningVec;

        mainScript = GameObject.Find("Main Camera").GetComponent<gameMain>();
        playerScript = GameObject.FindWithTag("Player").GetComponent<playerShip>();
    }

    // Update is called once per frame
    void Update()
    {
        GameObject playerShip;
        Vector3 newUp;
        if (mainScript.waitingForRespawn == false)
        {
            playerShip = GameObject.FindWithTag("Player");
            if (playerShip != null)
            {
                newUp = playerShip.transform.position - transform.position;

                if (newUp.magnitude < 3.5f)
                {
                    if (Vector2.SignedAngle(transform.up, newUp) < 0)
                    {
                        shipBody.AddTorque(-1.5f);
                    }
                    else
                    {
                        shipBody.AddTorque(1.5f);
                    }
                    shipBody.velocity = transform.up;
                    autoFire();
                }
            }
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
    }

    //COLLISION CHECKS
    private void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject collidedWith = collision.gameObject;

        //COLLIDING WITH PLAYERSHIP OR FRIENDLY LASER
        if ((collidedWith.layer == 8 && !playerScript.shieldActive) || collidedWith.layer == 10)
        {
            Destroy(gameObject);
            Destroy(collidedWith);

            if (Random.value < 0.5f)
            {
                GameObject newLaser = Instantiate<GameObject>(powerUpPrefab);
                Vector3 pos = Vector3.zero;
                pos.x = transform.position.x;
                pos.y = transform.position.y;
                newLaser.transform.position = pos;
            }
        }

    }

    //AUTOMATICALLY FIRES ENEMY LASER WHEN WITHIN RANGE OF PLAYER SHIP
    private void autoFire()
    {
        GameObject[] activeLasers = GameObject.FindGameObjectsWithTag("enemyLaser");
        if (activeLasers.Length == 0)
        {
            shootSource.Play();
            GameObject newLaser = Instantiate<GameObject>(enemyLaserPrefab);
            Vector3 pos = Vector3.zero;
            pos.x = transform.position.x;
            pos.y = transform.position.y;
            newLaser.transform.position = pos;
            newLaser.transform.rotation = transform.rotation;

            Rigidbody2D laserRigidbody = newLaser.GetComponent<Rigidbody2D>();
            laserRigidbody.AddForce(transform.up * 400);
        }
    }
}
