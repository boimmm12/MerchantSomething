using UnityEngine;

public class CustomerNegotiator : MonoBehaviour
{
    public CustomerType type = CustomerType.TawarMenawar;
    public CustomerArchetype overrideArchetype;

    int currentRound;
    float targetPrice;
    float lastCounter;
    System.Random rng;

    void Awake() => rng = new System.Random(Random.Range(int.MinValue, int.MaxValue));

    public void ResetSession() { currentRound = 0; targetPrice = 0; lastCounter = 0; }

    public BargainParams GetParams()
    {
        if (overrideArchetype) return overrideArchetype.ToParams();
        return BargainCatalog.Get(type);
    }

    public void StartNegotiation(float baseCost, float openingPrice)
    {
        var p = GetParams();
        float tMin = p.targetPriceMultiplierRange.x;
        float tMax = p.targetPriceMultiplierRange.y;
        float mu = (tMin + tMax) * 0.5f;
        float span = (tMax - tMin) * 0.5f;
        float chosenMult = Mathf.Clamp(mu + NextGaussian() * 0.35f * span, tMin, tMax);

        targetPrice = baseCost * chosenMult;
        currentRound = 0;
        lastCounter = Mathf.Min(openingPrice, Mathf.Lerp(openingPrice, targetPrice, 0.5f));

        if (openingPrice > baseCost * (1f + p.hardRejectPct))
            lastCounter = -1f;
    }

    public float GetCounterOffer(float baseCost, float sellerOffer)
    {
        var p = GetParams();
        if (currentRound >= p.maxRounds) return -1f;

        float acceptThreshold = baseCost * (1f + p.acceptProfitPct);
        if (sellerOffer <= acceptThreshold && sellerOffer <= targetPrice * 1.05f)
        { lastCounter = sellerOffer; return lastCounter; }

        float move = Mathf.Lerp(lastCounter, targetPrice, p.moveRate);
        move *= 1f + ((float)(rng.NextDouble()*2-1) * p.randomNoise);

        float counter = Mathf.Min(move, sellerOffer - Mathf.Max(1f, sellerOffer * 0.005f));
        counter = Mathf.Max(counter, baseCost * 0.1f);

        currentRound++;
        lastCounter = counter;

        if (sellerOffer > baseCost * (1f + p.hardRejectPct)) return -1f;
        return counter;
    }

    private float NextGaussian()
    {
        double u1 = 1.0 - rng.NextDouble();
        double u2 = 1.0 - rng.NextDouble();
        return (float)(System.Math.Sqrt(-2.0 * System.Math.Log(u1)) * System.Math.Sin(2.0 * System.Math.PI * u2));
    }
}
