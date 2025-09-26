using System;
using UnityEngine;

public class Condition
{
    public ConditionID Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string StartMessage { get; set; }
    public float TickInterval { get; set; }
    public Action<HealthComponent> OnStart;
    public Action<HealthComponent> OnTick;
    public Action<HealthComponent> OnEnd;
}
