using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{

    private Animator animator;
    private Rigidbody2D body;

    [SerializeField] private Transform attackPoint;
    [SerializeField] private float attackRange;
    [SerializeField] private LayerMask enemyLayers;

    public int attackDamage = 10;
    float attackCooldown = 0;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        body = GetComponent<Rigidbody2D>();
        attackPoint = GetComponent<Transform>();
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (attackCooldown > 0.5f)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Attack();

            }
        }
        else
        {
            attackCooldown += Time.deltaTime;
        }
    }
    void Attack()
    {
        animator.SetTrigger("attack");
        /* rework this as the hitbox seems quesitonable */
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

        foreach(Collider2D enemy in hitEnemies)
        {
            Debug.Log("hit enemy");
            enemy.GetComponent<Enemy>().TakeDamage(attackDamage);
        }
        attackCooldown = 0;
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
        {
            return;
        }
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
