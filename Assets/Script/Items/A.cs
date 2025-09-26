using UnityEngine;

[CreateAssetMenu(menuName = "Items/Create A")]
public class CaptureBall : ItemBase
{
    public override bool Use()
    {
        return true;
    }

    public override bool CanUseOutsideBattle => false;
}
