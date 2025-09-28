using System.Collections;
using UnityEngine;

public class Market : MonoBehaviour, Interactable
{
    public IEnumerator Interact(Transform initiator)
    {
        Debug.Log("Po");
        yield return null;
    }

}
