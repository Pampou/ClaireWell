using System.Collections;
using TMPro;
using UnityEngine;

public class SecretViewer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _secretText;

    /// <summary>
    /// Est-ce que la fen�tre est ferm�e ?
    /// </summary>
    private bool _isDisposed;

    public void SetText(string text)
    {
        _secretText.text = text;
    }

    /// <summary>
    /// Appel� depuis Unity - Fermer la fen�tre
    /// </summary>
    public void Close()
    {
        Destroy(gameObject);
    }

    /// <summary>
    /// Permet d'attendre avant que la fen�tre ne soit ferm�e
    /// </summary>
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
