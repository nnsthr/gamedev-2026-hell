using UnityEngine;

//壁判定
public class WallDetector : MonoBehaviour
{
    bool wallEnter,wallStay,wallExit;
    bool enemyEnter,enemyStay,enemyExit;
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Ground")
        {
            wallEnter=true;
        }
        else if(collision.tag == "Enemy")
        {
            enemyEnter=true;
        }
    }
    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Ground")
        {
            wallStay=true;
        }
        else if(collision.tag == "Enemy")
        {
            enemyStay=true;
        }
    }
    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Ground")
        {
            wallExit=true;
        }
        else if(collision.tag == "Enemy")
        {
            enemyExit=true;
        }
    }
    public bool IsWall()
    {
        bool iswall=false;
        if (wallEnter || wallStay)
        {
            iswall=true;
        }
        else if (wallExit)
        {
            iswall=false;
        }
        wallEnter=wallStay=wallExit=false;
        return iswall;
    }
    public bool IsEnemy()
    {
        bool isEnemy=false;
        if (enemyEnter || enemyStay)
        {
            isEnemy=true;
        }
        else if (enemyExit)
        {
            isEnemy=false;
        }
        enemyEnter=enemyStay=enemyExit=false;
        return isEnemy;
    }

}
