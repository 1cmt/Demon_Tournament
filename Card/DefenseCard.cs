using UnityEngine;

public class DefenseCard : Card
{
    protected override void Awake()
    {
        base.Awake();
    }
    
    public int GetReducedDamage()
    {
        return -cardData.Damage;
    }
}