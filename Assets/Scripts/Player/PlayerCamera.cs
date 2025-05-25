using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCamera : MonoBehaviour
{
    /// <summary>
    /// Référence du player
    /// </summary>
    [SerializeField] private Transform _player;

    /// <summary>
    /// Sensibilité de la souris
    /// </summary>
    [SerializeField] private Vector2 _sensivity = Vector2.one;

    /// <summary>
    /// Stocke l'input regard avec souris
    /// </summary>
    private Vector2 _lookInputAxis;

    /// <summary>
    /// Stocke dernière rotation du regard
    /// </summary>
    private Vector2 _lookRotation;

    /// <summary>
    /// Gestion des inputs
    /// </summary>
    private GameInputs _gameInputs;

    /// <summary>
    /// Est-ce qu'on peut regarder ?
    /// </summary>
    private bool _canLook;

    // Start is called before the first frame update
    void Start()
    {
        _canLook = true;

        _gameInputs = new GameInputs();
        _gameInputs.Enable();

        _gameInputs.Player.Look.performed += LookPerformed;
        _gameInputs.Player.Look.canceled += LookCanceled;

        ApplicationController.Instance.OnOpenWindow += OnOpenWindow;
        ApplicationController.Instance.OnCloseWindow += OnCloseWindow;
    }

    private void OnCloseWindow()
    {
        _canLook = true;
    }

    private void OnOpenWindow()
    {
        _canLook = false;
    }
    private void LookCanceled(InputAction.CallbackContext obj)
    {
        _lookInputAxis = Vector2.zero;
    }

    private void LookPerformed(InputAction.CallbackContext obj)
    {
        _lookInputAxis = obj.ReadValue<Vector2>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_canLook)
        {
            float mouseX = _lookInputAxis.x * Time.deltaTime * _sensivity.x;
            float mouseY = _lookInputAxis.y * Time.deltaTime * _sensivity.y;

            _lookRotation.y += mouseX;
            _lookRotation.x -= mouseY;
            _lookRotation.x = Mathf.Clamp(_lookRotation.x, -70f, 70f);

            transform.position = _player.position;
            transform.rotation = Quaternion.Euler(_lookRotation.x, _lookRotation.y, 0);
            _player.rotation = Quaternion.Euler(0, _lookRotation.y, 0);
        }
    }
}
