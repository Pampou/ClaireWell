using UnityEngine;

/// <summary>
/// Utile pour répondre aux potentiels raycast et donner l'objet parent maître qui nous intéresse
/// </summary>
public class SensorComponent : MonoBehaviour
{
    [SerializeField] private MonoBehaviour _source;
    
    public MonoBehaviour GetSource()
    {
        return _source;
    }
}
