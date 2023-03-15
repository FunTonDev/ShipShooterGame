using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO add angular deviation for shooting(prevent dead-on accuracy)
//TODO Resolve faulty player destruction when shielded(more in Projectile.cs)
public class EnemyShip : MonoBehaviour {
    private Rigidbody2D shipBody;

    public void Awake() {
        shipBody = gameObject.GetComponent<Rigidbody2D>();
        Vector3 beginningVec = new Vector3(Random.value, Random.value, 0);
        shipBody.velocity = beginningVec;
        transform.up = beginningVec;

        StartCoroutine(Common.lerpAlpha(transform.Find("Sprite").GetComponent<SpriteRenderer>(), 1.0f));
    }

    public void Update() {
        //Attempt to chase/attack player when within detection radius
        if (GameManager.instance.playerAlive) {
            Vector3 playerToEnemyship = GameManager.instance.currentPlayer.transform.position - transform.position;

            if (playerToEnemyship.magnitude < 5f) {
                shipBody.AddTorque(1.5f * Mathf.Sign(Vector2.SignedAngle(transform.up, playerToEnemyship)));
                shipBody.velocity = transform.up;
                if (ProjectileManager.getHostileProjectiles().Count == 0) {
                    ProjectileManager.createProjectile(transform, ProjectileType.HostileRegular);
                }
            }
        }
    }
}
