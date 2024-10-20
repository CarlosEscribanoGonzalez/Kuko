using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    GameObject player;
    Rigidbody2D rb;
    private bool isDamaged = false;
    [SerializeField] private AudioSource death;
    [SerializeField] private AudioSource hurt;

    public ParticleSystem fire;
    public bool dead = false;

    public GameObject[] hearts;
    public int life;
    public GameObject kukoImagen;


    void Start()
    {
        player = GameObject.Find("Player");
        rb = GetComponent<Rigidbody2D>();
        life = 5;
    }

    private void CheckLife()
    {
        if (life < 1)
        {
            Destroy(hearts[0].gameObject);
            Destroy(kukoImagen);
            StartCoroutine(GameOver());
        }
        else if (life < 2)
        {
            Destroy(hearts[1].gameObject);

        }
        else if (life < 3)
        {
            Destroy(hearts[2].gameObject);

        }
        else if (life < 4)
        {
            Destroy(hearts[3].gameObject);

        }
        else if (life < 5)
        {
            Destroy(hearts[4].gameObject);

        }
    }


    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Axe") && !isDamaged) //Herido por berserker
        {
            life--;
            CheckLife();
            StartCoroutine(Damager());
            
        }

        if(collision.CompareTag("Spike") && !isDamaged)
        {
            life--;
            CheckLife();
            StartCoroutine(Damager());
        }

        if (collision.CompareTag("Rock") && !isDamaged)
        {
            life--;
            CheckLife();
            StartCoroutine(Damager());
        }
        if(collision.CompareTag("Ground") && !isDamaged)
            {
                life--;
                CheckLife();
                StartCoroutine(Damager());
            }
    }

    IEnumerator GameOver()
    {
        if (!dead)
        {
            fire.Play();
            death.Play();
            dead = true;
            rb.velocity = new Vector2(0, 0);
            yield return new WaitForSeconds(2f);
            death.Stop();
            GameObject transition = GameObject.Find("Transition");
            Animator transAnim = transition.GetComponent<Animator>();
            transAnim.SetBool("start", true);
            yield return new WaitForSeconds(0.5f);
            transAnim.SetBool("start", false);
            SceneManager.LoadScene(2);
        }
    }

    IEnumerator Damager()
    {
        if (!dead)
        {
            isDamaged = true;
            hurt.Play();
            StartCoroutine(ChangeMaterial());
            yield return new WaitForSeconds(0.5f);
            isDamaged = false;
        }
    }

    IEnumerator ChangeMaterial()
    {
        GetComponent<SpriteRenderer>().material = GetComponent<DamageEffect>().blink;
        yield return new WaitForSeconds(0.05f);
        GetComponent<SpriteRenderer>().material = GetComponent<DamageEffect>().transition;
        yield return new WaitForSeconds(0.05f);
        GetComponent<SpriteRenderer>().material = GetComponent<DamageEffect>().blink;
        yield return new WaitForSeconds(0.05f);
        GetComponent<SpriteRenderer>().material = GetComponent<DamageEffect>().original;
    }
}
