using UnityEngine;

public class ItemBase : ScriptableObject
{
    [SerializeField] string name;
    [SerializeField] string description;
    [SerializeField] string questDescription;
    [SerializeField] Sprite icon;
    [SerializeField] int price;
    [SerializeField] bool isSellable;

    public virtual string Name => name;
    public string Description => description;
    public string QuestDescription => questDescription;
    public Sprite Icon => icon;
    public int Price => price;
    public bool IsSellable => isSellable;

    public virtual bool Use()
    {
        return false;
    }
    public virtual bool IsReusable => false;
    public virtual bool CanUseInBattle => true;
    public virtual bool CanUseOutsideBattle => true;
}
