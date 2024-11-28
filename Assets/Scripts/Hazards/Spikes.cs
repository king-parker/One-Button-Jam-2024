using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spikes : MonoBehaviour
{
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //print("Spikes entered");
        if (collision.gameObject.layer == Layers.Player)
        {
            //print("Player died");
            PlayerController player = collision.gameObject.GetComponent<PlayerController>();
            player.Death();
        }
    }
}
