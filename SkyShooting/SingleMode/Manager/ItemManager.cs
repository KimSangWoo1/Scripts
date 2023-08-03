using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class ItemManager : Singleton<ItemManager>
{
    // 비행기 업그레이드 아이템 Pool
    private  ObjectPooling item_BulletPooling = new ObjectPooling();
    private  ObjectPooling item_MuzzlePooling = new ObjectPooling();
    private  ObjectPooling item_TurbinPooling = new ObjectPooling();
    // HP 아이템 Pool
    private  ObjectPooling item_RedHealthPooling = new ObjectPooling();
    private  ObjectPooling item_YellowHealthPooling = new ObjectPooling();
    private  ObjectPooling item_GreenHealthPooling = new ObjectPooling();
    // 돈 아이템 Pool
    private  ObjectPooling item_RedDollarPooling = new ObjectPooling();
    private  ObjectPooling item_YellowDollarPooling = new ObjectPooling();
    private  ObjectPooling item_GreenDollarPooling = new ObjectPooling();

    private void Awake()
    {

    }
    void Start()
    {
        // 비행기 업그레이드 아이템 생성
        item_BulletPooling.Set_UpgradeItemState(ObjectPooling.Item_State.Bullet);
        item_BulletPooling.Item_Creation();

        item_MuzzlePooling.Set_UpgradeItemState(ObjectPooling.Item_State.Muzzle);
        item_MuzzlePooling.Item_Creation();

        item_TurbinPooling.Set_UpgradeItemState(ObjectPooling.Item_State.Turbin);
        item_TurbinPooling.Item_Creation();

        //HP 아이템 생성
        item_RedHealthPooling.Set_HealthState(ObjectPooling.Item_State.Health, ObjectPooling.Item_HealthState.Red);
        item_RedHealthPooling.Item_Creation();

        item_YellowHealthPooling.Set_HealthState(ObjectPooling.Item_State.Health, ObjectPooling.Item_HealthState.Yellow);
        item_YellowHealthPooling.Item_Creation();

        item_GreenHealthPooling.Set_HealthState(ObjectPooling.Item_State.Health, ObjectPooling.Item_HealthState.Green);
        item_GreenHealthPooling.Item_Creation();

        // 돈 아이템 생성
        item_RedDollarPooling.Set_DollarState(ObjectPooling.Item_State.Dollar, ObjectPooling.Item_DollarState.Red);
        item_RedDollarPooling.Item_Creation();

        item_YellowDollarPooling.Set_DollarState(ObjectPooling.Item_State.Dollar, ObjectPooling.Item_DollarState.Yellow);
        item_YellowDollarPooling.Item_Creation();

        item_GreenDollarPooling.Set_DollarState(ObjectPooling.Item_State.Dollar, ObjectPooling.Item_DollarState.Green);
        item_GreenDollarPooling.Item_Creation();
    }

    //죽었을 경우 아이템 랜덤으로 나오게 하기
    public void ItemRandom(Transform deadPlane)
    {
        int random = Random.Range(1, 10);
        switch (random)
        {
            case 1:
                ItemPop(deadPlane, ObjectPooling.Item_State.Bullet);
                break;
            case 2:
                ItemPop(deadPlane, ObjectPooling.Item_State.Muzzle);
                break;
            case 3:
                ItemPop(deadPlane, ObjectPooling.Item_State.Turbin);
                break;
            case 4:
                ItemHealthPop(deadPlane, ObjectPooling.Item_HealthState.Red);
                break;
            case 5:
                ItemHealthPop(deadPlane, ObjectPooling.Item_HealthState.Yellow);
                break;
            case 6:
                ItemHealthPop(deadPlane, ObjectPooling.Item_HealthState.Green);
                break;
            case 7:
                ItemDollarPop(deadPlane, ObjectPooling.Item_DollarState.Red);
                break;
            case 8:
                ItemDollarPop(deadPlane, ObjectPooling.Item_DollarState.Yellow);
                break;
            case 9:
                ItemDollarPop(deadPlane, ObjectPooling.Item_DollarState.Green);
                break;
            default:
                break;

        }
    }
}
