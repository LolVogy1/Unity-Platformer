using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    PlayerHero playerHero;
    private int damage;
    private bool canAttack;

    private void OnEnable()
    {
        canAttack = true;
        playerHero = GameObject.Find("HeroKnight").GetComponent<PlayerHero>();
        damage = playerHero.GetDamage();
    }

    // Start is called before the first frame update
    void Start()
    {
        playerHero = GameObject.Find("HeroKnight").GetComponent<PlayerHero>();
    }

    // Works but has errors on non-enemy hitboxes?
    private void OnTriggerEnter2D(Collider2D target)
    {
        if (canAttack && target.gameObject.layer == LayerMask.NameToLayer("Enemies"));
        {
            Debug.Log("Enemy hit");
            Debug.Log("Damage done:" + damage);
            target.GetComponent<Enemy>().TakeDamage(damage);
            canAttack = false;
        }
    }

 
}
