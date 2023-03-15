using UnityEngine;

public class Debris : MonoBehaviour {
    public void Awake() {
        Rigidbody2D asteroidRigidbody = GetComponent<Rigidbody2D>();
        asteroidRigidbody.AddForce(new Vector3(Random.Range(-100, 100), Random.Range(-100, 100), 0));
        asteroidRigidbody.AddTorque(Random.Range(-50f, 50f));

        GameObject asteroidSubtype = Random.value < 0.5f ? transform.GetChild(0).gameObject :
                                        transform.GetChild(1).gameObject;
        CircleCollider2D borderHandlerCollider = transform.Find("BorderHandler").GetComponent<CircleCollider2D>();
        borderHandlerCollider.radius = asteroidSubtype.name == "0" ? 0.9f : 0.5f;
        asteroidSubtype.SetActive(true);
        StartCoroutine(Common.lerpAlpha(asteroidSubtype.GetComponent<SpriteRenderer>(), 1.0f));
    }
}
