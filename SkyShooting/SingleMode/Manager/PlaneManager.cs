using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneManager : Singleton<PlaneManager>
{
    private ObjectPooling PlayerPool = new ObjectPooling(); //FX_Item Pool
    private ObjectPooling AIPool = new ObjectPooling();// FX Money Pool

    private void Awake()
    {

    }
    void Start()
    {
        //비행기 생성
        PlayerPool.Set_PlaneState(ObjectPooling.PlaneState.Player);
        PlayerPool.PlaneCreation();

        AIPool.Set_PlaneState(ObjectPooling.PlaneState.AI);
        AIPool.PlaneCreation();

    }

    //플레이어 Pool Push
    internal void Plane_Push(GameObject _plane, ObjectPooling.PlaneState _planeState)
    {
        _plane.SetActive(false);

        switch (_planeState)
        {
            case ObjectPooling.PlaneState.Player:
                PlayerPool.PlanePush(_plane);
                break;
            case ObjectPooling.PlaneState.AI:
                AIPool.PlanePush(_plane);
                break;       
        }
    }

    //AI Pool POP
    internal GameObject Plane_Pop(ObjectPooling.PlaneState _planeState)
    {
        GameObject plane = null;
        switch (_planeState)
        {
            case ObjectPooling.PlaneState.Player:
                if (PlayerPool.Get_PlaneState() != ObjectPooling.PlaneState.Player)
                {
                    PlayerPool.Set_PlaneState(ObjectPooling.PlaneState.Player);
                }
                plane = PlayerPool.PlanePop();
                break;
            case ObjectPooling.PlaneState.AI:
                if (AIPool.Get_PlaneState() != ObjectPooling.PlaneState.AI)
                {
                    AIPool.Set_PlaneState(ObjectPooling.PlaneState.AI);
                }
                plane = AIPool.PlanePop();
                break;
        }

        return plane;
    }
}
