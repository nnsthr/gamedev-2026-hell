using UnityEngine;

//playerの子objectにcolliderを付けて接地判定を行う
public class PlayerOnGroundChecker : MonoBehaviour
{
    bool isGroundEnter,isGroundStay,isGroundExit;
    bool isEnemyEnter,isEnemyStay,isEnemyExit;
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Ground")
        {
            isGroundEnter=true;
        }
        else if (collision.tag == "Enemy")
        {
            isEnemyEnter=true;
        }
    }
    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Ground")
        {
            isGroundStay=true;
        }
        else if (collision.tag == "Enemy")
        {
            isEnemyStay=true;
        }
    }
    void OnTriggerExit2D(Collider2D collision) 
    {
        if (collision.tag == "Ground")
        {
            isGroundExit=true;
        }
        else if (collision.tag == "Enemy")
        {
            isEnemyExit=true;
        }
    }
    public bool IsGround()
    {
        bool isGround = false;
        if(isGroundEnter || isGroundStay)
        {
            isGround=true;
        }
        else if (isGroundExit)
        {
            isGround=false;
        }
        isGroundEnter = isGroundStay = isGroundExit = false;
        return isGround;
    }
    public bool IsEnemy()
    {
        bool isEnemy=false;
        if(isEnemyEnter || isEnemyStay)
        {
            isEnemy=true;
        }
        else if (isEnemyExit)
        {
            isEnemy=false;
        }
        isEnemyEnter = isEnemyStay = isEnemyExit = false;
        return isEnemy;
    }
}
