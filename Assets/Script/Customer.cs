using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class Customer : MonoBehaviour
{
    public IEnumerator Trade()
    {
        print("po");
        yield return GameController.Instance.StateMachine.PushAndWait(SellingState.i);


    }
}
