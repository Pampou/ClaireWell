using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class ApplicationController : MonoBehaviour
{
    /// <summary>
    /// Ev�nement � d�clencher lorsqu'une fen�tre est ouverte
    /// </summary>
    public event System.Action OnOpenWindow;

    /// <summary>
    /// Ev�nement � d�clencher lorsqu'une fen�tre est ferm�e
    /// </summary>
    public event System.Action OnCloseWindow;

    public static ApplicationController Instance;

    /// <summary>
    /// Identifiant unique par machine
    /// </summary>
    public string DeviceId
    {
        get
        {
            if (PlayerPrefs.HasKey("device_id"))
                return PlayerPrefs.GetString("device_id");

            string deviceId = System.Guid.NewGuid().ToString();

            PlayerPrefs.SetString("device_id", deviceId);
            PlayerPrefs.Save();

            return deviceId;
        }
    }

    /// <summary>
    /// Canvas pour accueillir les fen�tres
    /// </summary>
    [SerializeField] private Transform _canvas;

    /// <summary>
    /// Intervalle de temps en secondes minimal et maximal
    /// </summary>
    [SerializeField] private Vector2 _nextSecretInterval;

    /// <summary>
    /// Texte au centre
    /// </summary>
    [SerializeField] private TextMeshProUGUI _crosshairText;

    /// <summary>
    /// Url du serveur de base de donn�es
    /// </summary>
    public string ServerUrl;

    /// <summary>
    /// Nom de la table de la base de donn�es
    /// </summary>
    public string TableName;

    /// <summary>
    /// Cl� d'API
    /// </summary>
    public string ServerApiKey;

    /// <summary>
    /// Liste des secrets
    /// </summary>
    private List<Secret> _allSecrets;

    /// <summary>
    /// Prochain secret
    /// </summary>
    private Secret _nextSecret;

    /// <summary>
    /// Secret gameobject
    /// </summary>
    private GameObject _secretObject;

    /// <summary>
    /// Timer pour savoir quand r�v�ler le prochain secret
    /// </summary>
    private float _nextSecretTimer;

    /// <summary>
    /// Dur�e avant la r�v�lation du prochain secret
    /// </summary>
    private float _nextSecretRevealInSeconds;

    /// <summary>
    /// Est-ce que le timer tourne ?
    /// </summary>
    private bool _canContinueTimer;

    private void Start()
    {
        Instance = this;
        StartCoroutine(GetAllSecretsCoroutine());
        RerollNextSecretCooldown();
    }

    /// <summary>
    /// Red�finit la prochaine dur�e avant de r�cup�rer un secret
    /// </summary>
    private void RerollNextSecretCooldown()
    {
        _nextSecretRevealInSeconds = Random.Range(_nextSecretInterval.x, _nextSecretInterval.y);
        _nextSecretTimer = 0;
        _canContinueTimer = true;
    }

    private void Update()
    {
        if (_canContinueTimer)
        {
            _nextSecretTimer += Time.deltaTime;

            if (_nextSecretTimer > _nextSecretRevealInSeconds)
            {
                GetNextSecret();
            }
        }
    }

    /// <summary>
    /// D�finit le prochain secret � ouvrir
    /// </summary>
    public void GetNextSecret()
    {
        StartCoroutine(GetNextSecretCoroutine());
    }

    /// <summary>
    /// R�cup�re tous les secrets
    /// </summary>
    private IEnumerator GetAllSecretsCoroutine()
    {
        string url = $"{ServerUrl}{TableName}";
        UnityWebRequest request = UnityWebRequest.Get(url);

        request.SetRequestHeader("apikey", ServerApiKey);
        request.SetRequestHeader("Authorization", $"Bearer {ServerApiKey}");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string json = request.downloadHandler.text;
            _allSecrets = JsonUtilityWrapper.FromJsonList<Secret>(json);
        }
    }

    /// <summary>
    /// Coroutine pour d�finir le prochain secret
    /// </summary>
    private IEnumerator GetNextSecretCoroutine()
    {
        _canContinueTimer = false;

        yield return GetAllSecretsCoroutine();

        if (_allSecrets.Any())
        {
            _nextSecret = _allSecrets[Random.Range(0, _allSecrets.Count)];
            SpawnNextSecretObject();
        }
        else
        {
            RerollNextSecretCooldown();
        }
    }

    /// <summary>
    /// Instantie un secret objet sur le bord du puits
    /// </summary>
    private void SpawnNextSecretObject()
    {
        Vector2 position2D = Random.insideUnitCircle;
        //Le fait de prendre la version normalis�e nous assure que le point est sur le cercle
        position2D = position2D.normalized;
        position2D *= 1.43f; //Correspond au rayon du puits

        GameObject newSecretObj = Instantiate(
            Resources.Load("SecretObject"),
            new Vector3(position2D.x, 0.7f, position2D.y),
            Quaternion.Euler(0, Random.Range(0f, 360f), 0)
            ) as GameObject;

        _secretObject = newSecretObj;
    }

    /// <summary>
    /// Affiche le prochain secret
    /// </summary>
    public void ShowNextSecret()
    {
        StartCoroutine(ShowNextSecretCoroutine());
    }

    /// <summary>
    /// Coroutine pour afficher le prochain secret
    /// </summary>
    private IEnumerator ShowNextSecretCoroutine()
    {
        OnOpenWindow?.Invoke();
        GameObject secretWindowObj = Instantiate(Resources.Load("SecretWindow"), _canvas) as GameObject;
        SecretViewer secretViewer = secretWindowObj.GetComponent<SecretViewer>();
        secretViewer.SetText(_nextSecret.secret);

        yield return secretViewer.WaitForClose();

        OnCloseWindow?.Invoke();
        Destroy(_secretObject);
        _secretObject = null;
        RerollNextSecretCooldown();
    }

    /// <summary>
    /// Affiche la fen�tre pour r�diger un secret
    /// </summary>
    public void ShowSecretSender()
    {
        //Mais si on a d�j� r�dig� un secret, on sort
        if (_allSecrets.Any(k => k.device_id == DeviceId))
            return;
        StartCoroutine(ShowSecretSenderCoroutine());
    }

    /// <summary>
    /// Coroutine pour afficher le prochain secret
    /// </summary>
    private IEnumerator ShowSecretSenderCoroutine()
    {
        OnOpenWindow?.Invoke();
        GameObject secretWindowObj = Instantiate(Resources.Load("SecretSenderWindow"), _canvas) as GameObject;
        SecretSender secretSender = secretWindowObj.GetComponent<SecretSender>();

        yield return secretSender.WaitForClose();

        OnCloseWindow?.Invoke();
    }

    /// <summary>
    /// Affichage du texte lorsque l'on vise l'outil pour �crire un secret
    /// </summary>
    public void ShowWriteSecretText()
    {
        if (_allSecrets.Any(k => k.device_id == DeviceId))
            _crosshairText.text = "Vous avez d�j� avou� un secret";
        else
            _crosshairText.text = "Ecrire un secret";
    }

    /// <summary>
    /// Affichage du texte lorsque l'on vise un secret � ouvrir
    /// </summary>
    public void ShowOpenSecretText()
    {
        _crosshairText.text = "Lire le secret";
    }

    /// <summary>
    /// Reset le texte � rien
    /// </summary>
    public void HideText()
    {
        _crosshairText.text = "";
    }
}
