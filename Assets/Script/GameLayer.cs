using UnityEngine;

public class GameLayer : MonoBehaviour
{
    [SerializeField] public LayerMask interactableLayer;
    public static GameLayer i { get; set; }
    private void Awake()
    {
        i = this;
    }

    public LayerMask InteractableLayer
    {
        get => interactableLayer;
    }
}
