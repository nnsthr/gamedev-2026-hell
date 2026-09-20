using UnityEngine;
using UnityEngine.InputSystem;
using System;
using Cysharp.Threading.Tasks;

public class PlayerController : MonoBehaviour
{
    Rigidbody2D rb;
    [SerializeField]GameObject plGround;
    [SerializeField]GameObject rightWall;
    [SerializeField]GameObject leftWall;
    [SerializeField]float WalkingAccelerationGround=80f;
    [SerializeField]float WalkingMaxSpeedGround=7f;
    [SerializeField]float WalkingAccelerationSky=15f;
    [SerializeField]float WalkingMaxSpeedSky=7f;
    [SerializeField]float JumpAcceleration=600f;
    [SerializeField]float WallKickAcceleration=400f;
    [SerializeField]float WallKickJumpAcceleration=630f;
    [SerializeField]float StopMultiplier=0.2f;
    [SerializeField]float jumpBuffer=0.1f;
    [SerializeField]float AttackBlowback=400f;
    [SerializeField]int plHP=3;
    [SerializeField]int plScore=0;
    [SerializeField]float MutekiTime=2f;
    bool isGround = false;
    bool isrightWall = false;
    bool isleftWall = false;
    float SpacekeyPressedTime=-999f;
    float WalkingMaxSpeed=7f;
    float WalkingAcceleration=80f;

    void Start()
    {
        Application.targetFrameRate=60;
        this.rb=GetComponent<Rigidbody2D>();
    }
    void Update()
    {
        //空中の操作を鈍化
        if (isGround)
        {
            WalkingMaxSpeed=WalkingMaxSpeedGround;
            WalkingAcceleration=WalkingAccelerationGround;
        }
        else
        {
            WalkingMaxSpeed=WalkingMaxSpeedSky;
            WalkingAcceleration=WalkingAccelerationSky;
        }
        //jump,buffered
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            SpacekeyPressedTime = Time.time;
        }
        bool canbuffer = Time.time-SpacekeyPressedTime<=jumpBuffer;
        if (isGround && canbuffer) 
        {
            rb.AddForce(transform.up*JumpAcceleration);
            //reset SpacekeyPressedTime
            SpacekeyPressedTime=-1*jumpBuffer;
        }
        //go right
        if (Keyboard.current.rightArrowKey.isPressed && this.rb.linearVelocityX <= WalkingMaxSpeed)
        {
            rb.AddForce(transform.right*WalkingAcceleration);
        }
        //go left
        if (Keyboard.current.leftArrowKey.isPressed && this.rb.linearVelocityX >= WalkingMaxSpeed*-1)
        {
            rb.AddForce(transform.right*-1*WalkingAcceleration);
        }
        //Stop
        if((Keyboard.current.rightArrowKey.wasReleasedThisFrame || Keyboard.current.leftArrowKey.wasReleasedThisFrame) && isGround)
        {
            rb.linearVelocityX *= StopMultiplier;
        }
        //壁ズリ、壁ジャン
        //壁ジャンと同時に床を踏むとめっちゃ飛ぶ まあええやろ
        if (isrightWall && !isGround && rb.linearVelocityY<=0 && Keyboard.current.rightArrowKey.isPressed)
        {
            rb.linearVelocityY=-1.5f;
            if (Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                rb.AddForce(new Vector2(WallKickAcceleration*-1,WallKickJumpAcceleration));
            }
        }
        if (isleftWall && !isGround && rb.linearVelocityY<=0 && Keyboard.current.leftArrowKey.isPressed)
        {
            rb.linearVelocityY=-1.5f;
            if (Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                rb.AddForce(new Vector2(WallKickAcceleration,WallKickJumpAcceleration));
            }
        }
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
    public bool attackornot=false;
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            if (plGround.GetComponent<PlayerOnGroundChecker>().IsEnemy() && rb.linearVelocityY<=0)
            {
                Debug.Log("attacked");
                rb.AddForce(transform.up*AttackBlowback);
                plScore += 100;
                attackornot=true;
            }
            else
            {
                Debug.Log("damaged");
                attackornot=false;
                plHP--;
                //被攻撃時無敵状態
                //敵の中で無敵状態解除すると2回ダメージ受けて無敵状態になるバグ
                gameObject.GetComponent<Collider2D>().excludeLayers = LayerMask.GetMask("unpushableEnemy");
                Invoke(nameof(ToggleMuteki),MutekiTime);
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
    void ToggleMuteki()
    {
        gameObject.GetComponent<Collider2D>().excludeLayers &= ~LayerMask.GetMask("unpushableEnemy");
        Debug.Log("not muteki");
    }
}
