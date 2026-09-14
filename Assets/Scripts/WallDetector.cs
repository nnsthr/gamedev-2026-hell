using UnityEngine;

//カベキックのための壁判定
public class WallDetector : MonoBehaviour
{
    bool wallEnter,wallStay,wallExit;
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Ground")
        {
            wallEnter=true;
        }
    }
    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Ground")
        {
            wallStay=true;
        }
    }
    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Ground")
        {
            wallExit=true;
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
}
