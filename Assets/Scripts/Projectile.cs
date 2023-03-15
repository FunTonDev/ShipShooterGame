using UnityEngine;

public class Projectile : MonoBehaviour {
    private const float projectileLifetimeSeconds = 1.0f;

    private bool isFriendly;
    private ProjectileType type;
    private Transform rootTransform;

    public void Awake() {
        if (type == ProjectileType.FriendlyCharged) {
            transform.localScale = new Vector3(3, 7, 1);
        }
        transform.position = rootTransform.position;
        transform.rotation = rootTransform.rotation;
        gameObject.tag = isFriendly ? "Player" : "Hazard";
        gameObject.layer = LayerMask.NameToLayer(gameObject.tag);
        foreach (Transform child in transform) {
            child.gameObject.layer = gameObject.layer;
        }
        transform.Find("Sprite").GetComponent<SpriteRenderer>().color = isFriendly ? new Color(0.4f, 1.0f, 0.4f, 1.0f) : new Color(1.0f, 0.4f, 0.4f, 1.0f);
        GetComponent<Rigidbody2D>().AddForce(rootTransform.up * (type == ProjectileType.FriendlyRegular ? 700 : 400));
        Invoke("Cleanup", projectileLifetimeSeconds);
    }

    //Collision cases(Friendly): EnemyShip, EnemyLaser, Asteroid
    //Collision cases(Hostile): <ANY>
    public void OnCollisionEnter2D(Collision2D collision) {
        if (isFriendly) {
            GameManager.instance.friendlyProjectileCollision(collision);
        }
        Destroy(collision.gameObject);
        Cleanup();
    }

    public void Init(Transform par_rootTransform, ProjectileType par_type) {
        rootTransform = par_rootTransform;
        type = par_type;
        isFriendly = par_type == ProjectileType.FriendlyRegular || par_type == ProjectileType.FriendlyCharged;
    }

    private void Cleanup() {
        ProjectileManager.removeProjectile(gameObject, type);
        Destroy(gameObject);
    }
}