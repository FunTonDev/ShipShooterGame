using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shockwave : MonoBehaviour {
    private const float lifetimeSeconds = 3.1f;

    public void Awake() {
        StartCoroutine(Common.lerpAlpha(transform.Find("Sprite").GetComponent<SpriteRenderer>(), 3.0f, false));
        GameManager.instance.shockwaveCollision(transform.position);
        Destroy(gameObject, lifetimeSeconds);
    }
}
