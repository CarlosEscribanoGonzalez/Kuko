using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BerserkerBoss : MonoBehaviour
{

    Rigidbody2D rb;
    Enemy enemy;
    GameObject player;
    public float fallGravityMultiplier;
    public float speed;
    public float jumpSpeed;

    public Transform wallCheck;
    public bool isWalled;
    public float wallCheckRadius;
    public LayerMask whatIsWall;

    EnemyHealth variable; //Para obtener cualquier variable de la clase EnemyHealth
    Animator anim;
    private bool attacking = false;
    private bool hasAttacked = false;
    private bool noRepetition = true; //Para que las coroutines se ejecuten únicamente una vez
    private int random; //Para hacer ataques aleatorios
    private bool willAttack = true; //Para generar un número aleatorio para el ataque
    private bool inJump = false;

    [SerializeField] AudioSource axeAttack;
    [SerializeField] AudioSource jumpAttack;

    private bool dead;
    private bool fightStarted = false;

    [SerializeField] private GameObject spikes;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        enemy = GetComponent<Enemy>();
        player = GameObject.Find("Player");
        variable = GetComponent<EnemyHealth>();
        anim = GetComponent<Animator>();
        speed = enemy.speed;
    }

    // Update is called once per frame
    void Update()
    {
        dead = variable.dead;

        isWalled = Physics2D.OverlapCircle(wallCheck.position, wallCheckRadius, whatIsWall);
        
        if (isWalled)
        {
            Flip();
            if (inJump)
            {
                rb.velocity += new Vector2(10 * enemy.transform.localScale.x, 0);
            }
        }

        if(transform.position.x - player.transform.position.x <= 10)
        {
            fightStarted = true;
        }

    }

    void FixedUpdate()
    {
        if (!dead && fightStarted)
        {
            if (willAttack)
            {
                random = Random.Range(1, 3);
                willAttack = false;
            }
            else if (!willAttack && random == 1)
            {
                random = 1;
            }
            else
            {
                random = 0;
            }

            if (random == 1)
            {
                if (!hasAttacked)
                {
                    FlippingAttack();
                    if (noRepetition && attacking)
                    {
                        StartCoroutine(Duration());
                    }
                }
                else if (hasAttacked)
                {
                    anim.SetBool("dizzy", true);
                    anim.SetBool("attack", false);
                    speed = 0;
                    attacking = false;
                    if (noRepetition)
                    {
                        StartCoroutine(Dizzed());
                    }
                }
            }
            else if (random == 2)
            {
                StartCoroutine(JumpAttack());
                StartCoroutine(JumpAttackAudio());
            }

            if (rb.velocity.y < 0)
            {
                rb.velocity += Vector2.up * Physics2D.gravity.y * (fallGravityMultiplier - 1) * Time.fixedDeltaTime;
            }
        }
        else
        {
            rb.velocity = new Vector2(0, 0);
            anim.SetBool("attack", false);
        }
    }

    void Flip()
    {
        if (enemy.transform.localScale.x < 0)
        {
            enemy.transform.localScale = new Vector3(2, 2, 2);
        }
        else
        {
            enemy.transform.localScale = new Vector3(-2, 2, 2);
        }
    }

    void FlippingAttack()
    {
        anim.SetBool("attack", true);
        speed = 10;
        rb.velocity = new Vector2(enemy.transform.localScale.x * speed, 0);
        if (noRepetition)
        {
            StartCoroutine(Duration());
        }
    }

    void SingleJump()
    {
        anim.SetBool("jump", true);
        rb.velocity = new Vector2(enemy.transform.localScale.x * 5, jumpSpeed);
    }

    IEnumerator Duration()
    {
        noRepetition = false;
        axeAttack.Play();
        yield return new WaitForSeconds(5f);
        axeAttack.Stop();
        hasAttacked = true;
        noRepetition = true;
    }

    IEnumerator Dizzed()
    {
        noRepetition = false;
        rb.velocity = new Vector2(0, 0);
        yield return new WaitForSeconds(3f);
        hasAttacked = false;
        anim.SetBool("dizzy", false);
        noRepetition = true;
        willAttack = true;
    }

    IEnumerator JumpAttack()
    {
        inJump = true;
        SingleJump();
        yield return new WaitForSeconds(3f);
        anim.SetBool("jump", false);
        SingleJump();
        yield return new WaitForSeconds(3f);
        anim.SetBool("jump", false);
        SingleJump();
        yield return new WaitForSeconds(3f);
        anim.SetBool("jump", false);
        inJump = false;
        willAttack = true;
    }

    IEnumerator JumpAttackAudio()
    {
        yield return new WaitForSeconds(2.2f);
        jumpAttack.Play();
        StartCoroutine(GenerateSpikes());
        yield return new WaitForSeconds(3f);
        jumpAttack.Play();
        StartCoroutine(GenerateSpikes());
        yield return new WaitForSeconds(3f);
        jumpAttack.Play();
        StartCoroutine(GenerateSpikes());
    }

    IEnumerator GenerateSpikes()
    {
        foreach(Transform child in spikes.transform)
        {
            child.gameObject.SetActive(true);
        }

        yield return new WaitForSeconds(0.8f);
        
        foreach (Transform child in spikes.transform)
        {
            child.gameObject.SetActive(false);
        }
    }
}
