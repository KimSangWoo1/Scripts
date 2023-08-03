using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class FX_Manager : Singleton<FX_Manager>
{
    //FX 죽음 Pool Push
    internal void FX_Push(GameObject FX_Item, ObjectPooling.DeadState _deadState)
    {
        FX_Item.SetActive(false);

        switch (_deadState)
        {
            case ObjectPooling.DeadState.None:
                break;
            case ObjectPooling.DeadState.Red:
                FX_RedDeadPool.FX_Push(FX_Item);
                break;
            case ObjectPooling.DeadState.Green:
                FX_GreenDeadPool.FX_Push(FX_Item);
                break;
            case ObjectPooling.DeadState.Blue:
                FX_BlueDeadPool.FX_Push(FX_Item);
                break;
            case ObjectPooling.DeadState.Orange:
                FX_OrangeDeadPool.FX_Push(FX_Item);
                break;
        }
    }

    //FX 죽음 Pool POP
    internal void FX_Pop(Transform deadObject, ObjectPooling.DeadState _deadState)
    {
        GameObject FX_Dead = null;
        switch (_deadState)
        {
            case ObjectPooling.DeadState.None:
                break;
            case ObjectPooling.DeadState.Red:
                if (FX_RedDeadPool.Get_FX_State() != ObjectPooling.FX_State.Dead || FX_RedDeadPool.Get_FX_DeadState() != ObjectPooling.DeadState.Red)
                {
                    FX_RedDeadPool.Set_FX_DeadState(ObjectPooling.FX_State.item, ObjectPooling.DeadState.Red);
                }
                FX_Dead = FX_RedDeadPool.FX_Pop();
                break;
            case ObjectPooling.DeadState.Green:
                if (FX_RedDeadPool.Get_FX_State() != ObjectPooling.FX_State.Dead || FX_RedDeadPool.Get_FX_DeadState() != ObjectPooling.DeadState.Green)
                {
                    FX_RedDeadPool.Set_FX_DeadState(ObjectPooling.FX_State.item, ObjectPooling.DeadState.Green);
                }
                FX_Dead = FX_GreenDeadPool.FX_Pop();
                break;
            case ObjectPooling.DeadState.Blue:
                if (FX_RedDeadPool.Get_FX_State() != ObjectPooling.FX_State.Dead || FX_RedDeadPool.Get_FX_DeadState() != ObjectPooling.DeadState.Blue)
                {
                    FX_RedDeadPool.Set_FX_DeadState(ObjectPooling.FX_State.item, ObjectPooling.DeadState.Blue);
                }
                FX_Dead = FX_BlueDeadPool.FX_Pop();
                break;
            case ObjectPooling.DeadState.Orange:
                if (FX_RedDeadPool.Get_FX_State() != ObjectPooling.FX_State.Dead || FX_RedDeadPool.Get_FX_DeadState() != ObjectPooling.DeadState.Orange)
                {
                    FX_RedDeadPool.Set_FX_DeadState(ObjectPooling.FX_State.item, ObjectPooling.DeadState.Orange);
                }
                FX_Dead = FX_OrangeDeadPool.FX_Pop();
                break;
        }
        if (FX_Dead != null)
        {
            FX_Dead.transform.position = deadObject.position;
            FX_Dead.transform.rotation = Quaternion.Euler(-90f, 0f, 0f);
            FX_Dead.SetActive(true);
        }
    }
}
