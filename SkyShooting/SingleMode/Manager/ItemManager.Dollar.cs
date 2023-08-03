using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class ItemManager : Singleton<ItemManager>
{
    // Health 아이템 Pool Push
    internal void ItemDollarPush(GameObject _item,ObjectPooling.Item_DollarState _dollarState)
    {
        _item.SetActive(false);
        switch (_dollarState)
        {
            case ObjectPooling.Item_DollarState.Red:
                item_RedDollarPooling.Item_Push(_item);
                break;
            case ObjectPooling.Item_DollarState.Yellow:
                item_YellowDollarPooling.Item_Push(_item);
                break;
            case ObjectPooling.Item_DollarState.Green:
                item_GreenDollarPooling.Item_Push(_item);
                break;
        }
    }

    // Health 아이템 Pool POP
    internal void ItemDollarPop(Transform _item, ObjectPooling.Item_DollarState _dollarState)
    {
        GameObject item = null;
        switch (_dollarState)
        {
            case ObjectPooling.Item_DollarState.Red:
                if (item_RedDollarPooling.Get_DollarState() != ObjectPooling.Item_DollarState.Red)
                {
                    item_RedDollarPooling.Set_DollarState(ObjectPooling.Item_State.Dollar,ObjectPooling.Item_DollarState.Red);
                }
                item = item_RedDollarPooling.Item_Pop();
                break;
            case ObjectPooling.Item_DollarState.Yellow:
                if (item_YellowDollarPooling.Get_DollarState() != ObjectPooling.Item_DollarState.Yellow)
                {
                    item_YellowDollarPooling.Set_DollarState(ObjectPooling.Item_State.Dollar, ObjectPooling.Item_DollarState.Yellow);
                }
                item = item_YellowDollarPooling.Item_Pop();
                break;
            case ObjectPooling.Item_DollarState.Green:
                if (item_GreenDollarPooling.Get_DollarState() != ObjectPooling.Item_DollarState.Green)
                {
                    item_GreenDollarPooling.Set_DollarState(ObjectPooling.Item_State.Dollar, ObjectPooling.Item_DollarState.Green);
                }
                item = item_GreenDollarPooling.Item_Pop();
                break;
        }

        item.transform.position = _item.position;
        item.transform.rotation = _item.rotation;
        item.SetActive(true);
    }
}
