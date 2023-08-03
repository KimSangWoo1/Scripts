using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class ItemManager : Singleton<ItemManager>
{
    // Health 아이템 Pool Push
    internal void ItemHealthPush(GameObject _item,ObjectPooling.Item_HealthState _healthState)
    {        
        _item.SetActive(false);

        switch (_healthState)
        {
            case ObjectPooling.Item_HealthState.Red:
                item_RedHealthPooling.Item_Push(_item);
                break;
            case ObjectPooling.Item_HealthState.Yellow:
                item_YellowHealthPooling.Item_Push(_item);
                break;
            case ObjectPooling.Item_HealthState.Green:
                item_GreenHealthPooling.Item_Push(_item);
                break;
        }
    }

    // Health 아이템 Pool POP
    internal void ItemHealthPop(Transform _item, ObjectPooling.Item_HealthState _healthState)
    {
        GameObject item = null;
        switch (_healthState)
        {
            case ObjectPooling.Item_HealthState.Red:
                if (item_RedHealthPooling.Get_HealthState() != ObjectPooling.Item_HealthState.Red)
                {
                    item_RedHealthPooling.Set_HealthState(ObjectPooling.Item_State.Health,ObjectPooling.Item_HealthState.Red);
                }
                item = item_RedHealthPooling.Item_Pop();
                break;
            case ObjectPooling.Item_HealthState.Yellow:
                if (item_YellowHealthPooling.Get_HealthState() != ObjectPooling.Item_HealthState.Yellow)
                {
                    item_YellowHealthPooling.Set_HealthState(ObjectPooling.Item_State.Health, ObjectPooling.Item_HealthState.Yellow);
                }
                item = item_YellowHealthPooling.Item_Pop();
                break;
            case ObjectPooling.Item_HealthState.Green:
                if (item_GreenHealthPooling.Get_HealthState() != ObjectPooling.Item_HealthState.Green)
                {
                    item_GreenHealthPooling.Set_HealthState(ObjectPooling.Item_State.Health, ObjectPooling.Item_HealthState.Green);
                }
                item = item_GreenHealthPooling.Item_Pop();
                break;
        }

        item.transform.position = _item.position;
        item.transform.rotation = _item.rotation;
        item.SetActive(true);
    }
}
