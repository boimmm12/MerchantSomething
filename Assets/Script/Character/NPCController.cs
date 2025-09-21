using System.Collections;
using UnityEngine;

public class NPCController : MonoBehaviour, Interactable
{
    [SerializeField] Dialog dialog;

    public IEnumerator Interact(Transform initiator)
    {
        yield return DialogManager.Instance.ShowDialog(dialog);
    }
}
