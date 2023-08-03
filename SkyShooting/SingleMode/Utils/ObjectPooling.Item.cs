using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public partial class ObjectPooling
{ 
    public enum Item_State{Bullet, Muzzle, Turbin, Health, Dollar}; //오브젝트 풀링 상태
    public enum Item_HealthState {None, Red, Yellow, Green };
    public enum Item_DollarState {None, Red, Yellow, Green };

    private Item_State item_State;
    private Item_HealthState item_HealthState;
    private Item_DollarState item_DollarState;
    
    //크기 설정
    private int item_BulletSize = 10; //아이템 총알 사이즈
    private int item_MuzzleSize = 10; //아이템 머즐 사이즈
    private int item_TurbinSize = 10; //아이템 터빈 사이즈
    private int item_HealthSize = 10; //아이템 HP 사이즈
    private int item_DollarSize = 10; //아이템 돈 사이즈

    //부모 설정
    private GameObject item_BulletParent; //아이템 총알 부모
    private GameObject item_MuzzleParent;  //아이템 머즐 부모
    private GameObject item_TurbinParent;  //아이템 터빈 부모
    private GameObject item_HealthParent; //아이템 HP 부모
    private GameObject item_DollarParent; //아이템 HP 부모

    // 비행기 업그레이드 아이템 Pool
    private Queue<GameObject> item_BulletPool = new Queue<GameObject>(); // item 총알 Pool
    private Queue<GameObject> item_MuzzlePool = new Queue<GameObject>(); // Item 머즐 Pool
    private Queue<GameObject> item_TurbinPool = new Queue<GameObject>(); // Item 터빈 Pool

    //체력 아이템 Pool
    private Queue<GameObject> item_RedHealthPool = new Queue<GameObject>(); // item 빨강 HP Pool
    private Queue<GameObject> item_YellowHealthPool = new Queue<GameObject>(); // item 노랑 HP Pool
    private Queue<GameObject> item_GreenHealthPool = new Queue<GameObject>(); // item 초록 HP Pool
    //돈 아이템 Pool
    private Queue<GameObject> item_RedDollarPool = new Queue<GameObject>(); // item 빨강 돈 Pool
    private Queue<GameObject> item_YellowDollarPool = new Queue<GameObject>(); // item 노랑 돈 Pool
    private Queue<GameObject> item_GreenDollarPool = new Queue<GameObject>(); // item 초록 돈 Pool


    #region SET
    //풀링 대상 오브젝트 resource 참조
    public void Set_UpgradeItemState(Item_State _state)
    {
        item_State = _state;
        switch (item_State)
        {
            case Item_State.Bullet:
                prefab = Resources.Load("Prefab/Item/Bullet_Blue") as GameObject;
                break;
            case Item_State.Muzzle:
                prefab = Resources.Load("Prefab/Item/Muzzle_Blue") as GameObject;
                break;
            case Item_State.Turbin:
                prefab = Resources.Load("Prefab/Item/Turbin_Blue") as GameObject;
                break;
        }
    }

    // HP 상태 설정
    public void Set_HealthState(Item_State _state, Item_HealthState subState)
    {
        item_State = _state;
        item_HealthState = subState;

        switch (item_State)
        {
            case Item_State.Health:
                switch (item_HealthState)
                {
                    case Item_HealthState.None:
                        break;
                    case Item_HealthState.Red:
                        prefab = Resources.Load("Prefab/Item/Health_Red") as GameObject;
                        break;
                    case Item_HealthState.Yellow:
                        prefab = Resources.Load("Prefab/Item/Health_Yellow") as GameObject;
                        break;
                    case Item_HealthState.Green:
                        prefab = Resources.Load("Prefab/Item/Health_Green") as GameObject;
                        break;
                }
                break;
        }
    }

    // 돈 상태 설정
    public void Set_DollarState(Item_State _state, Item_DollarState subState)
    {
        item_State = _state;
        item_DollarState = subState;

        switch (item_State)
        {
            case Item_State.Dollar:
                switch (item_DollarState)
                {
                    case Item_DollarState.None:
                        break;
                    case Item_DollarState.Red:
                        prefab = Resources.Load("Prefab/Item/Dollar_Red") as GameObject;
                        break;
                    case Item_DollarState.Yellow:
                        prefab = Resources.Load("Prefab/Item/Dollar_Yellow") as GameObject;
                        break;
                    case Item_DollarState.Green:
                        prefab = Resources.Load("Prefab/Item/Dollar_Green") as GameObject;
                        break;
                }
                break;
        }
    }
    #endregion

    #region GET
    public Item_State Get_ItemState()
    {
        return item_State;
    }
    
    public Item_HealthState Get_HealthState()
    {
        return item_HealthState;
    }

    public Item_DollarState Get_DollarState()
    {
        return item_DollarState;
    }
    #endregion

    #region 아이템 생성
    //오브젝트 생성
    public void Item_Creation()
    {
        switch (item_State)
        {
            case Item_State.Bullet:
                if (item_BulletParent == null || !item_BulletParent.activeInHierarchy)
                {
                    item_BulletParent = GameObject.Find("Item_BulletPool");
                    if (item_BulletParent == null)
                    {
                        item_BulletParent = new GameObject();
                        item_BulletParent.transform.name = "Item_BulletPool";
                    }
                }

                for (int i = 0; i < item_BulletSize; i++)
                {
                    if (prefab == null)
                    {
                        Set_UpgradeItemState(Item_State.Bullet);
                    }
                    clone = GameObject.Instantiate(prefab, item_BulletParent.transform.position, Quaternion.identity, item_BulletParent.transform);
                    clone.SetActive(false);
                    item_BulletPool.Enqueue(clone);
                }
                break;
            case Item_State.Muzzle:
                if (item_MuzzleParent == null || !item_MuzzleParent.activeInHierarchy)
                {
                    item_MuzzleParent = GameObject.Find("Item_MuzzlePool");
                    if (item_MuzzleParent == null)
                    {
                        item_MuzzleParent = new GameObject();
                        item_MuzzleParent.transform.name = "Item_MuzzlePool";
                    }
                }

                for (int i = 0; i < item_MuzzleSize; i++)
                {
                    if (prefab == null)
                    {
                        Set_UpgradeItemState(Item_State.Muzzle);
                    }
                    clone = GameObject.Instantiate(prefab, item_MuzzleParent.transform.position, Quaternion.identity, item_MuzzleParent.transform);
                    clone.SetActive(false);
                    item_MuzzlePool.Enqueue(clone);
                }
                break;
            case Item_State.Turbin:
                if (item_TurbinParent == null || !item_TurbinParent.activeInHierarchy)
                {
                    item_TurbinParent = GameObject.Find("Item_TurbinPool");
                    if (item_TurbinParent == null)
                    {
                        item_TurbinParent = new GameObject();
                        item_TurbinParent.transform.name = "Item_TurbinPool";
                    }
                }

                for (int i = 0; i < item_TurbinSize; i++)
                {
                    if (prefab == null)
                    {
                        Set_UpgradeItemState(Item_State.Turbin);
                    }
                    clone = GameObject.Instantiate(prefab, item_TurbinParent.transform.position, Quaternion.identity, item_TurbinParent.transform);
                    clone.SetActive(false);
                    item_TurbinPool.Enqueue(clone);
                }
                break;
            case Item_State.Health:
                if (item_HealthParent == null || !item_HealthParent.activeInHierarchy)
                {
                    item_HealthParent = GameObject.Find("Item_HealthPool");
                    if (item_HealthParent == null)
                    {
                        item_HealthParent = new GameObject();
                        item_HealthParent.transform.name = "Item_HealthPool";
                    }
                }

                for (int i = 0; i < item_HealthSize; i++)
                {
                    if (prefab == null)
                    {
                        Set_HealthState(Item_State.Health, item_HealthState);
                    }
                    clone = GameObject.Instantiate(prefab, item_HealthParent.transform.position, Quaternion.identity, item_HealthParent.transform);
                    clone.SetActive(false);
                    switch (item_HealthState)
                    {
                        case Item_HealthState.None:
                            break;
                        case Item_HealthState.Red:
                            item_RedHealthPool.Enqueue(clone);
                            break;
                        case Item_HealthState.Yellow:
                            item_YellowHealthPool.Enqueue(clone);
                            break;
                        case Item_HealthState.Green:
                            item_GreenHealthPool.Enqueue(clone);
                            break;
                    }
                }
                break;
            case Item_State.Dollar:
                if (item_DollarParent == null || !item_DollarParent.activeInHierarchy)
                {
                    item_DollarParent = GameObject.Find("Item_DollarPool");
                    if (item_DollarParent == null)
                    {
                        item_DollarParent = new GameObject();
                        item_DollarParent.transform.name = "Item_DollarPool";
                    }
                }

                for (int i = 0; i < item_DollarSize; i++)
                {
                    if (prefab == null)
                    {
                        Set_DollarState(Item_State.Dollar, item_DollarState);
                    }
                    clone = GameObject.Instantiate(prefab, item_DollarParent.transform.position, Quaternion.identity, item_DollarParent.transform);
                    clone.SetActive(false);
                    switch (item_DollarState)
                    {
                        case Item_DollarState.None:
                            break;
                        case Item_DollarState.Red:
                            item_RedDollarPool.Enqueue(clone);
                            break;
                        case Item_DollarState.Yellow:
                            item_YellowDollarPool.Enqueue(clone);
                            break;
                        case Item_DollarState.Green:
                            item_GreenDollarPool.Enqueue(clone);
                            break;
                    }
                }
                break;
        }
    }
    #endregion

    #region PUSH
    //오브젝트 넣기
    public void Item_Push(GameObject temp)
    {
        if (temp.activeSelf)
        {
            temp.SetActive(false);
        }

        switch (item_State)
        {
            case Item_State.Bullet:
                item_BulletPool.Enqueue(temp);
                break;
            case Item_State.Muzzle:
                item_MuzzlePool.Enqueue(temp);
                break;
            case Item_State.Turbin:
                item_TurbinPool.Enqueue(temp);
                break;
            case Item_State.Health:
                switch (item_HealthState)
                {
                    case Item_HealthState.None:
                        break;
                    case Item_HealthState.Red:
                        item_RedHealthPool.Enqueue(clone);
                        break;
                    case Item_HealthState.Yellow:
                        item_YellowHealthPool.Enqueue(clone);
                        break;
                    case Item_HealthState.Green:
                        item_GreenHealthPool.Enqueue(clone);
                        break;
                }
                break;
            case Item_State.Dollar:
                switch (item_DollarState)
                {
                    case Item_DollarState.None:
                        break;
                    case Item_DollarState.Red:
                        item_RedDollarPool.Enqueue(clone);
                        break;
                    case Item_DollarState.Yellow:
                        item_YellowDollarPool.Enqueue(clone);
                        break;
                    case Item_DollarState.Green:
                        item_GreenDollarPool.Enqueue(clone);
                        break;
                }
                break;
        }
    }

        #endregion

    #region POP
        //오브젝트 빼기
        public GameObject Item_Pop()
    {
        GameObject temp;
        switch (item_State)
        {
            case Item_State.Bullet:
                if (item_BulletPool.Count == 0)
                {
                    Item_Creation();
                }
                temp = item_BulletPool.Dequeue();
                break;
            case Item_State.Muzzle:
                if (item_MuzzlePool.Count == 0)
                {
                    Item_Creation();
                }
                temp = item_MuzzlePool.Dequeue();

                break;
            case Item_State.Turbin:
                if (item_TurbinPool.Count == 0)
                {
                    Item_Creation();
                }
                temp = item_TurbinPool.Dequeue();

                break;
            case Item_State.Health:
                switch (item_HealthState)
                {
                    case Item_HealthState.None:
                        temp = null;
                        break;
                    case Item_HealthState.Red:
                        if (item_RedHealthPool.Count == 0)
                        {
                            Item_Creation();
                        }
                        temp = item_RedHealthPool.Dequeue();
                        break;
                    case Item_HealthState.Yellow:
                        if (item_YellowHealthPool.Count == 0)
                        {
                            Item_Creation();
                        }
                        temp = item_YellowHealthPool.Dequeue();
                        break;
                    case Item_HealthState.Green:
                        if (item_GreenHealthPool.Count == 0)
                        {
                            Item_Creation();
                        }
                        temp = item_GreenHealthPool.Dequeue();
                        break;
                    default:
                        temp = null;
                        break;
                }
                break;
            case Item_State.Dollar:
                switch (item_DollarState)
                {
                    case Item_DollarState.None:
                        temp = null;
                        break;
                    case Item_DollarState.Red:
                        if (item_RedDollarPool.Count == 0)
                        {
                            Item_Creation();
                        }
                        temp = item_RedDollarPool.Dequeue();
                        break;
                    case Item_DollarState.Yellow:
                        if (item_YellowDollarPool.Count == 0)
                        {
                            Item_Creation();
                        }
                        temp = item_YellowDollarPool.Dequeue();
                        break;
                    case Item_DollarState.Green:
                        if (item_GreenDollarPool.Count == 0)
                        {
                            Item_Creation();
                        }
                        temp = item_GreenDollarPool.Dequeue();
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


