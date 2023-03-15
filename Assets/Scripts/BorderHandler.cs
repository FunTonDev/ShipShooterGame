using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BorderHandler : MonoBehaviour {
    private Transform parentTransform;

    public void Awake() {
        parentTransform = transform.parent.gameObject.transform;
    }
    
    public void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.tag == "Border") {
            Vector3 newPos = parentTransform.position;
            if (collision.gameObject.name == "Left" || collision.gameObject.name == "Right") {
                newPos.x *= -1;
            }
            else {
                newPos.y *= -1;
            }
            parentTransform.position = newPos;
        }
    }
}
