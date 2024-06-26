using UnityEngine;

public class ParticleTestManager : MonoBehaviour
{
    public Transform player2;
    public FieldController fieldController;
    public GameObject particle;

    public int xOffset = 0;

    private void Awake()
    {
        fieldController = GetComponent<FieldController>();
    }

    private void Start()
    {
        MakeEffect();
    }

    public void MakeEffect()
    {
        Vector2 worldPos;

        fieldController.MovePlayerToCell(player2, fieldController.aiGridPosition, 1f, 0.3f);
        player2.rotation = Quaternion.Euler(0, 180f, 0);
        
        worldPos = fieldController.GetWorldPos(new Vector2(3f,1f), xOffset, 0.3f);
        Vector2 worldPos2 = fieldController.GetWorldPos(new Vector2(2f,1f), 1, 0.3f);
        //2.23 0.05 (암것도 없)
        //3.23 0.05(저격))
        Debug.Log(worldPos2);

        Instantiate(particle, worldPos, particle.transform.rotation);
    }
}