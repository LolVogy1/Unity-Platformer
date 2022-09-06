using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance;
    GameObject tEnemyObj;
    TutorialEnemy tEnemy1;
    bool tEnemyIsDead;
 

    // Start is called before the first frame update
    void Start()
    {
        tEnemyObj = GameObject.Find("Tutorial Enemy");
        tEnemy1 = tEnemyObj.GetComponent<TutorialEnemy>();
        tEnemyIsDead = tEnemy1.IsDead();
    }

    // Update is called once per frame
    void Update()
    {
        tEnemyIsDead = tEnemy1.IsDead();
        if (tEnemyIsDead)
        {
            Destroy(tEnemyObj, 1f);       
        }
    }
}
