using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class ItemManager : Singleton<ItemManager>
{
    // Upgrade 아이템 Pool Push
    internal void ItemUpgradePush(GameObject _item, ObjectPooling.Item_State _itemState)
    {        
        _item.SetActive(false);

        switch (_itemState)
        {
            case ObjectPooling.Item_State.Bullet:
                item_BulletPooling.Item_Push(_item);
                break;
            case ObjectPooling.Item_State.Muzzle:
                item_MuzzlePooling.Item_Push(_item);
                break;
            case ObjectPooling.Item_State.Turbin:
                item_TurbinPooling.Item_Push(_item);
                break;
        }
    }

    // Upgrade 아이템 Pool POP
    internal void ItemPop(Transform _item, ObjectPooling.Item_State _itemState)
    {
        GameObject item = null;
        switch (_itemState)
        {
            case ObjectPooling.Item_State.Bullet:
                if (item_BulletPooling.Get_ItemState() != ObjectPooling.Item_State.Bullet)
                {
                    item_BulletPooling.Set_UpgradeItemState(ObjectPooling.Item_State.Bullet);
                }
                item = item_BulletPooling.Item_Pop();
                break;
            case ObjectPooling.Item_State.Muzzle:
                if (item_MuzzlePooling.Get_ItemState() != ObjectPooling.Item_State.Muzzle)
                {
                    item_MuzzlePooling.Set_UpgradeItemState(ObjectPooling.Item_State.Muzzle);
                }
                item = item_MuzzlePooling.Item_Pop();
                break;               
            case ObjectPooling.Item_State.Turbin:
                if (item_TurbinPooling.Get_ItemState() != ObjectPooling.Item_State.Turbin)
                {
                    item_TurbinPooling.Set_UpgradeItemState(ObjectPooling.Item_State.Turbin);
                }
                item = item_TurbinPooling.Item_Pop();
                break;
            default:
                item = null;
                break;
        }

        item.transform.position = _item.position;
        item.transform.rotation = _item.rotation;
        item.SetActive(true);
    }
}
