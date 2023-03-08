using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class debris : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
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
}
