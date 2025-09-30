using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum CustomerState { Idle, Walking, Buying }

public class Customer : MonoBehaviour
{
    [SerializeField] float timeBetweenPattern = 0.6f;
    [SerializeField] List<Vector2> movementPattern = new();
    [SerializeField] bool patternAsOffset = true;

    [SerializeField] float moveSpeed = 3f;
    [SerializeField] float arriveThreshold = 0.02f;

    [SerializeField] Vector2 buyIntervalRange = new Vector2(8f, 15f);
    [SerializeField] bool autoFindMarketsInScene = true;
    [SerializeField] List<Market> markets = new();

    [SerializeField] public GameObject exclamation;
    public bool IsWaitingToSell { get; private set; }
    public bool IsInSellingMenu { get; private set; }
    CustomerState state = CustomerState.Idle;
    float idleTimer;
    int currentPattern;
    Coroutine activeRoutine;

    float buyTimer;
    float nextBuyTime;

    Market targetMarket;

    void Awake()
    {
        ResetNextBuyTime();
    }

    void Start()
    {
        if (autoFindMarketsInScene)
            markets.AddRange(FindObjectsOfType<Market>());
    }

    void ResetNextBuyTime()
    {
        nextBuyTime = Random.Range(buyIntervalRange.x, buyIntervalRange.y);
        buyTimer = 0f;
    }

    public void BuyFrom(Market market)
    {
        if (market == null) return;

        if (activeRoutine != null) { StopCoroutine(activeRoutine); activeRoutine = null; }
        targetMarket = market;
        state = CustomerState.Buying;
        idleTimer = 0f;
    }

    void Update()
    {
        if (state == CustomerState.Idle)
        {
            if (IsWaitingToSell) return;
            buyTimer += Time.deltaTime;
            if (buyTimer >= nextBuyTime && markets.Count > 0)
            {
                var market = markets[Random.Range(0, markets.Count)];
                BuyFrom(market);
            }
        }

        switch (state)
        {
            case CustomerState.Idle:
                idleTimer += Time.deltaTime;
                if (idleTimer > timeBetweenPattern && movementPattern.Count > 0)
                {
                    idleTimer = 0f;
                    if (activeRoutine == null)
                        activeRoutine = StartCoroutine(WalkPatternStep());
                }
                break;

            case CustomerState.Walking:
                break;

            case CustomerState.Buying:
                if (activeRoutine == null && targetMarket != null)
                    activeRoutine = StartCoroutine(BuyRoutine(targetMarket));
                break;
        }
    }

    IEnumerator WalkPatternStep()
    {
        state = CustomerState.Walking;

        Vector3 from = transform.position;
        Vector3 target = patternAsOffset
            ? from + (Vector3)movementPattern[currentPattern]
            : new Vector3(movementPattern[currentPattern].x, movementPattern[currentPattern].y, from.z);

        yield return MoveTo(target);

        if ((transform.position - from).sqrMagnitude > arriveThreshold * arriveThreshold)
            currentPattern = (currentPattern + 1) % movementPattern.Count;

        state = CustomerState.Idle;
        activeRoutine = null;
    }

    IEnumerator BuyRoutine(Market market)
    {
        // 1) Jalan ke market
        Vector3 targetPos = market.GetEntrancePosition();
        yield return MoveTo(targetPos);

        if (market.TryReserveRandomAvailableItem(out var chosen))
        {
            if (!market.TrySetWaiting(this, chosen))
            {
                ResetAfterBuy(false);
                yield break;
            }
            IsWaitingToSell = true;
            Debug.Log($"{name} mengambil (reserve): {chosen.name}. Menunggu dijual…");

            float t = 0f, waitDuration = 3f;
            while (t < waitDuration)
            {
                exclamation.gameObject.SetActive(true);
                if (market.ConsumeSellFinished(this))
                {
                    Debug.Log($"{name}: transaksi selesai.");
                    break;
                }

                if (!IsInSellingMenu)
                    t += Time.deltaTime;

                yield return null;
            }

            market.ClearWaiting(this);
            IsWaitingToSell = false;
        }
        else
        {
            Debug.Log($"{name}: Market kosong.");
        }

        ResetAfterBuy(true);
    }

    void ResetAfterBuy(bool scheduleNext)
    {
        targetMarket = null;
        if (scheduleNext) ResetNextBuyTime();
        idleTimer = 0f;
        state = CustomerState.Idle;
        activeRoutine = null;
        exclamation.gameObject.SetActive(false);
        IsInSellingMenu = false;
    }

    IEnumerator MoveTo(Vector3 target)
    {
        while ((transform.position - target).sqrMagnitude > arriveThreshold * arriveThreshold)
        {
            transform.position = Vector3.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);
            yield return null;
        }
        transform.position = target;
    }

    public void OnSellingMenuOpened()
    {
        IsInSellingMenu = true;
        if (exclamation) exclamation.SetActive(true);
    }

    public void OnSellingMenuClosed()
    {
        IsInSellingMenu = false;
    }
}
