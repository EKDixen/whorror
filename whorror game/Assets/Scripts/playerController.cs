using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    #region Variables and Components
    [Header("Variables")]
    public float _playerMoveSpeed;
    public float rotationSpeed;
    public KeyCode _flashlight;

    [Header("Components")]
    [SerializeField] private Rigidbody2D _playerRB;
    [SerializeField] private GameObject _light;
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

        // Create the target rotation (only around Z)
        Quaternion targetRotation = Quaternion.Euler(0f, 0f, angle-90);

        // Smoothly interpolate toward target rotation
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        if(Input.GetKeyDown(_flashlight))
        {
            if(_light.active)
            {
                _light.SetActive(false);
            } else
            {
                _light.SetActive(true);
            }
        }

    }
}
