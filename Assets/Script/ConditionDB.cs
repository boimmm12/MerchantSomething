using System.Collections.Generic;
using UnityEngine;

public class ConditionsDB
{
    public static void Init()
    {
        foreach (var kvp in Conditions)
        {
            var conditionId = kvp.Key;
            var condition = kvp.Value;

            condition.Id = conditionId;
        }
    }

    public static Dictionary<ConditionID, Condition> Conditions { get; set; } = new Dictionary<ConditionID, Condition>()
    {
        {
            ConditionID.psn,
            new Condition
            {
                Name = "Poison",
                StartMessage = "has been poisoned",
                TickInterval = 1f,                            // 1x per detik
                OnStart = (hp) => Debug.Log($"{hp.name} poisoned!"),
                OnTick  = (hp) => hp.TakeDamage(5),           // <<--- 5 damage / detik
                OnEnd   = (hp) => Debug.Log($"{hp.name} poison ended")
            }
        },

    };
}
public enum ConditionID
{
    none, psn, brn, par, frz, slp,
    confusion, regeneration, leech
}
