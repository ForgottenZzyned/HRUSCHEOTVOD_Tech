using UnityEngine;
public class StatUpgrade : Interactable
{
    public StatBonus bonus;
    protected override void OnUse()
    {
        StatManager.Apply(bonus);
        if (bonus.stat == StatType.MaxHealth || bonus.stat == StatType.MaxStamina)
        {
            FOVManager.Instance.TriggerFOV(4f, 100);
        }
    }
}
