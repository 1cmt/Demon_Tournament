using System;
using UnityEngine;

public class MoveCard : Card
{
    protected override void Awake()
    {
        base.Awake();
    }

    public void PrintMoveDir()
    {
        Debug.Log(cardData.MoveDir.ToString());
    }

    public Vector2 GetGridPosInfo(Vector2 playerCoord) => cardData.MoveDir switch
    {
        MoveDirection.Up => ExcludeOverRangeCoord(playerCoord, Vector2.up),
        MoveDirection.Down => ExcludeOverRangeCoord(playerCoord, Vector2.down),
        MoveDirection.Right => ExcludeOverRangeCoord(playerCoord, Vector2.right),
        MoveDirection.Left => ExcludeOverRangeCoord(playerCoord, Vector2.left),
        _ => throw new ArgumentException("Unknown type of a Direction", nameof(cardData.MoveDir)),
    };

    public Vector2 ExcludeOverRangeCoord(Vector2 playerCoord, Vector2 moveCoord)
    {
        Vector2 newCoord = playerCoord + moveCoord;
        //범위초과시 (-2,-2) 반환
        if(newCoord.x < 0 || newCoord.x > 3 || newCoord.y < 0 || newCoord.y > 2) newCoord = new Vector2(-2, -2);

        return newCoord;
    }
}