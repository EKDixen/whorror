using UnityEngine;

public class Door : MonoBehaviour
{
    public GameObject player;
    public GameObject world;
    public GameObject mirrorWorld;

    public bool isVertical;

    private float entrySide;

    void Awake()
    {
        player = GameObject.Find("player");
        mirrorWorld = GameObject.Find("MirrorHouse");
        world = GameObject.Find("House");
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
            
            world.SetActive(false);
            mirrorWorld.SetActive(true);

            pc.inMirrorWorld = false;
        }
        else
        {
            world.SetActive(true);
            mirrorWorld.SetActive(false);

            pc.inMirrorWorld = true;
        }

    }
}
