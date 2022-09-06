using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    TutorialEnemy tEnemy;
    private int damage;
    private bool canAttack;
    // Start is called before the first frame update
    void OnEnable()
    {
        canAttack = true;
        tEnemy = GameObject.Find("Tutorial Enemy").GetComponent<TutorialEnemy>();
        damage = tEnemy.GetDamage();
    }

    private void Start()
    {
        tEnemy = GameObject.Find("Tutorial Enemy").GetComponent<TutorialEnemy>();
    }

    private void OnTriggerEnter2D(Collider2D target)
    {
        if (canAttack && target.gameObject.layer == LayerMask.NameToLayer("Player")) ;
        {
            //Debug.Log("Enemy hit");
            //Debug.Log("Damage done:" + damage);
            target.GetComponent<PlayerHero>().TakeDamage(damage);
            canAttack = false;
        }
    }

}
