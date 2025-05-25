using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    /// <summary>
    /// Filtrage des objets à interagir avec
    /// </summary>
    [SerializeField] private LayerMask _interactableLayer;

    /// <summary>
    /// Transform pour définir la position et l'orientation de la visée
    /// </summary>
    [SerializeField] private Transform _aimTransform;

    private GameInputs _gameInputs;

    private MonoBehaviour _focusedObject;

    private bool _windowOpened;

    // Start is called before the first frame update
    void Start()
    {
        _gameInputs = new GameInputs();
        _gameInputs.Enable();

        _gameInputs.Player.Interact.performed += InteractPerformed;

        ApplicationController.Instance.OnOpenWindow += OnOpenWindow;
        ApplicationController.Instance.OnCloseWindow += OnCloseWindow;
    }

    private void OnCloseWindow()
    {
        _windowOpened = false;
    }

    private void OnOpenWindow()
    {
        _windowOpened = true;
    }

    private void InteractPerformed(InputAction.CallbackContext obj)
    {
        if (_windowOpened)
            return;

        if (_focusedObject == null)
            return;

        if (_focusedObject is SecretObject)
            ApplicationController.Instance.ShowNextSecret();

        if(_focusedObject is SecretWriterObject)
            ApplicationController.Instance.ShowSecretSender();
    }

    // Update is called once per frame
    void Update()
    {
        if(Physics.Raycast(_aimTransform.position, _aimTransform.forward, out RaycastHit hitInfo, 1, _interactableLayer))
        {
            if (hitInfo.collider.TryGetComponent(out SensorComponent sensorComponent))
            {
                _focusedObject = sensorComponent.GetSource();

                if (_focusedObject is SecretObject)
                {
                    ApplicationController.Instance.ShowOpenSecretText();
                }
                if (_focusedObject is SecretWriterObject)
                {
                    ApplicationController.Instance.ShowWriteSecretText();
                }
            }
            else
            {
                _focusedObject = null;
                ApplicationController.Instance.HideText();
            }
        }
        else
        {
            _focusedObject = null;
            ApplicationController.Instance.HideText();
        }
    }
}

