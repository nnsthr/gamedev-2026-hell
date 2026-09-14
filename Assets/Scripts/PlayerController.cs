using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    Rigidbody2D rigit2D;
    public GameObject plGround;
    public GameObject rightWall;
    public GameObject leftWall;
    float WalkingAcceleration=70f;
    float WalkingMaxSpeed=7f;
    float JumpAcceleration=600f;
    float WallKickAcceleration=400f;
    float WallKickJumpAcceleration=630f;
    float StopMultiplier=0.2f;
    float jumpBuffer=0.1f;
    float AttackBlowback=400f;
    int plHP=3;
    int plScore=0;
    bool isGround = false;
    bool isrightWall = false;
    bool isleftWall = false;
    float SpacekeyPressedTime=-999f;
    void Start()
    {
        Application.targetFrameRate=60;
        this.rigit2D=GetComponent<Rigidbody2D>();
    }
    void Update()
    {
        //空中の操作を鈍化
        if (isGround)
        {
            WalkingMaxSpeed=7f;
            WalkingAcceleration=80f;
        }
        else
        {
            WalkingMaxSpeed=5f;
            WalkingAcceleration=15f;
        }
        //jump,buffered
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            SpacekeyPressedTime = Time.time;
        }
        bool canbuffer = Time.time-SpacekeyPressedTime<=jumpBuffer;
        if (isGround && canbuffer) 
        {
            rigit2D.AddForce(transform.up*JumpAcceleration);
            //reset SpacekeyPressedTime
            SpacekeyPressedTime=-1*jumpBuffer;
        }
        //go right
        if (Keyboard.current.rightArrowKey.isPressed && this.rigit2D.linearVelocityX <= WalkingMaxSpeed)
        {
            rigit2D.AddForce(transform.right*WalkingAcceleration);
        }
        //go left
        if (Keyboard.current.leftArrowKey.isPressed && this.rigit2D.linearVelocityX >= WalkingMaxSpeed*-1)
        {
            rigit2D.AddForce(transform.right*-1*WalkingAcceleration);
        }
        //Stop
        if((Keyboard.current.rightArrowKey.wasReleasedThisFrame || Keyboard.current.leftArrowKey.wasReleasedThisFrame) && isGround)
        {
            rigit2D.linearVelocityX *= StopMultiplier;
        }
        //壁ズリ、壁ジャン
        //壁ジャンと同時に床を踏むとめっちゃ飛ぶ
        if (isrightWall && !isGround && rigit2D.linearVelocityY<=0 && Keyboard.current.rightArrowKey.isPressed)
        {
            rigit2D.linearVelocityY=-1.5f;
            if (Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                rigit2D.AddForce(new Vector2(WallKickAcceleration*-1,WallKickJumpAcceleration));
            }
        }
        if (isleftWall && !isGround && rigit2D.linearVelocityY<=0 && Keyboard.current.leftArrowKey.isPressed)
        {
            rigit2D.linearVelocityY=-1.5f;
            if (Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                rigit2D.AddForce(new Vector2(WallKickAcceleration,WallKickJumpAcceleration));
            }
        }
        Debug.Log(attackornot);
    }
    void FixedUpdate()
    {
        //judge onground
        isGround = plGround.GetComponent<PlayerOnGroundChecker>().IsGround();
        //ckeck wall
        isrightWall = rightWall.GetComponent<WallDetector>().IsWall();
        isleftWall = leftWall.GetComponent<WallDetector>().IsWall();
    }
    //踏みつけ、被ダメージ
    //２回踏まないと消えないバグ,attackornotの扱いを要修正
    bool attackornot=false;
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            if (plGround.GetComponent<PlayerOnGroundChecker>().IsEnemy() && rigit2D.linearVelocityY<=0)
            {
                Debug.Log("attacked");
                rigit2D.AddForce(transform.up*AttackBlowback);
                plScore += 100;
                attackornot=true;
            }
            else
            {
                Debug.Log("damaged");
                plHP--;
                //被攻撃時無敵状態,未完成
                gameObject.GetComponent<Collider2D>().excludeLayers = LayerMask.GetMask("unpushableEnemy");
                attackornot=false;
            }
        }
    }
    //敵側に攻撃を伝えるためのmethod
    public bool CheckAttacked()
    {
        return attackornot;
    }
    //HPを調べる
    public int CheckremainHP()
    {
        return plHP;
    }
    public int CheckplScore()
    {
        return plScore;
    }
}
