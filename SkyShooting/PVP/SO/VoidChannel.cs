using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class VoidChannel : DescriptSO
{
    public UnityAction Requested;

    public void Event()
    {
        if (Requested != null)
        {
            Requested.Invoke();
        }
    }
}
