using UnityEngine;

public class Door : MonoBehaviour
{
    public GameObject player;
    public GameObject cameraa;

    public bool isVertical;

    private float entrySide;

    void Awake()
    {
        player = GameObject.Find("player");
        cameraa = GameObject.Find("Main Camera");
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject != player) return;

        if (!isVertical)
        {
            Vector2 directionToPlayer = player.transform.position - transform.position;

            entrySide = Vector2.Dot(directionToPlayer, transform.right);
        }
        else
        {
            Vector2 directionToPlayer = player.transform.position - transform.position;

            entrySide = Vector2.Dot(directionToPlayer, transform.up);
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject != player) return;

        float exitSide;
        if (!isVertical)
        {

            Vector2 directionToPlayer = player.transform.position - transform.position;
            exitSide = Vector2.Dot(directionToPlayer, transform.right);
        }
        else
        {
            Vector2 directionToPlayer = player.transform.position - transform.position;
            exitSide = Vector2.Dot(directionToPlayer, transform.up);
        }


        if (entrySide * exitSide < 0)
        {
            ToggleMirrorWorld();
        }
    }

    void ToggleMirrorWorld()
    {
        PlayerController pc = player.GetComponent<PlayerController>();

        if (pc.inMirrorWorld)
        {
            player.transform.position = new Vector3(player.transform.position.x, player.transform.position.y, 0);
            cameraa.transform.position = new Vector3(0, 0, -10);
            cameraa.transform.localRotation = Quaternion.identity;
            pc.inMirrorWorld = false;
        }
        else
        {
            player.transform.position = new Vector3(player.transform.position.x, player.transform.position.y, 10);
            cameraa.transform.position = new Vector3(0, 0, 20);
            cameraa.transform.localRotation = Quaternion.Euler(0, 180, 0);
            pc.inMirrorWorld = true;
        }

    }
}
