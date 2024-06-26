using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DefaultCharacter",menuName = "Characters/DefaultCharacter")]
public class CharacterSO : ScriptableObject
{
    [field: SerializeField] public string Name;
    //[field: SerializeField] public bool IsLeft = true;
    [field: SerializeField] public bool IsPlayerSelected = false;
    //[field: SerializeField] private List<Card> Cards;
}
