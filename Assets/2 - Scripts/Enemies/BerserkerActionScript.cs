using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BerserkerActionScript : MonoBehaviour
{

    Rigidbody2D rb;
    Enemy enemy;
    GameObject player;
    public float fallGravityMultiplier;

    public Transform wallCheck;
    public bool isWalled;
    public float wallCheckRadius;
    public LayerMask whatIsWall;

    public Transform groundCheck;
    public bool isGrounded;
    public float groundCheckRadius;
    public LayerMask whatIsGround;

    bool stopMovement;
    EnemyHealth variable; //Para obtener cualquier variable de la clase EnemyHealth
    Animator anim;
    private bool attacking = false;
    private bool canFlip = true;
    private bool hasAttacked = false;
    public float speed;
    private bool noRepetition = true; //Para que las coroutines se ejecuten únicamente una vez

    [SerializeField] AudioSource axeAttack;

    private bool dead;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        enemy = GetComponent<Enemy>();
        player = GameObject.Find("Player");
        variable = GetComponent<EnemyHealth>();
        anim = GetComponent<Animator>();
        speed = enemy.speed;
        anim.SetBool("walk", true);
    }

    // Update is called once per frame
    void Update()
    {
        dead = variable.dead;
        
        isWalled = Physics2D.OverlapCircle(wallCheck.position, wallCheckRadius, whatIsWall);
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, whatIsGround);

        stopMovement = variable.stopMovement;
        
    }

    void FixedUpdate()
    {
        if (!dead)
        {
            if (!stopMovement)
            {
                Walk();
                anim.SetBool("hit", false);
            }
            else
            {
                anim.SetBool("hit", true);
            }

            if (!hasAttacked)
            {
                Attack();
                if (noRepetition && attacking)
                {
                    StartCoroutine(AttackDuration());
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
                    StartCoroutine(IsDizzed());
                }
            }

            if (rb.velocity.y < 0)
            {
                rb.velocity += Vector2.up * Physics2D.gravity.y * (fallGravityMultiplier - 1) * Time.fixedDeltaTime;
            }
        }
        else
        {
            rb.velocity = new Vector2(0, 0);
            anim.SetBool("dizzy", true);
            anim.SetBool("attack", false);
        }
    }

    void Flip()
    {
        if(enemy.transform.localScale.x < 0)
        {
            enemy.transform.localScale = new Vector3(1, 1, 1);
        }
        else
        {
            enemy.transform.localScale = new Vector3(-1, 1, 1);
        }
    }

    void Walk()
    {
        if (!attacking && !hasAttacked)
        {
            speed = 3;
        } 
        else if (attacking)
        {
            speed = 8;
        } 
        
        rb.velocity = new Vector2(enemy.transform.localScale.x * speed, rb.velocity.y);
        
        if ((!isGrounded || isWalled) && !attacking)
        {
            Flip();
        }
    }

    void Attack()
    {

            if (player.transform.position.x - enemy.transform.position.x < 0 && player.transform.position.x - enemy.transform.position.x > -5 && !attacking && player.transform.position.y - enemy.transform.position.y <= 5 && player.transform.position.y - enemy.transform.position.y >= -5)
            {
                axeAttack.Play();
                enemy.transform.localScale = new Vector3(-1, 1, 1);
                anim.SetBool("attack", true);
                attacking = true;
            }
            else if (player.transform.position.x - enemy.transform.position.x < 0 && player.transform.position.x - enemy.transform.position.x > -5 && !attacking && player.transform.position.y - enemy.transform.position.y <= 5 && player.transform.position.y - enemy.transform.position.y >= -5)
            {
                axeAttack.Play();
                enemy.transform.localScale = new Vector3(1, 1, 1);
                anim.SetBool("attack", true);
                attacking = true;
            }
            else if (attacking)
            {
                if ((player.transform.position.x - enemy.transform.position.x > 2 && canFlip && enemy.transform.localScale.x < 0) || (player.transform.position.x - enemy.transform.position.x < -2 && canFlip) && enemy.transform.localScale.x > 0)
                {
                    StartCoroutine(Flipping());
                }
            }
        
    }

    IEnumerator Flipping()
    {
        canFlip = false;
        Flip();
        yield return new WaitForSeconds(1.0f);
        canFlip = true;
    }

    IEnumerator AttackDuration()
    {
        noRepetition = false;
        yield return new WaitForSeconds(5f);
        axeAttack.Stop();
        hasAttacked = true;
        noRepetition = true;
    }

    IEnumerator IsDizzed()
    {
        noRepetition = false;
        yield return new WaitForSeconds(3f);
        hasAttacked = false;
        anim.SetBool("dizzy", false);
        noRepetition = true;
    }
}
