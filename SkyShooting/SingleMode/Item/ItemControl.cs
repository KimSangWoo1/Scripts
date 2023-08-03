using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemControl : MonoBehaviour
{
    //Manager 싱글톤
    ItemManager IM;

    //오브젝트 풀링
    public ObjectPooling.Item_State itemState;
    public ObjectPooling.Item_HealthState healthState;
    public ObjectPooling.Item_DollarState dollarState;


    private void Awake()
    {
        IM = ItemManager.Instance;

    }

    public void OnCollisionEnter(Collision collision)
    {
        switch (itemState)
        {
            case ObjectPooling.Item_State.Bullet:
            case ObjectPooling.Item_State.Muzzle:
            case ObjectPooling.Item_State.Turbin:
                IM.ItemUpgradePush(this.gameObject, itemState); //Push 및 active 설정
                break;
            case ObjectPooling.Item_State.Health:
                switch (healthState)
                {
                    case ObjectPooling.Item_HealthState.None:
                        break;
                    case ObjectPooling.Item_HealthState.Red:
                    case ObjectPooling.Item_HealthState.Yellow:
                    case ObjectPooling.Item_HealthState.Green:
                        IM.ItemHealthPush(this.gameObject, healthState); //Push 및 active 설정
                        break;
                }
                break;
            case ObjectPooling.Item_State.Dollar:
                switch (dollarState)
                {
                    case ObjectPooling.Item_DollarState.None:
                        break;
                    case ObjectPooling.Item_DollarState.Red:
                    case ObjectPooling.Item_DollarState.Yellow:
                    case ObjectPooling.Item_DollarState.Green:
                        IM.ItemDollarPush(this.gameObject, dollarState); //Push 및 active 설정                
                        break;
                }
                break;
        }
        this.gameObject.SetActive(false);
    }
}
