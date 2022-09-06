using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHero : MonoBehaviour
{
    [SerializeField] float m_speed = 4.0f;
    [SerializeField] float m_jumpForce = 7.5f;
    [SerializeField] float m_rollForce = 6.0f;
    [SerializeField] bool m_noBlood = false;
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private LayerMask boundaryLayer;
    [SerializeField] private LayerMask ledgeLayer;
    [SerializeField] GameObject m_slideDust;

    // Components
    private Animator m_animator;
    private Rigidbody2D m_body2d;
    private BoxCollider2D hitbox;
    private Sensor_HeroKnight m_groundSensor;
    private Sensor_HeroKnight m_wallSensorR1;
    private Sensor_HeroKnight m_wallSensorR2;
    private Sensor_HeroKnight m_wallSensorL1;
    private Sensor_HeroKnight m_wallSensorL2;

    // Animation handling
    private bool m_isWallSliding = false;
    private bool m_isOnLedge = false;
    private bool m_grounded = false;
    private bool m_rolling = false;
    private int m_facingDirection = 1;
    private int m_currentAttack = 0;
    private float m_timeSinceAttack = 0.0f;
    private float m_delayToIdle = 0.0f;
    private float m_rollDuration = 8.0f / 14.0f;
    private float m_rollCurrentTime;

    // Stats
    private int maxHealth;
    private int currentHealth;
    private int damage;


    // Use this for initialization
    void Start()
    {
        // Base Stats
        maxHealth = 100;
        currentHealth = maxHealth;
        damage = 10;
        // Components
        m_animator = GetComponent<Animator>();
        m_body2d = GetComponent<Rigidbody2D>();
        hitbox = GetComponent<BoxCollider2D>();
        m_groundSensor = transform.Find("GroundSensor").GetComponent<Sensor_HeroKnight>();
        m_wallSensorR1 = transform.Find("WallSensor_R1").GetComponent<Sensor_HeroKnight>();
        m_wallSensorR2 = transform.Find("WallSensor_R2").GetComponent<Sensor_HeroKnight>();
        m_wallSensorL1 = transform.Find("WallSensor_L1").GetComponent<Sensor_HeroKnight>();
        m_wallSensorL2 = transform.Find("WallSensor_L2").GetComponent<Sensor_HeroKnight>();
    }

    // Update is called once per frame
    void Update()
    {
        // Increase timer that controls attack combo
        m_timeSinceAttack += Time.deltaTime;

        // Increase timer that checks roll duration
        if (m_rolling)
            m_rollCurrentTime += Time.deltaTime;

        // Disable rolling if timer extends duration
        // and if there is space
        if (m_rollCurrentTime > m_rollDuration)
        {
            if (!RollingHitCeiling()) { 
                m_rolling = false;
                m_rollCurrentTime = 0;
            }
            else
            {
                // Continue the rolling animation
                m_animator.SetTrigger("Roll");
                // Disable the upper sensors for some amount of time
                m_wallSensorL2.Disable(0.2f);
                m_wallSensorR2.Disable(0.2f);
                // Move the player
                m_body2d.velocity = new Vector2(m_rollForce * (float)m_facingDirection, 0);
            }
        }
        // Check if character just landed on the ground
        if (!m_grounded && m_groundSensor.State())
        {
            m_grounded = true;
            m_animator.SetBool("Grounded", m_grounded);
        }

        //Check if character just started falling
        if (m_grounded && !m_groundSensor.State())
        {
            m_grounded = false;
            m_animator.SetBool("Grounded", m_grounded);
        }

        // -- Handle input and movement --
        float inputX = Input.GetAxis("Horizontal");

        // Swap direction of sprite depending on walk direction
        if (inputX > 0)
        {
            GetComponent<SpriteRenderer>().flipX = false;
            m_facingDirection = 1;
        }

        else if (inputX < 0)
        {
            GetComponent<SpriteRenderer>().flipX = true;
            m_facingDirection = -1;
        }

        // Move wall sliding check here to prevent getting stuck in boundary
        m_isWallSliding = (((m_wallSensorR1.State() && m_wallSensorR2.State()) || (m_wallSensorL1.State() && m_wallSensorL2.State())));
        
        // Check if on ledge
        // if not grounded and lower sensor is false but upper sensor is true
        m_isOnLedge = (!m_grounded && onWall(m_facingDirection) && ((m_wallSensorR1.State() && !m_wallSensorR2.State()) || (m_wallSensorL1.State() && !m_wallSensorL2.State())));

        // Move
        // if not rolling
        // if not on a ledge
        // if not touching a wall/boundary or touching it but in the air (this prevents getting stuck)
        if (!m_rolling && !m_isOnLedge && ((m_isWallSliding && m_grounded) || !m_isWallSliding))
        {
            m_body2d.velocity = new Vector2(inputX * m_speed, m_body2d.velocity.y);
        }
        // if on ledge keep going up until ledge is cleared
        else if (m_isOnLedge && m_body2d.velocity.y > 0 && Mathf.Abs(inputX) > 0)
        {
            m_body2d.velocity = new Vector2(0, m_jumpForce);
        }

        // Set AirSpeed in animator
        m_animator.SetFloat("AirSpeedY", m_body2d.velocity.y);

        // -- Handle Animations --
        //Wall Slide
        //m_isWallSliding = (((m_wallSensorR1.State() && m_wallSensorR2.State()) || (m_wallSensorL1.State() && m_wallSensorL2.State())));

        // check we're on a wall, not a boundary
        if (m_isWallSliding && !onWall(m_facingDirection))
        {
            m_isWallSliding = false;
        }
        m_animator.SetBool("WallSlide", m_isWallSliding);


        /*
        //Death
        if (Input.GetKeyDown("e") && !m_rolling)
        {
            m_animator.SetBool("noBlood", m_noBlood);
            m_animator.SetTrigger("Death");
        }

        //Hurt
        else if (Input.GetKeyDown("q") && !m_rolling)
            m_animator.SetTrigger("Hurt");
        */

        // Attack
        if (Input.GetMouseButtonDown(0) && m_timeSinceAttack > 0.25f && !m_rolling)
        {
            m_currentAttack++;

            // Loop back to one after third attack
            if (m_currentAttack > 3)
                m_currentAttack = 1;

            // Reset Attack combo if time since last attack is too large
            if (m_timeSinceAttack > 1.0f)
                m_currentAttack = 1;

            // Call one of three attack animations "Attack1", "Attack2", "Attack3"
            m_animator.SetTrigger("Attack" + m_currentAttack);

            // Reset timer
            m_timeSinceAttack = 0.0f;
        }

        // Block
        else if (Input.GetMouseButtonDown(1) && !m_rolling)
        {
            m_animator.SetTrigger("Block");
            m_animator.SetBool("IdleBlock", true);
        }

        else if (Input.GetMouseButtonUp(1))
            m_animator.SetBool("IdleBlock", false);

        // Roll
        else if (Input.GetKeyDown("left shift") && !m_rolling && !m_isWallSliding)
        {
            m_rolling = true;
            m_animator.SetTrigger("Roll");
            m_body2d.velocity = new Vector2(m_facingDirection * m_rollForce, m_body2d.velocity.y);
            m_wallSensorL2.Disable(m_rollDuration);
            m_wallSensorR2.Disable(m_rollDuration);
        }


        // Jump
        else if (Input.GetKeyDown("space") && m_grounded && !m_rolling && !m_isOnLedge)
        {
            m_animator.SetTrigger("Jump");
            m_grounded = false;
            m_animator.SetBool("Grounded", m_grounded);
            m_body2d.velocity = new Vector2(m_body2d.velocity.x, m_jumpForce);
            m_groundSensor.Disable(0.2f);
        }
        // Wall Jump
        else if (Input.GetKeyDown("space") && !m_grounded && m_isWallSliding)
        {
            m_animator.SetTrigger("Jump");
            m_grounded = false;
            m_animator.SetBool("Grounded", m_grounded);

            if (inputX == 0)
            {
                m_body2d.velocity = new Vector2(-Mathf.Sign(transform.localScale.x) * 10, 0);
            }
            else
            {
                m_body2d.velocity = new Vector2(-Mathf.Sign(transform.localScale.x) * 5, m_jumpForce);
            }
            m_groundSensor.Disable(0.2f);
        }
        // Run
        else if (Mathf.Abs(inputX) > Mathf.Epsilon && !m_isOnLedge && ((m_isWallSliding && m_grounded) || !m_isWallSliding))
        {
            // Reset timer
            m_delayToIdle = 0.05f;
            m_animator.SetInteger("AnimState", 1);
        }

        // Idle
        else
        {
            // Prevents flickering transitions to idle
            m_delayToIdle -= Time.deltaTime;
            if (m_delayToIdle < 0)
                m_animator.SetInteger("AnimState", 0);
        }
    }

    // Animation Events
    // Called in slide animation.
    void AE_SlideDust()
    {
        Vector3 spawnPosition;

        if (m_facingDirection == 1)
            spawnPosition = m_wallSensorR2.transform.position;
        else
            spawnPosition = m_wallSensorL2.transform.position;

        if (m_slideDust != null)
        {
            // Set correct arrow spawn position
            GameObject dust = Instantiate(m_slideDust, spawnPosition, gameObject.transform.localRotation) as GameObject;
            // Turn arrow in correct direction
            dust.transform.localScale = new Vector3(m_facingDirection, 1, 1);
        }
    }
    private bool onWall(int direction)
    {
        RaycastHit2D raycastHit;

        if (direction == 1)
        {
            raycastHit = Physics2D.BoxCast(hitbox.bounds.center, hitbox.bounds.size, 0, new Vector2(transform.localScale.x, 0), 0.2f, wallLayer);
        }
        else
        {
            raycastHit = Physics2D.BoxCast(hitbox.bounds.center, hitbox.bounds.size, 0, new Vector2(-transform.localScale.x, 0), 0.2f, wallLayer);
        }
        return raycastHit.collider != null;

    }
    private bool onBoundary(int direction)
    {
        RaycastHit2D raycastHit;
        if (direction == 1)
        {
            raycastHit = Physics2D.BoxCast(hitbox.bounds.center, hitbox.bounds.size, 0, new Vector2(transform.localScale.x, 0), 0.2f, boundaryLayer);
        }
        else
        {
            raycastHit = Physics2D.BoxCast(hitbox.bounds.center, hitbox.bounds.size, 0, new Vector2(-transform.localScale.x, 0), 0.2f, boundaryLayer);
        }
        return raycastHit.collider != null;
    }
    // Check if in contact with a ceiling
    private bool RollingHitCeiling()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(hitbox.bounds.center, hitbox.bounds.size, 0, new Vector2(0, transform.localScale.y), 10f, wallLayer);
        return raycastHit.collider != null;
    }

    public void TakeDamage(int damageTaken)
    {
        currentHealth -= damageTaken;
        if (currentHealth <= 0)
        {
            m_animator.SetTrigger("Death");
        }
        else
        {
            m_animator.SetTrigger("Hurt");
        }
    }

    public int GetDamage()
    {
        return damage;
    }
}
