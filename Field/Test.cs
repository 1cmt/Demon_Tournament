using UnityEngine;

public class Test : MonoBehaviour
{
    public FieldController fieldController;

    private void Awake()
    {
        fieldController = GetComponent<FieldController>();
    }

    private void Start()
    {
        ChangeColor(new Vector2(1,2));
    }

    public void ChangeColor(Vector2 coord)
    {
        Cell cell = fieldController.cells[fieldController.columnsCount * (int)coord.y + (int)coord.x].gameObject.GetComponent<Cell>();
        cell.ChangeColor(Color.red);
    }
}