using UnityEngine;

//playerが頭だけぶつけた状態で下だけenemyに押されると床にめり込むバグ
public class Enemy1Controller : MonoBehaviour
{
    GameObject player;
    [SerializeField]GameObject onGroundRight;
    [SerializeField]GameObject onGroundLeft;
    [SerializeField]GameObject rightWall;
    [SerializeField]GameObject leftWall;
    Rigidbody2D rb;
    PlayerController pl;
    [SerializeField]float WalkingSpeed=1f;
    int walkdirection = 1;
    [SerializeField]int enHP = 1;
    void Start()
    {
        this.player = GameObject.Find("player");
        this.pl = player.GetComponent<PlayerController>();
        this.rb = GetComponent<Rigidbody2D>();
    }
    void FixedUpdate()
    {
        //方向転換 walldetectorを使いまわすのはよくないかも
        if (!onGroundRight.GetComponent<PlayerOnGroundChecker>().IsGround() || rightWall.GetComponent<WallDetector>().IsWall() || rightWall.GetComponent<WallDetector>().IsEnemy())
        {
            walkdirection = -1;
        }
        if (!onGroundLeft.GetComponent<PlayerOnGroundChecker>().IsGround() || leftWall.GetComponent<WallDetector>().IsWall() || leftWall.GetComponent<WallDetector>().IsEnemy())
        {
            walkdirection = 1;
        }
        rb.linearVelocityX = walkdirection*WalkingSpeed;
    }
    //被攻撃時の処理
    bool Attackedornot =false;
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag=="Player") 
        {
            Attackedornot = true;
        }
    }
    void LateUpdate()
    {
        if(Attackedornot==true && pl.CheckAttacked())
        {
            Attackedornot=false;
            pl.attackornot=false;
            enHP--;
            if (enHP <= 0)
            {
                Destroy(gameObject);
            }
        }
    }
}
