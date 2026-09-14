using UnityEngine;

public class CameraController : MonoBehaviour
{
    GameObject player;
    Vector3 adjustment = new Vector3(0,0,-10);
    Vector3 velocity = Vector3.zero;
    float CameraCatchuptime = 0.03f;
    void Start()
    {
        player=GameObject.Find("player");
    }

    void FixedUpdate()
    {
        transform.position = Vector3.SmoothDamp(player.transform.position+adjustment,transform.position,ref velocity,CameraCatchuptime);
    }
}
