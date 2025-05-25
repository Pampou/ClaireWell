using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class SecretSender : MonoBehaviour
{
    [SerializeField] private TMP_InputField _secretInputField;

    /// <summary>
    /// Est-ce que la fen�tre est ferm�e ?
    /// </summary>
    private bool _isDisposed;

    /// <summary>
    /// Est-ce qu'un envoi d'un secret a r�ussi ?
    /// </summary>
    private bool _sendSuccess;

    /// <summary>
    /// Appel� depuis Unity - Envoyer le secret
    /// </summary>
    public void SendSecret()
    {
        string secret = _secretInputField.text.Trim();

        //Si c'est vide, on ne va pas plus loin
        if (string.IsNullOrWhiteSpace(secret))
            return;

        StartCoroutine(SendSecretCoroutine(secret));
    }

    /// <summary>
    /// Coroutine d'envoi du secret
    /// </summary>
    private IEnumerator SendSecretCoroutine(string secret)
    {
        string jsonBody = $"{{\"device_id\":\"{ApplicationController.Instance.DeviceId}\",\"secret\":\"{secret}\"}}";
        var request = new UnityWebRequest(ApplicationController.Instance.ServerUrl + ApplicationController.Instance.TableName, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonBody);

        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("apikey", ApplicationController.Instance.ServerApiKey);
        request.SetRequestHeader("Authorization", $"Bearer {ApplicationController.Instance.ServerApiKey}");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success || request.responseCode == 201)
        {
            Debug.LogError("success");
        }
        else if (request.responseCode == 409)
        {
            Debug.LogError("Une phrase a d�j� �t� soumise depuis cet appareil.");
        }
        else
        {
            Debug.LogError($"Erreur : {request.responseCode}\n{request.downloadHandler.text}");
        }

        Destroy(gameObject);
    }

    /// <summary>
    /// Appel� depuis Unity - Annuler et fermer la fen�tre
    /// </summary>
    public void Cancel()
    {
        Destroy(gameObject);
    }

    public IEnumerator WaitForClose()
    {
        //Tant que la fen�tre est pas ferm�e
        while (!_isDisposed)
            yield return null; //On rend une frame
    }

    private void OnDestroy()
    {
        _isDisposed = true;
    }
}
