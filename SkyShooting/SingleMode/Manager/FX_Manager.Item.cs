using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class FX_Manager : Singleton<FX_Manager>
{
    //FX 아이템 Pool Push
    internal void FX_ItemPush(GameObject FX_Item)
    {
        FX_Item.SetActive(false);
        FX_ItemPool.FX_Push(FX_Item);
    }

    //FX 아이템 Pool POP
    internal void FX_ItemPop(Transform EatObject)
    {
        if (FX_ItemPool.Get_FX_State() != ObjectPooling.FX_State.item)
        {
            FX_ItemPool.SetFXState(ObjectPooling.FX_State.item);
        }

        GameObject FX_Item = FX_ItemPool.FX_Pop();
        FX_Item.transform.position = EatObject.position;
        FX_Item.transform.rotation = EatObject.rotation;
        FX_Item.SetActive(true);
    }
}
