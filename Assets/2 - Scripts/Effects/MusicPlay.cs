using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlay : MonoBehaviour
{
    [SerializeField] private AudioSource bossTheme;
    [SerializeField] private AudioSource ambientSound;
    private bool collided = false;

    void OnTriggerEnter2D(Collider2D collision) 
    {
        if (collision.CompareTag("Player") && !collided)
        {
            bossTheme.Play();
            ambientSound.Stop();
            collided = true;
        }
    }
}
