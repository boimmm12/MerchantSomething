using UnityEngine;

[CreateAssetMenu(menuName = "Items/Create A")]
public class A : ItemBase
{
    public override bool Use(PlayerController player)
    {
        return true;
    }

    public override bool CanUseOutsideBattle => false;
}
