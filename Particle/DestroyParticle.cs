using UnityEngine;

public class DestroyParticle : MonoBehaviour
{
    [SerializeField] private float destroyTime = 3f;

    void Start()
    {
        Destroy(gameObject, destroyTime);   
    }
}
