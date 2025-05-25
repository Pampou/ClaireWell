using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    /// <summary>
    /// Vitesse de déplacement
    /// </summary>
    [SerializeField] private float _movementSpeed = 8f;

    /// <summary>
    /// Gestion des inputs
    /// </summary>
    private GameInputs _gameInputs;

    /// <summary>
    /// Stocke l'input déplacement
    /// </summary>
    private Vector2 _movementInputAxis;

    /// <summary>
    /// Rigidbody
    /// </summary>
    private Rigidbody _rigidBody;

    /// <summary>
    /// Peux-on bouger ?
    /// </summary>
    private bool _canMove;

    /// <summary>
    /// Stocke dernière velocité
    /// </summary>
    private Vector3 _velocity;


    // Start is called before the first frame update
    void Start()
    {
        _rigidBody = GetComponent<Rigidbody>();
        _canMove = true;

        _gameInputs = new GameInputs();
        _gameInputs.Enable();

        _gameInputs.Player.Move.performed += MovePerformed;
        _gameInputs.Player.Move.canceled += MoveCanceled;

        ApplicationController.Instance.OnOpenWindow += OnOpenWindow;
        ApplicationController.Instance.OnCloseWindow += OnCloseWindow;
    }

    private void OnCloseWindow()
    {
        _canMove = true;
    }

    private void OnOpenWindow()
    {
        _canMove= false;
    }

    private void MoveCanceled(InputAction.CallbackContext obj)
    {
        _movementInputAxis = Vector2.zero;
    }

    private void MovePerformed(InputAction.CallbackContext obj)
    {
        _movementInputAxis = obj.ReadValue<Vector2>();
    }

    void Update()
    {
        if (_canMove)
        {
            _velocity = _rigidBody.velocity;

            Vector3 move = new Vector3(_movementInputAxis.x, 0, _movementInputAxis.y);
            move = transform.forward * move.z + transform.right * move.x;

            _velocity.x = move.x * _movementSpeed;
            _velocity.z = move.z * _movementSpeed;

            _velocity.y = 0;
        }
        else
        {
            _velocity = Vector3.zero;
        }

        _rigidBody.velocity = _velocity;
        _rigidBody.angularVelocity = Vector3.zero;
    }
}
