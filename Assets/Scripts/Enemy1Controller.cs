using UnityEngine;

//playerが頭だけぶつけた状態で下だけenemyに押されると床にめり込むバグ
public class Enemy1Controller : MonoBehaviour
{
    GameObject player;
    public GameObject onGroundRight;
    public GameObject onGroundLeft;
    public GameObject rightWall;
    public GameObject leftWall;
    Rigidbody2D rb;
    float WalkingSpeed=1f;
    int walkdirection = 1;
    void Start()
    {
        this.player = GameObject.Find("player");
        this.rb = GetComponent<Rigidbody2D>();
    }
    void FixedUpdate()
    {
        //方向転換
        if (!onGroundRight.GetComponent<PlayerOnGroundChecker>().IsGround() || rightWall.GetComponent<WallDetector>().IsWall())
        {
            walkdirection = -1;
        }
        if (!onGroundLeft.GetComponent<PlayerOnGroundChecker>().IsGround() || leftWall.GetComponent<WallDetector>().IsWall())
        {
            walkdirection = 1;
        }
        rb.linearVelocityX = walkdirection*WalkingSpeed;
    }
    //被攻撃時の処理
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag=="Player" && player.GetComponent<PlayerController>().CheckAttacked())
        {
            Debug.Log("Destroyed");
            Destroy(gameObject);
        }
    }
}
