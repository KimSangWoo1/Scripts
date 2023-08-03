using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class FX_Manager : Singleton<FX_Manager>
{
    //FX 돈 Pool Push
    internal void FX_MoneyPush(GameObject FX_Money)
    {
        FX_Money.SetActive(false);
        FX_MoneyPool.FX_Push(FX_Money);
    }

    //FX 돈 Pool POP
    internal void FX_MoneyPop(Transform EatObject)
    {
        if (FX_MoneyPool.Get_FX_State() != ObjectPooling.FX_State.Money)
        {
            FX_MoneyPool.SetFXState(ObjectPooling.FX_State.Money);
        }
        GameObject FX_Money = FX_MoneyPool.FX_Pop();
        FX_Money.transform.position = EatObject.position;
        FX_Money.transform.rotation = EatObject.rotation;
        FX_Money.SetActive(true);
    }
}
