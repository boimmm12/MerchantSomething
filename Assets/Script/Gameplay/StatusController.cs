using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusController : MonoBehaviour
{
    [System.Serializable]
    private class ActiveEffect
    {
        public ConditionID id;
        public float remaining;                 // durasi sisa (detik). <0 = tanpa durasi (infinite)
        public Coroutine routine;
    }

    [SerializeField] private List<ActiveEffect> active = new();
    private HealthComponent hp;
    void Awake() => hp = GetComponent<HealthComponent>();

    public void Apply(ConditionID id, float durationSeconds = 5f)
    {
        // kalau sudah ada, refresh durasi
        var cond = active.Find(a => a.id.Equals(id));
        if (cond != null)
        {
            cond.remaining = durationSeconds;
            return;
        }

        var data = ConditionsDB.Conditions[id];
        var a = new ActiveEffect { id = id, remaining = durationSeconds };
        active.Add(a);

        data.OnStart?.Invoke(hp);
        a.routine = StartCoroutine(TickRoutine(a, data));
    }

    public void Clear(ConditionID id)
    {
        var a = active.Find(x => x.id.Equals(id));
        if (a == null) return;
        var data = ConditionsDB.Conditions[id];
        if (a.routine != null) StopCoroutine(a.routine);
        data.OnEnd?.Invoke(hp);
        active.Remove(a);
    }

    private IEnumerator TickRoutine(ActiveEffect a, Condition data)
    {
        var wait = new WaitForSeconds(data.TickInterval);
        while (a.remaining > 0f || a.remaining < 0f) // <0 artinya infinite
        {
            yield return wait;
            data.OnTick?.Invoke(hp);
            if (a.remaining > 0f) a.remaining -= data.TickInterval;
        }
        Clear(a.id);
    }
}
