using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectible2D : MonoBehaviour
{

    public float rotationSpeed = 0.5f;
    public GameObject onCollectEffect;
    public AudioClip coinSoundClip;

    void Update()
    {
        transform.Rotate(0, 0, rotationSpeed);
        
    }

    private void OnTriggerEnter2D(Collider2D other) {
        
        if (other.GetComponent<PlayerController>() != null) 
        {
            SoundFXManager.instance.PlaySoundFXClip(coinSoundClip, transform, 1f);
            CoinCount.count++;
            Destroy(gameObject);
            Instantiate(onCollectEffect, transform.position, transform.rotation);
        }
    }


}


