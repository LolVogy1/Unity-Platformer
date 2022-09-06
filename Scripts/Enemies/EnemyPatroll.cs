using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPatroll : MonoBehaviour
{
    [Header ("patrol points")]
    [SerializeField] private Transform leftBoundary;
    [SerializeField] private Transform rightBoundary;

    [Header("enemy")]
    [SerializeField] private Transform enemy;
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private TutorialEnemy enemyObj;
    private bool dead;

    [Header("Movement")]
    [SerializeField] private float speed;
    private bool movingLeft;
    private float attackCD;

    private void Awake()
    {
        attackCD = 0;
        dead = enemyObj.IsDead();
    }

    private void Update()
    {
        dead = enemyObj.IsDead();
        //increment attack timer
        attackCD += Time.deltaTime;
        // timed attack
        if (!dead)
        {
            if (attackCD >= 5f)
            {
                animator.SetInteger("Anim State", 0);
                //Debug.Log("Enemy will attack");
                enemyObj.EnemyAttack();
                attackCD = 0;
            }
            else if (movingLeft && attackCD > 2f)
            {
                if (enemy.position.x >= leftBoundary.position.x)
                {
                    MoveEnemy(-1);
                }
                else
                {
                    // Change direction
                    DirectionChange();
                }
            }
            else if (!movingLeft && attackCD > 2f)
            {
                if (enemy.position.x <= rightBoundary.position.x)
                {
                    MoveEnemy(1);
                }
                else
                {
                    DirectionChange();
                }
            }
        }
    }

    // Moves the enemy in a direction
    private void MoveEnemy(int direction)
    {
        //Set Direction
        if (movingLeft)
        {
            sprite.flipX = true;
        }
        else
        {
            sprite.flipX = false;
        }

        // Move enemy
        animator.SetInteger("Anim State", 1);
        animator.SetBool("Grounded", true);
        enemy.position = new Vector2(enemy.position.x + Time.deltaTime * direction * speed, enemy.position.y);
    }

    private void DirectionChange()
    {
        movingLeft = !movingLeft;
    }
}
