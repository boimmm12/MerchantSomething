using System.Collections;
using UnityEngine;

public class NPCController : MonoBehaviour, Interactable
{
    [SerializeField] Dialog dialog;
    Customer cust;
    void Awake()
    {
        cust = GetComponent<Customer>();
    }
    public IEnumerator Interact(Transform initiator)
    {
        if (cust != null)
        {
            yield return cust.Trade();
        }
        else
        {
            yield return DialogManager.Instance.ShowDialog(dialog);
        }

    }
}
