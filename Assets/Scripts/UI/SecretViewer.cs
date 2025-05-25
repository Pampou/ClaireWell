using System.Collections;
using TMPro;
using UnityEngine;

public class SecretViewer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _secretText;

    /// <summary>
    /// Est-ce que la fenêtre est fermée ?
    /// </summary>
    private bool _isDisposed;

    public void SetText(string text)
    {
        _secretText.text = text;
    }

    /// <summary>
    /// Appelé depuis Unity - Fermer la fenêtre
    /// </summary>
    public void Close()
    {
        Destroy(gameObject);
    }

    /// <summary>
    /// Permet d'attendre avant que la fenêtre ne soit fermée
    /// </summary>
    public IEnumerator WaitForClose()
    {
        //Tant que la fenêtre est pas fermée
        while (!_isDisposed)
            yield return null; //On rend une frame
    }

    private void OnDestroy()
    {
        _isDisposed = true;
    }
}
