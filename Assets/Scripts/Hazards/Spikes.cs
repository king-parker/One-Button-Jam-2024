using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spikes : MonoBehaviour
{
    private GameManager gameManager;

    void Start()
    {
        if (gameManager == null)
        {
            gameManager = GameObject.FindWithTag(Tags.GameManager).GetComponent<GameManager>();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == Layers.Player)
        {
            PlayerController player = collision.gameObject.GetComponent<PlayerController>();
            gameManager.GameOver();
        }
    }
}
