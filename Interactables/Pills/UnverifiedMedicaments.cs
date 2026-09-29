using System.Collections.Generic;
using UnityEngine;
public class UnverifiedMedicaments : Interactable
{
    [Header("Possible Stats")]
    public List<StatBonus> possibleStats;
    protected override void OnUse()
    {
        StatBonus randStat = GetRandomStat();
        StatManager.Apply(randStat);
    }
    private StatBonus GetRandomStat()
    {
        float totalWeight = 0;
        foreach(var stat in possibleStats)
        {
            totalWeight += stat.weight;
        }
        float randomPoint = Random.Range(0, totalWeight);
        foreach(var stat in possibleStats)
        {
            randomPoint -= stat.weight;
            if(randomPoint <= 0)
            {
                return stat;
            }
        }
        return possibleStats[0];
    }
}
