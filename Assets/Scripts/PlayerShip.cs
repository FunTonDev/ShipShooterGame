using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShip : MonoBehaviour {
    private const int machinegunProjectileCount = 20;

    private bool shieldActive;
    private float laserEnergy;
    private Buff buff;
    private GameObject shockwavePrefab;
    private Rigidbody2D shipBody;
    private SpriteRenderer shieldSpriterenderer;
    private ParticleSystem thrusterParticlesystem;
    private AudioSource playerAudiosource;
    private AudioSource thrusterAudiosource;

    public void Awake() {
        GetComponent<Collider2D>().enabled = false;
        shieldActive = false;
        laserEnergy = 0.0f;
        buff = new Buff();
        shockwavePrefab = Resources.Load<GameObject>("Prefabs/Prefab_Shockwave");
        shipBody = gameObject.GetComponent<Rigidbody2D>();
        shieldSpriterenderer = transform.Find("Shield").GetComponent<SpriteRenderer>();
        thrusterParticlesystem = transform.Find("Thruster").GetComponent<ParticleSystem>();
        playerAudiosource = gameObject.AddComponent<AudioSource>();
        thrusterAudiosource = gameObject.AddComponent<AudioSource>();
        thrusterAudiosource.clip = Resources.Load<AudioClip>("Audio/thruster");
        thrusterAudiosource.loop = true;
        thrusterAudiosource.Play();
        GetComponent<Collider2D>().enabled = true;
    }

    public void Update() {
        moveCheck();
        shootCheck();
        buffCheck();
    }

    //Collision cases: EnemyShip, EnemyLaser, Asteroid, Buff/Powerup
    public void OnCollisionEnter2D(Collision2D collision) {
        string collisionTag = collision.gameObject.tag;

        if (collisionTag != "Hazard" && collisionTag != "Untagged") {
            Debug.Log(string.Format("?: Player collided with a {0} GameObject... ignored handling", collisionTag));
            return;
        }

        GameObject collisionObject = collision.gameObject;
        bool destroyShip = false;
        if (collisionTag == "Hazard") {
            destroyShip = !shieldActive;
            if (shieldActive) {
                enableShield(false);
            }
            GameManager.instance.playerHazardCollision(collision, destroyShip);
        }

        Destroy(collisionObject);
        if (destroyShip) {
            Destroy(gameObject);
        }
    }

    public void OnTriggerEnter2D(Collider2D collision) {
        GameObject collisionObject = collision.gameObject;
        if (collisionObject.name.StartsWith("Prefab_Buff")) {
            buff = collisionObject.GetComponent<PlayerBuff>().getBuff();
            Destroy(collision.gameObject);
        }
    }

    private void moveCheck() {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");

        if (y != 0) {
            float forceMagnitude = Mathf.Sign(y) * 5;
            shipBody.AddForce(transform.up * ((shipBody.velocity.magnitude < 5) ? forceMagnitude : 0));
            thrusterAudiosource.volume = 1;
            if (forceMagnitude > 0) {
                thrusterParticlesystem.Play();
            }
        }
        else if (thrusterAudiosource.volume != 0) {
            thrusterAudiosource.volume = 0;
        }

        if (x != 0) {
            shipBody.AddTorque(x * ((Mathf.Abs(shipBody.angularVelocity) < 500) ? -6 : 0));
        }
    }

    private void shootCheck() {
        if (Input.GetButton("Shoot")) {
            laserEnergy += Time.deltaTime;
        }
        
        if (Input.GetButtonUp("Shoot") && ProjectileManager.getFriendlyProjectiles().Count < 2) {
            ProjectileType pType = laserEnergy <= 0.75 ? ProjectileType.FriendlyRegular : ProjectileType.FriendlyCharged;
            ProjectileManager.createProjectile(transform, pType);
            laserEnergy = 0.0f;
        }
    }

    private void buffCheck() {
        if (Input.GetButtonDown("Buff")) {
            playerAudiosource.PlayOneShot(buff.activate());
            buff = new Buff();
        }
    }

    public void setBuff(Buff par_buff) {
        buff = par_buff;
    }

    public void activateShockwave() {
        //Destroys hazards within player radius
        Vector3 currentPosition = new Vector3(transform.position.x, transform.position.y, 0.0f);
        Instantiate<GameObject>(shockwavePrefab, currentPosition, Quaternion.Euler(0, 0, 0));
    }

    public void activateMachinegun() {
        StartCoroutine(machinegunCoroutine());
    }

    private IEnumerator machinegunCoroutine() {
        //Rapid-fires projectiles
        for (int i = 0; i < machinegunProjectileCount; i++) {
            ProjectileManager.createProjectile(transform, ProjectileType.FriendlyRegular);
            yield return new WaitForSeconds(0.05f);
        }
    }

    public void enableShield(bool isEnabled) {
        shieldActive = isEnabled;
        shieldSpriterenderer.color = new Color(255f, 255f, 255f, isEnabled ? 1f : 0f);
    }
}

