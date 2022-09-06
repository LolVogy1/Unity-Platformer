using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    // Components
    protected Animator animator;
    protected Rigidbody2D body;
    protected BoxCollider2D hitbox;

    // Stats
    protected int maxHealth;
    protected int currentHealth;
    protected int damage;
    protected bool grounded;
    protected float attackCD;
    protected bool dead;

    // Start is called before the first frame update
    void Start()
    {
        //Debug.Log("Starting Health" + currentHealth);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public virtual void TakeDamage(int damage)
    {
        //Debug.Log("Current Health: " + currentHealth);
        currentHealth -= damage;
        //Debug.Log("Took " + damage + " damage");
        //Debug.Log("Health: " + currentHealth);
        if (currentHealth <= 0)
        {
            Die();
        }
        
    }
    void Die()
    {
        Debug.Log("Enemy Died");
    }
    public abstract void EnemyAttack();

    public abstract void EnemyIdle();

    protected void InitEnemy(int inHealth, int inDamage)
    {
        maxHealth = inHealth;
        damage = inDamage;
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
        body = GetComponent<Rigidbody2D>();
        hitbox = GetComponent<BoxCollider2D>();
    }
    public int GetDamage()
    {
        return damage;
    }
}
