using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class powerUp : MonoBehaviour
{
    public AudioSource spawnSource;
    
    // Start is called before the first frame update
    void Start()
    {
        spawnSource.Play();
        int powerNum = Random.Range(1,4);
        if (powerNum == 1)
        {
            gameObject.tag = "Shockwave Power";
        }
        else if (powerNum == 2)
        {
            gameObject.tag = "Machinegun Power";
        }
        else
        {
            gameObject.tag = "Shield Power";
        }
        Destroy(gameObject, 8.0f);
        StartCoroutine(fadeOut());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator fadeOut()
    {
        SpriteRenderer renderer = gameObject.GetComponent<SpriteRenderer>();
        renderer.color = new Color(255f, 255f, 255f, 1f);
        while (renderer.color.a > 0.0f)
        {
            renderer.color = new Color(255.0f, 255.0f, 255.0f, renderer.color.a - (Time.deltaTime / 8.0f));
            yield return null;
        }
    }
}
