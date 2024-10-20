using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{

    Enemy enemy;
    Rigidbody2D rb;
    public bool isDamaged = false;
    public bool stopMovement;
    public bool dead = false;
    public ParticleSystem fire;

    [SerializeField] private AudioSource deathEffect;
    [SerializeField] private AudioSource enemyDamaged;


    // Start is called before the first frame update
    void Start()
    {
        enemy = GetComponent<Enemy>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Tail") && !isDamaged) //GOLPE DE COLA
        {
            enemy.healthPoints -= 2; //Daño susceptible de cambios
            enemyDamaged.Play();
            StartCoroutine(Damager());
            StartCoroutine(NoMovement());
            if (enemy.forceBackX != 0 && enemy.forceBackY != 0)
            {
                if (collision.transform.position.x < transform.position.x)
                {
                    rb.velocity = new Vector2(enemy.forceBackX, enemy.forceBackY);
                }
                else
                {
                    rb.velocity = new Vector2(-enemy.forceBackX, enemy.forceBackY);
                }
            }
        }
        else if(collision.CompareTag("Head") && !isDamaged) //CABEZAZO
        {
            enemy.healthPoints -= 5; //Daño susceptible de cambios
            enemyDamaged.Play();
            StartCoroutine(Damager());
        }
        if (enemy.healthPoints <= 0)
        {
            StartCoroutine(Death());
        }
    }

    IEnumerator Damager() //Esta función sirve para que no se detecten colisiones más de una vez por golpe
    {
        isDamaged = true;
        StartCoroutine(ChangeMaterial());
        yield return new WaitForSeconds(0.5f);
        isDamaged = false;
    }

    IEnumerator NoMovement()
    {
        stopMovement = true;
        yield return new WaitForSeconds(1f);
        stopMovement = false;
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

    IEnumerator Death()
    {
        deathEffect.Play();
        dead = true;
        rb.velocity = new Vector2(0, 0);
        fire.Play();
        yield return new WaitForSeconds(1f);
        Destroy(gameObject);
    }
}
