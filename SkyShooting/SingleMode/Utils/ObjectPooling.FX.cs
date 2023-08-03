using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public partial class ObjectPooling : MonoBehaviour
{
    public enum FX_State {item,Health,Money, Dead };
    public enum DeadState {None, Red, Green, Blue, Orange };

    private FX_State fxState;
    private DeadState deadState;

    //크기 설정
    private int FX_ItemSize = 10; //아이템 첫 사이즈
    private int FX_HealthSize = 5; //체력 첫 사이즈
    private int FX_MoneySize = 5; //돈 첫 사이즈
    private int FX_DeadSize = 5;  //죽음 첫 사이즈

    //부모 설정
    private GameObject FX_ItemParent;  //FX 아이템 부모
    private GameObject FX_HealthParent;  //FX 체력 부모
    private GameObject FX_MoneyParent;  //FX 돈 부모
    private GameObject FX_DeadParent;  //FX 죽음 부모

    // FX 아이템 Pool
    private Queue<GameObject> FX_ItemPool = new Queue<GameObject>(); // FX_Item Pool
    // FX 체력 Pool
    private Queue<GameObject> FX_HealthPool = new Queue<GameObject>(); // FX_Money Pool
    // FX 돈 Pool
    private Queue<GameObject> FX_MoneyPool = new Queue<GameObject>(); // FX_Money Pool


    // FX 죽음 Pool
    private Queue<GameObject> FX_RedDeadPool = new Queue<GameObject>(); // FX_RedDead Pool
    private Queue<GameObject> FX_GreenDeadPool = new Queue<GameObject>(); // FX_GreenDead Pool
    private Queue<GameObject> FX_BlueDeadPool = new Queue<GameObject>(); // FX_BlueDead Pool
    private Queue<GameObject> FX_OrangeDeadPool = new Queue<GameObject>(); // FX_OrangeDead Pool

    #region SET
    public void SetFXState(FX_State _state)
    {
        fxState = _state;
        switch (fxState)
        {
            case FX_State.item:
                prefab = Resources.Load("Prefab/FX/Effect/FX_Rainbow") as GameObject;
                break;
            case FX_State.Health:
                prefab = Resources.Load("Prefab/FX/Effect/FX_HealthRainbow") as GameObject;
                break;
            case FX_State.Money:
                prefab = Resources.Load("Prefab/FX/Effect/FX_MoneyRainbow") as GameObject;
                break;
        }
    }

    //죽음 상태
    public void Set_FX_DeadState(FX_State _state, DeadState subState)
    {
        fxState = _state;
        deadState = subState;

        switch (fxState)
        {
            case FX_State.Dead:
                switch (deadState)
                {
                    case DeadState.None:
                        break;
                    case DeadState.Red:
                        prefab = Resources.Load("Prefab/FX/Dead/FX_RedDead") as GameObject;
                        break;
                    case DeadState.Green:
                        prefab = Resources.Load("Prefab/FX/Dead/FX_GreenDead") as GameObject;
                        break;
                    case DeadState.Blue:
                        prefab = Resources.Load("Prefab/FX/Dead/FX_BlueDead") as GameObject;
                        break;
                    case DeadState.Orange:
                        prefab = Resources.Load("Prefab/FX/Dead/FX_OrangeDead") as GameObject;
                        break;
                }
                break;
        }
    }
    #endregion
    #region GET
    public FX_State Get_FX_State()
    {
        return fxState;
    }

    public DeadState Get_FX_DeadState()
    {
        return deadState;
    }
    #endregion

    #region FX 생성
    //오브젝트 생성
    public void FX_Creation()
    {

        switch (fxState)
        {
            case FX_State.item:
                if (FX_ItemParent == null || !FX_ItemParent.activeInHierarchy)
                {
                    FX_ItemParent = GameObject.Find("FX_ItemPool");
                    if (FX_ItemParent == null)
                    {
                        FX_ItemParent = new GameObject();
                        FX_ItemParent.transform.name = "FX_ItemPool";
                    }
                }

                for (int i = 0; i < FX_ItemSize; i++)
                {
                    if (prefab == null)
                    {
                        SetFXState(FX_State.item);
                    }
                    clone = GameObject.Instantiate(prefab, FX_ItemParent.transform.position, Quaternion.Euler(0f, 0f, 0f), FX_ItemParent.transform);
                    clone.SetActive(false);
                    FX_ItemPool.Enqueue(clone);
                }
                break;
            case FX_State.Health:
                if (FX_HealthParent == null || !FX_HealthParent.activeInHierarchy)
                {
                    FX_HealthParent = GameObject.Find("FX_HealthPool");
                    if (FX_HealthParent == null)
                    {
                        FX_HealthParent = new GameObject();
                        FX_HealthParent.transform.name = "FX_HealthPool";
                    }
                }

                for (int i = 0; i < FX_HealthSize; i++)
                {
                    if (prefab == null)
                    {
                        SetFXState(FX_State.Money);
                    }
                    clone = GameObject.Instantiate(prefab, FX_HealthParent.transform.position, Quaternion.Euler(0f, 0f, 0f), FX_HealthParent.transform);
                    clone.SetActive(false);
                    FX_HealthPool.Enqueue(clone);
                }
                break;
            case FX_State.Money:
                if (FX_MoneyParent == null || !FX_MoneyParent.activeInHierarchy)
                {
                    FX_MoneyParent = GameObject.Find("FX_MoneyPool");
                    if (FX_MoneyParent == null)
                    {
                        FX_MoneyParent = new GameObject();
                        FX_MoneyParent.transform.name = "FX_MoneyPool";
                    }
                }

                for (int i = 0; i < FX_MoneySize; i++)
                {
                    if (prefab == null)
                    {
                        SetFXState(FX_State.Money);
                    }
                    clone = GameObject.Instantiate(prefab, FX_MoneyParent.transform.position, Quaternion.Euler(0f, 0f, 0f), FX_MoneyParent.transform);
                    clone.SetActive(false);
                    FX_MoneyPool.Enqueue(clone);
                }
                break;
            case FX_State.Dead:
                if (FX_DeadParent == null || !FX_DeadParent.activeInHierarchy)
                {
                    FX_DeadParent = GameObject.Find("FX_DeadPool");
                    if (FX_DeadParent == null)
                    {
                        FX_DeadParent = new GameObject();
                        FX_DeadParent.transform.name = "FX_DeadPool";
                    }
                }

                for (int i = 0; i < FX_DeadSize; i++)
                {
                    if (prefab == null)
                    {
                        Set_FX_DeadState(FX_State.Dead,deadState);
                    }
                    clone = GameObject.Instantiate(prefab, FX_DeadParent.transform.position, Quaternion.Euler(-90f, 0f, 0f), FX_DeadParent.transform);
                    clone.SetActive(false);
                    switch (deadState)
                    {
                        case DeadState.None:
                            break;
                        case DeadState.Red:
                            FX_RedDeadPool.Enqueue(clone);
                            break;
                        case DeadState.Green:
                            FX_GreenDeadPool.Enqueue(clone);
                            break;
                        case DeadState.Blue:
                            FX_BlueDeadPool.Enqueue(clone);
                            break;
                        case DeadState.Orange:
                            FX_OrangeDeadPool.Enqueue(clone);
                            break;
                    }
                }
                break;
        }
    }
    #endregion

    #region PUSH
    // FX Push
    public void FX_Push(GameObject temp)
    {
        if (temp.activeSelf)
        {
            temp.SetActive(false);
        }

        switch (fxState)
        {
            case FX_State.item:
                FX_ItemPool.Enqueue(temp);
                break;
            case FX_State.Health:
                FX_HealthPool.Enqueue(temp);
                break;
            case FX_State.Money:
                FX_MoneyPool.Enqueue(temp);
                break;
            case FX_State.Dead:
                switch (deadState)
                {
                    case DeadState.None:
                        break;
                    case DeadState.Red:
                        FX_RedDeadPool.Enqueue(clone);
                        break;
                    case DeadState.Green:
                        FX_GreenDeadPool.Enqueue(clone);
                        break;
                    case DeadState.Blue:
                        FX_BlueDeadPool.Enqueue(clone);
                        break;
                    case DeadState.Orange:
                        FX_OrangeDeadPool.Enqueue(clone);
                        break;
                }
                break;
        }
    }
    #endregion

    #region POP
    public GameObject FX_Pop()
    {
        GameObject temp;

        switch (fxState)
        {
            case FX_State.item:
                if (FX_ItemPool.Count == 0)
                {
                    FX_Creation();
                }
                temp = FX_ItemPool.Dequeue();
                break;
            case FX_State.Health:
                if (FX_HealthPool.Count == 0)
                {
                    FX_Creation();
                }
                temp = FX_HealthPool.Dequeue();
                break;
            case FX_State.Money:
                if (FX_MoneyPool.Count == 0)
                {
                    FX_Creation();
                }
                temp = FX_MoneyPool.Dequeue();
                break;
            case FX_State.Dead:
                switch (deadState)
                {
                    case DeadState.None:
                        temp = null;
                        break;
                    case DeadState.Red:
                        if (FX_RedDeadPool.Count == 0)
                        {
                            FX_Creation();
                        }
                        temp = FX_RedDeadPool.Dequeue();
                        break;
                    case DeadState.Green:
                        if (FX_GreenDeadPool.Count == 0)
                        {
                            FX_Creation();
                        }
                        temp = FX_GreenDeadPool.Dequeue();
                        break;
                    case DeadState.Blue:
                        if (FX_BlueDeadPool.Count == 0)
                        {
                            FX_Creation();
                        }
                        temp = FX_BlueDeadPool.Dequeue();
                        break;
                    case DeadState.Orange:
                        if (FX_OrangeDeadPool.Count == 0)
                        {
                            FX_Creation();
                        }
                        temp = FX_OrangeDeadPool.Dequeue();
                        break;
                    default:
                        temp = null;
                        break;
                }
                break;
            default:
                temp = null;
                break;
        }
        return temp;
    }
    #endregion
}
