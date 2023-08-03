using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class FX_Manager : Singleton<FX_Manager>
{
    private  ObjectPooling FX_ItemPool = new ObjectPooling(); //FX_Item Pool
    private ObjectPooling FX_HealthPool = new ObjectPooling(); //FX_Health Pool
    private ObjectPooling FX_MoneyPool = new ObjectPooling();// FX Money Pool

    private ObjectPooling FX_RedDeadPool = new ObjectPooling();// FX_RedDead Pool
    private  ObjectPooling FX_GreenDeadPool = new ObjectPooling();// FX_GreenDead Pool
    private  ObjectPooling FX_BlueDeadPool = new ObjectPooling(); // FX_BlueDead Pool
    private  ObjectPooling FX_OrangeDeadPool = new ObjectPooling(); // FX_OrangeDead Pool

    private void Awake()
    {

    }
    void Start()
    {
        //FX Item 생성
        FX_ItemPool.SetFXState(ObjectPooling.FX_State.item);
        FX_ItemPool.FX_Creation();

        //FX Health 생성
        FX_MoneyPool.SetFXState(ObjectPooling.FX_State.Health);
        FX_MoneyPool.FX_Creation();

        //FX Money 생성
        FX_MoneyPool.SetFXState(ObjectPooling.FX_State.Money);
        FX_MoneyPool.FX_Creation();

        //FX Dead 생성
        FX_RedDeadPool.Set_FX_DeadState(ObjectPooling.FX_State.Dead, ObjectPooling.DeadState.Red);
        FX_RedDeadPool.FX_Creation();

        FX_GreenDeadPool.Set_FX_DeadState(ObjectPooling.FX_State.Dead, ObjectPooling.DeadState.Green);
        FX_GreenDeadPool.FX_Creation();

        FX_BlueDeadPool.Set_FX_DeadState(ObjectPooling.FX_State.Dead, ObjectPooling.DeadState.Blue);
        FX_BlueDeadPool.FX_Creation();

        FX_OrangeDeadPool.Set_FX_DeadState(ObjectPooling.FX_State.Dead, ObjectPooling.DeadState.Orange);
        FX_OrangeDeadPool.FX_Creation();
    }

}
