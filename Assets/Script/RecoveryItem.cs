using UnityEngine;

[CreateAssetMenu(menuName = "Items/Create new Recovery Item")]
public class RecoveryItem : ItemBase
{
    [Header("HP")]
    [SerializeField] public int hpAmount;
    [SerializeField] public bool restoreMaxHP;

    // [Header("PP")]
    // [SerializeField] public int ppAmount;
    // [SerializeField] public bool restoreMaxPP;

    [Header("Status Condition")]
    [SerializeField] public ConditionID status;
    [SerializeField] public bool recoverAllStatus;

    public override bool Use(PlayerController player)
    {
        var hp = player.GetComponent<HealthComponent>();

        //heal
        if (restoreMaxHP || hpAmount > 0)
        {
            if (hp.health == hp.maxHealth)
                return false;

            if (restoreMaxHP)
                hp.IncreaseHP(hp.maxHealth);
            else
                hp.IncreaseHP(hpAmount);
        }

        // //status
        // if (recoverAllStatus || status != ConditionID.none)
        // {
        //     if (pokemon.Status == null && pokemon.VolatileStatus == null)
        //         return false;

        //     if (recoverAllStatus)
        //     {
        //         pokemon.CureStatus();
        //         pokemon.CureVolatileStatus();
        //     }
        //     else
        //     {
        //         if (pokemon.Status.Id == status)
        //             pokemon.CureStatus();
        //         else if (pokemon.VolatileStatus.Id == status)
        //             pokemon.CureVolatileStatus();
        //         else
        //             return false;
        //     }
        // }

        // //restore PP
        // if (restoreMaxPP)
        // {
        //     pokemon.Moves.ForEach(m => m.IncreasePP(m.Base.PP));
        // }
        // else if (ppAmount > 0)
        // {
        //     pokemon.Moves.ForEach(m => m.IncreasePP(ppAmount));
        // }
        return true;
    }
}
