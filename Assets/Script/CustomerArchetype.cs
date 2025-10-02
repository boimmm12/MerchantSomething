using UnityEngine;

[CreateAssetMenu(fileName = "CustomerArchetype", menuName = "Negotiation/Customer Archetype")]
public class CustomerArchetype : ScriptableObject
{
    public CustomerType type;
    public Vector2 targetPriceMultiplierRange = new Vector2(0.9f, 1.1f);
    [Range(0f,1f)] public float moveRate = 0.5f;
    [Range(0f,0.5f)] public float randomNoise = 0.05f;
    public int maxRounds = 4;
    public float acceptProfitPct = 0.1f;
    public float hardRejectPct = 0.7f;

    public BargainParams ToParams() =>
        new BargainParams(targetPriceMultiplierRange, moveRate, randomNoise, maxRounds, acceptProfitPct, hardRejectPct);
}
