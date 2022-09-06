using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialEnemy : Enemy
{
    private EnemySensor groundSensor;
    float deathTimer;
    // Start is called before the first frame update
    void Start() 
    {
        dead = false;
        deathTimer = 0;
        groundSensor = transform.Find("GroundSensor").GetComponent<EnemySensor>();
        int mHealth = 30;
        int dmg = 1;
        attackCD = 0;
        InitEnemy(mHealth, dmg);
        grounded = true;
        animator.SetBool("Grounded", grounded);

    }
    

    // Update is called once per frame
    void Update()
    {
        // Check whether enemy is grounded
        grounded = groundSensor.State();
        animator.SetBool("Grounded", grounded);
        if (dead)
        {
            deathTimer += Time.deltaTime;
            if (deathTimer > 1f)
            {

            }
        }

    }

    public override void EnemyAttack()
    {
        //Debug.Log(grounded);
        if (grounded)
        {
            //Debug.Log("Enemy attacks");
            animator.SetTrigger("Attack");
        }
    }

    public override void EnemyIdle()
    {
        throw new System.NotImplementedException();
    }
    public override void TakeDamage(int damage)
    {
        //Debug.Log("Current Health: " + currentHealth);
        currentHealth -= damage;
        //Debug.Log("Took " + damage + " damage");
        //Debug.Log("Health: " + currentHealth);
        if (currentHealth > 0)
        {
            animator.SetTrigger("Damage");
        }
        else 
        {
            dead = true;
            animator.SetTrigger("Die");

        }

    }
    public bool IsDead()
    {
        return dead;
    }

}
