using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallMovement : MonoBehaviour
{
    [SerializeField] private AudioSource rolling;
    [SerializeField] private AudioSource bounce;
    Rigidbody2D rb;
    private bool isDamaged = false;
    Enemy enemy;
    private bool canBounce = true;
    private bool canRoll = true;
    public bool isTimeline;
    
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        enemy = GetComponent<Enemy>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Tail") && !isDamaged)
        {
            if (collision.transform.position.x < transform.position.x)
            {
                rb.velocity = new Vector2(enemy.forceBackX, enemy.forceBackY);
            }
            else
            {
                rb.velocity = new Vector2(-enemy.forceBackX, enemy.forceBackY);
            }
            bounce.Play();
        }
        else if (collision.CompareTag("Head") && !isDamaged)
        {
            bounce.Play();
        }
        StartCoroutine(Damager());
    }


    IEnumerator Damager()
    {
        isDamaged = true;
        yield return new WaitForSeconds(0.7f);
        isDamaged = false;
    }

    void Update()
    {
        if (!isTimeline)
        {
            if (rb.transform.position.x > 7 || rb.transform.position.x < -4.5)
            {
                if (canBounce)
                {
                    StartCoroutine(Bounce());
                }
            }
        }
        else
        {
            if (rb.transform.position.x > 7)
            {
                if (canBounce)
                {
                    StartCoroutine(Bounce());
                }
            }
        }

        if((rb.velocity.x >= 0.5 || rb.velocity.x <= 0.5) && rb.velocity.y == 0)
        {
            if (canRoll)
            {
                rolling.Play();
                canRoll = false;
            }
        }
        else
        {
            rolling.Stop();
            canRoll = true;
        }
    }

    IEnumerator Bounce()
    {
        bounce.Play();
        rb.velocity *= new Vector2(-1, 1);
        canBounce = false;
        yield return new WaitForSeconds(1f);
        canBounce = true;
    }
}
