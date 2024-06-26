using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldController : MonoBehaviour
{
    public Transform[] cells;

    private Vector2[,] gridPositions;

    public Vector2 playerGridPosition = new Vector2(0, 1);
    public Vector2 aiGridPosition = new Vector2(3, 1);

    public int rowsCount = 3;
    public int columnsCount = 4;

    void Awake()
    {
        gridPositions = new Vector2[columnsCount, rowsCount];

        for (int i = 0; i < cells.Length; i++)
        {
            //Debug.Log(cells[i].position + " i");
            int row = i / columnsCount;
            int column = i % columnsCount;
            gridPositions[column, row] = cells[i].position;
        }

        Debug.Log("플레이어 초기 " + gridPositions[(int)playerGridPosition.x, (int)playerGridPosition.y]);
        Debug.Log("상대방 초기 " + gridPositions[(int)aiGridPosition.x, (int)aiGridPosition.y]);

        //플레이어와 상대방의 초기 위치를 설정
        //MovePlayerToCell(player.transform, playerPosition,GameManager.Instance.Xoffset,GameManager.Instance.Yoffset);
        //MovePlayerToCell(enemy.transform, aiPosition,GameManager.Instance._Xoffset,GameManager.Instance.Yoffset);
        //MovePlayerToCell(player, playerPosition,GameManager.Instance.Xoffset,GameManager.Instance.Yoffset);
        //MovePlayerToCell(enemy, aiPosition,GameManager.Instance._Xoffset,GameManager.Instance.Yoffset);
    }

    public void MovePlayerToCell(Transform character, Vector2 position, float Xoffset, float Yoffset)
    {
        // 캐릭터 위치 설정
        //Debug.Log(character.position);
        character.position = gridPositions[(int)position.x, (int)position.y] + new Vector2(Xoffset, Yoffset);
        //Debug.Log(character.position);
    }

    public Vector2 GetWorldPos(Vector2 position, float Xoffset, float Yoffset)
    {
        return gridPositions[(int)position.x, (int)position.y] + new Vector2(Xoffset, Yoffset);
    }

    public void ChangeCellColor(Vector2 coord, Color color)
    {
        Cell cell = cells[columnsCount * (int)coord.y + (int)coord.x].gameObject.GetComponent<Cell>();
        cell.ChangeColor(color);
    }
}
