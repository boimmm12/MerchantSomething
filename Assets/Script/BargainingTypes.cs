using UnityEngine;

[System.Serializable]
public struct BargainParams
{
    public Vector2 targetPriceMultiplierRange;
    [Range(0f, 1f)] public float moveRate;
    [Range(0f, 0.5f)] public float randomNoise;
    public int maxRounds;
    public float acceptProfitPct;
    public float hardRejectPct;

    public BargainParams(Vector2 r, float m, float n, int rounds, float acc, float rej)
    {
        targetPriceMultiplierRange = r; moveRate = m; randomNoise = n;
        maxRounds = rounds; acceptProfitPct = acc; hardRejectPct = rej;
    }
}

public static class BargainCatalog
{
    public static BargainParams Get(CustomerType t)
    {
        switch (t)
        {
            case CustomerType.Pelit:        return new BargainParams(new Vector2(0.75f,0.95f),0.25f,0.05f,6,0.05f,0.50f);
            case CustomerType.TawarMenawar: return new BargainParams(new Vector2(0.90f,1.05f),0.40f,0.05f,5,0.10f,0.65f);
            case CustomerType.Normal:       return new BargainParams(new Vector2(1.00f,1.15f),0.60f,0.03f,4,0.12f,0.80f);
            case CustomerType.Dermawan:     return new BargainParams(new Vector2(1.05f,1.25f),0.75f,0.02f,3,0.15f,1.00f);
            case CustomerType.TidakSabar:   return new BargainParams(new Vector2(0.95f,1.05f),0.70f,0.04f,2,0.07f,0.40f);
            default:                        return new BargainParams(new Vector2(0.90f,1.10f),0.50f,0.05f,4,0.10f,0.70f);
        }
    }
}
