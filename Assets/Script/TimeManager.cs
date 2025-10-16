using System;
using UnityEngine;
public enum TimeOfDay
{
    Morning, Noon, Afternoon, Night
}
public class TimeManager : MonoBehaviour
{
    private TimeOfDay[] sequence =
        new TimeOfDay[] { TimeOfDay.Morning, TimeOfDay.Noon, TimeOfDay.Afternoon, TimeOfDay.Night };

    private int index = 0;

    public event Action<TimeOfDay> OnTimeChanged;

    public static TimeManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(this);

        if (sequence == null || sequence.Length == 0)
            sequence = new TimeOfDay[] { TimeOfDay.Morning, TimeOfDay.Noon, TimeOfDay.Afternoon, TimeOfDay.Night };
    }

    /// <summary>Waktu saat ini dalam bentuk enum (pagi, siang, sore, malam).</summary>
    public TimeOfDay Current => sequence[Mathf.Clamp(index, 0, sequence.Length - 1)];

    /// <summary>Index saat ini dalam urutan sequence (0 = pagi, 1 = siang, dst).</summary>
    public int CurrentIndex => index;

    /// <summary>Jumlah total slot waktu (biasanya 4).</summary>
    public int TotalTimes => sequence.Length;

    /// <summary>Majukan waktu ke index berikutnya.</summary>
    public void AdvanceTime(int steps = 1)
    {
        index = (index + steps) % sequence.Length;
        OnTimeChanged?.Invoke(Current);
        Debug.Log($"[TimeManager] Time advanced → {Current} (index={index})");
    }

    /// <summary>Set waktu secara langsung menggunakan enum.</summary>
    public void SetTime(TimeOfDay t)
    {
        int i = Array.IndexOf(sequence, t);
        if (i >= 0)
        {
            index = i;
            OnTimeChanged?.Invoke(Current);
        }
    }

    public int GetIndex() => index;

    public void SetIndex(int i)
    {
        index = i % sequence.Length;
        OnTimeChanged?.Invoke(Current);
    }

    /// <summary>Apakah waktu sekarang adalah waktu tertentu.</summary>
    public bool Is(TimeOfDay time) => Current == time;

    /// <summary>Apakah waktu sekarang termasuk salah satu dari list waktu yang diberikan.</summary>
    public bool IsIn(params TimeOfDay[] times)
    {
        foreach (var t in times)
            if (t == Current) return true;
        return false;
    }

    /// <summary>Apakah aksi boleh dilakukan hanya pada waktu tertentu.</summary>
    public bool CanPerformAction(TimeOfDay allowedTime)
    {
        return Current == allowedTime;
    }

    /// <summary>Apakah aksi boleh dilakukan hanya pada beberapa waktu tertentu.</summary>
    public bool CanPerformAction(params TimeOfDay[] allowedTimes)
    {
        return IsIn(allowedTimes);
    }

    // 🔹 Debug GUI
    private void OnGUI()
    {
        var style = new GUIStyle();
        style.fontSize = 28;
        style.normal.textColor = Color.yellow;
        style.alignment = TextAnchor.UpperRight;

        GUI.Label(new Rect(Screen.width - 220, 20, 200, 40),
            $"Time: {Current} ({index})", style);
    }
}