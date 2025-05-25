using UnityEngine;

/// <summary>
/// Utile pour r�pondre aux potentiels raycast et donner l'objet parent ma�tre qui nous int�resse
/// </summary>
public class SensorComponent : MonoBehaviour
{
    [SerializeField] private MonoBehaviour _source;
    
    public MonoBehaviour GetSource()
    {
        return _source;
    }
}
