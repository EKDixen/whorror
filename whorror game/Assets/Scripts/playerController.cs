using UnityEngine;

public class PlayerController : MonoBehaviour
{
    #region Variables and Components
    [Header("Variables")]
    public float _playerMoveSpeed;

    [Header("Components")]
    [SerializeField] private Rigidbody2D _playerRB;
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
    }
}
