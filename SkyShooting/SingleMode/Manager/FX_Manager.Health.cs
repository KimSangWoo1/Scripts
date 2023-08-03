using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class FX_Manager : Singleton<FX_Manager>
{
    //FX 돈 Pool Push
    internal void FX_HealthPush(GameObject FX_Health)
    {
        FX_Health.SetActive(false);
        FX_MoneyPool.FX_Push(FX_Health);
    }

    //FX 돈 Pool POP
    internal void FX_HealthPop(Transform EatObject)
    {
        if (FX_HealthPool.Get_FX_State() != ObjectPooling.FX_State.Health)
        {
            FX_HealthPool.SetFXState(ObjectPooling.FX_State.Health);
        }
        GameObject FX_Health = FX_HealthPool.FX_Pop();
        FX_Health.transform.position = EatObject.position;
        FX_Health.transform.rotation = EatObject.rotation;
        FX_Health.SetActive(true);
    }
}
