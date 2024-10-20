using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //VELOCIDADES:
    public float walkSpeed, runSpeed, jumpSpeed, wallSpeed; 
    private float speed;
    float velX, velY;

    //DETECCIÓN DE COLISIONES:
    Rigidbody2D rb;
    public Transform groundCheck;
    public bool isGrounded;
    public float groundCheckRadius;
    public LayerMask whatIsGround;

    public Transform wallCheck;
    public bool isWalled;
    public float wallCheckRadius;
    public LayerMask whatIsWall;
    private bool wallJumped = false;

    //SALTOS Y MOVIMIENTOS:
    private bool jumpPressed = false;
    public bool midAir = false;
    private int jumpCount;
    public float fallGravityMultiplier = 3; //Multiplicador para hacer que caer sea más rápido que saltar
    private bool cabezazoListo = true;

    private bool checkDirection = true;
    float multiplier = 1;

    //ANIMADOR:
    Animator anim;

    //AUDIO:
    private bool reproduceWalk = true;
    private bool reproduceRun = true;
    private bool reproduceIdleWall = true;
    private bool reproduceClimb = true;

    [SerializeField] AudioSource dragonAttack;
    [SerializeField] AudioSource jump;
    [SerializeField] AudioSource walk;
    [SerializeField] AudioSource run;
    [SerializeField] AudioSource idleWall;
    [SerializeField] AudioSource climb;

    private bool dead;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        jumpCount = 0;
    }

    // Update is called once per frame
    void Update()
    {
        dead = GetComponent<PlayerHealth>().dead;


        if (!dead)
        {
            FlipCharacter();

            isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, whatIsGround);
            isWalled = Physics2D.OverlapCircle(wallCheck.position, wallCheckRadius, whatIsWall);

            if (Input.GetButtonDown("Jump"))
            {
                jumpPressed = true;
            }

            if (isGrounded)
            {
                jumpCount = 0;
                anim.SetBool("jump", false);
                anim.SetBool("doubleJump", false);
                midAir = false;
                cabezazoListo = true;
            }
            else
            {
                anim.SetBool("jump", true);
                midAir = true;
                walk.Stop();
                run.Stop();
                reproduceWalk = true;
                reproduceRun = true;
            }

            if (rb.velocity.y < jumpSpeed * 0.7 && !isWalled)
            {
                checkDirection = true;
            }

            if(!isWalled && transform.localScale.y == -1)
            {
                transform.localScale *= new Vector2(1, -1);
            }

            Colazo();
            Cabezazo();
        }
        else
        {
            anim.SetBool("walk", false);
            anim.SetBool("run", false);
            anim.SetBool("jump", false);
            anim.SetBool("doubleJump", false);
            anim.SetBool("climb", false);
            anim.SetBool("idleWall", false);
            anim.SetBool("golpeCabezazo", false);
            anim.SetBool("golpeCola", false);
            walk.Stop();
            run.Stop();
            idleWall.Stop();
            climb.Stop();
        }
    }

    private void FixedUpdate() //En FixedUpdate se guarda todo aquello perteneciente a Update que requiera movimientos físicos.
    {
        if (!dead)
        {
            Movement();
            Jump();
            Climb();


            if (rb.velocity.y < 0)
            {
                rb.velocity += Vector2.up * Physics2D.gravity.y * (fallGravityMultiplier - 1) * Time.fixedDeltaTime;
            }

            if (midAir && rb.velocity.y < 0 && ((rb.velocity.x >= 0 && Input.GetKey(KeyCode.A)) || (rb.velocity.x <= 0 && Input.GetKey(KeyCode.D)))) 
            {
                float velMidAir = Input.GetAxisRaw("Horizontal");
                rb.velocity = new Vector2(velMidAir * 5, rb.velocity.y);
            }

            if(midAir && !isWalled && rb.velocity.y > 7.5 && rb.velocity.y < 8) //Al despegarse de una pared
            {
                rb.velocity = new Vector2(0, 0);
                rb.velocity = new Vector2(transform.localScale.x * 3, 3);
            }
        }
    }

    void Movement()
    {
        velX = Input.GetAxisRaw("Horizontal");
        velY = rb.velocity.y;

        if (Input.GetKey(KeyCode.LeftShift) && isGrounded && rb.velocity.x != 0)
        {
            anim.SetBool("run", true);
            speed = runSpeed;
            if (reproduceRun)
            {
                run.Play();
                reproduceRun = false;
            }
        }
        else if (midAir) //Para que mantenga la velocidad que llevaba previamente en el aire hasta tocar el suelo
        {
            return;
        }
        else if (velX == 0)
        {
            anim.SetBool("run", false);
            run.Stop();
            reproduceRun = true;
        }
        else
        {
            anim.SetBool("run", false);
            speed = walkSpeed;
            run.Stop();
            reproduceRun = true;
        }

        rb.velocity = new Vector2(velX * speed, velY);
        
        if(rb.velocity.x != 0 && speed != runSpeed)
        {
            anim.SetBool("walk", true);
            if (reproduceWalk)
            {
                walk.Play();
                reproduceWalk = false;
            }
        }
        else
        {
            anim.SetBool("walk", false);
            walk.Stop();
            reproduceWalk = true;
        }
    }

    void FlipCharacter()
    {
        float inicial = transform.localScale.x;
        if (!isWalled)
        {
            if (rb.velocity.x < 0)
            {
                transform.localScale = new Vector3(-1, 1, 1);
            }
            else if (rb.velocity.x > 0)
            {
                transform.localScale = new Vector3(1, 1, 1);
            }
        } 
        else if (isWalled)
        {
            if(rb.velocity.y > -jumpSpeed/2) //En este caso el personaje desliza por la pared si el jugador no se mueve, por lo que no tomamos el valor 0
            {
                transform.localScale = new Vector3(inicial, 1, 1); //inicial sirve para que no haya diferencia entre trepar una pared a tu izquierda o una a tu derecha
            }
            else if(rb.velocity.y < -jumpSpeed/2)
            {
                transform.localScale = new Vector3(inicial, -1, 1);
            }
        }
    }

    void Jump()
    {
        if (jumpPressed && !isWalled)
        {
            jumpPressed = false;
            if(isGrounded && jumpCount == 0)
            {
                rb.velocity += Vector2.up * jumpSpeed;
                jump.Play();
            }

            else if(!isGrounded && jumpCount == 0)
            {
                if (velX == 0 && rb.velocity.x != 0)
                {
                    velX = transform.localScale.x;
                }
                else if(velX == 0 && rb.velocity.x == 0)
                {
                    velX = 0;
                }
                rb.velocity = new Vector2(velX * speed, 0); //Se le quita la velocidad que tenía antes en Y para que ambos saltos no junten sus impulsos
                rb.velocity = new Vector2(velX * speed, jumpSpeed * 3/4);
                anim.SetBool("doubleJump", true);
                jump.Play();
                jumpCount++; //Se usa como int por si acaso se le puede dar otro salto extra más adelante en el juego
            }
            else if (wallJumped && rb.velocity.y <= jumpSpeed * 0.7)
            {
                if (velX == 0)
                {
                    velX = transform.localScale.x;
                }
                wallJumped = false;
                rb.velocity = new Vector2(velX * 10, jumpSpeed * 3/4);
                anim.SetBool("doubleJump", true);
                jump.Play();
            }  
        } 
    }

    void Colazo()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            anim.SetBool("golpeCola", true);
            dragonAttack.Play();
        }
        else
        {
            anim.SetBool("golpeCola", false);
        }
    }

    void Cabezazo()
    {
        if (cabezazoListo == true)
        {
            if (Input.GetButtonDown("Fire2"))
            {
                anim.SetBool("cabezazo", true);
                dragonAttack.Play();
                if (velX != 0)
                {
                    rb.velocity = new Vector2(velX * speed * 2, jumpSpeed / 5);
                }
                else
                {
                    rb.velocity = new Vector2(transform.localScale.x * speed * 2, jumpSpeed / 5);
                }
                cabezazoListo = false;
            }
            else
            {
                anim.SetBool("cabezazo", false);
            }
        }
        else
        {
            anim.SetBool("cabezazo", false);
        }
    }

    void Climb() 
    {
        if (isWalled && midAir)
        {
            run.Stop();
            walk.Stop();
            reproduceRun = true;
            reproduceWalk = true;
            
            
            velY = Input.GetAxisRaw("Vertical");
            jumpCount = 1; //En caso de que se salte desde la pared así no se solapa con el doble salto
            cabezazoListo = true;
            fallGravityMultiplier = 0;
            rb.velocity = new Vector2(0, velY * wallSpeed);
            anim.SetBool("doubleJump", false);
            if (velY != 0)
            {
                anim.SetBool("climb", true);
                anim.SetBool("idleWall", false);
                reproduceIdleWall = true;
                idleWall.Stop();
                if (reproduceClimb)
                {
                    climb.Play();
                    reproduceClimb = false;
                }
            }
            else
            {
                anim.SetBool("idleWall", true);
                anim.SetBool("climb", false);
                reproduceClimb = true;
                climb.Stop();
                if (reproduceIdleWall)
                {
                    idleWall.Play();
                    reproduceIdleWall = false;
                }
            }
            WallJump();
            WallDrop();
        }
        else
        {
            fallGravityMultiplier = 3;
            anim.SetBool("climb", false);
            anim.SetBool("idleWall", false);
            reproduceIdleWall = true;
            reproduceClimb = true;
            idleWall.Stop();
            climb.Stop();
        }
    }

    void WallDrop() //Función para dejar de trepar 
    {
        if (isWalled)
        {
            float velocityX = rb.velocity.x;
            float velocityY = rb.velocity.y;
            if (Input.GetKey(KeyCode.A))
            {
                velocityX = -5;
                velocityY = 0;
            }
            else if (Input.GetKey(KeyCode.D))
            {
                velocityX = 5;
                velocityY = 0;
            }
            rb.velocity = new Vector2(velocityX, velocityY);
        }
    }

    void WallJump()
    {
        if (checkDirection)
        {
            multiplier = transform.localScale.x * -1;
            checkDirection = false;
        }
        if (isWalled && Input.GetKey(KeyCode.Space))
        {
            rb.velocity = new Vector2(multiplier * 10, jumpSpeed);
            wallJumped = true;
            jump.Play();
        }
    }
}

    
