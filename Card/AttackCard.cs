using System;
using System.Collections.Generic;
using UnityEngine;

public class AttackCard : Card
{
    protected override void Awake()
    {
        base.Awake();
    }

    public List<Vector2> GetGridPosList(Vector2 playerCoord) => cardData.AtkShape switch
    {
        //맵에 bool 2차원 배열을 해당 좌표들에 대해선 on시켜야할 
        AttackShape.Vertical => ExcludeOverRangeCoordList(playerCoord,
        new List<Vector2>
        {
            new Vector2(0, -1),
            new Vector2(0, 0),
            new Vector2(0, 1)
        }),
        AttackShape.Horizontal => ExcludeOverRangeCoordList(playerCoord,
        new List<Vector2>
        {
            new Vector2(-1, 0),
            new Vector2(0, 0),
            new Vector2(1, 0)
        }),
        AttackShape.Slash => ExcludeOverRangeCoordList(playerCoord,
        new List<Vector2>
        {
            new Vector2(-1, -1),
            new Vector2(0, 0),
            new Vector2(1, 1)
        }),
        AttackShape.backSlash => ExcludeOverRangeCoordList(playerCoord,
        new List<Vector2>
        {
            new Vector2(-1, 1),
            new Vector2(0, 0),
            new Vector2(1, -1)
        }),
        AttackShape.X => ExcludeOverRangeCoordList(playerCoord,
        new List<Vector2>
        {
            new Vector2(-1, 1),
            new Vector2(0, 0),
            new Vector2(1, -1),
            new Vector2(-1, -1),
            new Vector2(1, 1)
        }),
        AttackShape.Cross => ExcludeOverRangeCoordList(playerCoord,
        new List<Vector2>
        {
            new Vector2(-1, 0),
            new Vector2(0, 0),
            new Vector2(1, 0),
            new Vector2(0, -1),
            new Vector2(0, 1)
        }),
        AttackShape.TUp => ExcludeOverRangeCoordList(playerCoord,
        new List<Vector2>
        {
            new Vector2(-1, -1),
            new Vector2(0, -1),
            new Vector2(1, -1),
            new Vector2(0, 0),
            new Vector2(0, 1),
        }),
        AttackShape.TDown => ExcludeOverRangeCoordList(playerCoord,
        new List<Vector2>
        {
            new Vector2(-1, 1),
            new Vector2(0, 1),
            new Vector2(1, 1),
            new Vector2(0, 0),
            new Vector2(0, -1)
        }),
        AttackShape.H => ExcludeOverRangeCoordList(playerCoord,
        new List<Vector2>
        {
            new Vector2(-1, 1),
            new Vector2(-1, 0),
            new Vector2(-1, -1),
            new Vector2(0, 0),
            new Vector2(1, 1),
            new Vector2(1, 0),
            new Vector2(1, -1),
        }),
        AttackShape.LyingH => ExcludeOverRangeCoordList(playerCoord,
        new List<Vector2>
        {
            new Vector2(-1, 1),
            new Vector2(0, 1),
            new Vector2(1, 1),
            new Vector2(0, 0),
            new Vector2(-1, -1),
            new Vector2(0, -1),
            new Vector2(1, -1),
        }),
        _ => throw new ArgumentException("Unknown type of a Attack", nameof(cardData.AtkShape)),
    };

    public List<Vector2> ExcludeOverRangeCoordList(Vector2 playerCoord, List<Vector2> coordList)
    {
        List<Vector2> newCoordList = new List<Vector2>();
        Vector2 newCoord;

        for (int i = 0; i < coordList.Count; i++)
        {
            newCoord = playerCoord + coordList[i];
            if (newCoord.x < 0  || newCoord.x > 3 || newCoord.y < 0 || newCoord.y > 2) continue;
            else newCoordList.Add(newCoord);
        }

        return newCoordList;
    }
}
