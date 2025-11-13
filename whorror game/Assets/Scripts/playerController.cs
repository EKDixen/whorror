using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    #region Variables and Components
    [Header("Variables")]
    public float _playerMoveSpeed;

    [Header("Components")]
    [SerializeField] private Rigidbody2D _playerRB;
    public Camera cam;

    [Header("MonsterStuff")]
    public bool closeToWall;
    public bool onFloor;

    #endregion
    private void Start()
    {
        _playerRB = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        float _speedX, _speedY;
        _speedX = Input.GetAxisRaw("Horizontal");
        _speedY = Input.GetAxisRaw("Vertical");

        _playerRB.linearVelocity = new Vector2(_speedX, _speedY).normalized * _playerMoveSpeed;


        Vector3 mousePos = Input.mousePosition;
        mousePos = cam.ScreenToWorldPoint(mousePos);

        Vector3 direction = mousePos - transform.position;
        direction.z = 0f; 

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(0f, 0f, angle-90);

    }
}
