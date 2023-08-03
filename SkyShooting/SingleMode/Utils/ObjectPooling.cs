using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Photon.Pun;

public partial class ObjectPooling
{ 
    public enum Pooling_State{ Cloud, TurnCloud, Bullet, StartBullet}; //오브젝트 풀링 상태

    private Pooling_State state;

    private int cloudSize = 3; //구름 사이즈
    private int bulletSize = 10; //총알 사이즈

    //풀링 셋팅
    private GameObject cloudParent;//구름 부모
    private GameObject bulletParent;//총알 부모

    private GameObject prefab; //구름&총알 Prefab
    private GameObject clone; //Clone
    
    //실제 풀 Queue 리스트
    private Queue<GameObject> cloudPool = new Queue<GameObject>(); //구름 Pool
    private Queue<GameObject> bulletPool = new Queue<GameObject>(); // 총알 Pool

    //풀링 대상 오브젝트 resource 참조
    public void Set_State(Pooling_State _state)
    {
        state = _state;
        switch (state)
        {
            case Pooling_State.Cloud:
                prefab = Resources.Load("Prefab/Env/Clouds") as GameObject;
                break;
            case Pooling_State.TurnCloud:
                prefab = Resources.Load("Prefab/Env/TurnClouds") as GameObject;
                break;
            case Pooling_State.Bullet:
                prefab = Resources.Load("Prefab/Bullets/Bullet") as GameObject;
                break;
            case Pooling_State.StartBullet:
                prefab = Resources.Load("Prefab/Bullets/Bullet2") as GameObject;
                break;
        }
    }

    public Pooling_State getState()
    {
        return state;
    }

    #region 주요 오브젝트 (총알,구름) 생성
    //오브젝트 생성
    public void Creation()
    {
        switch (state)
        {
            case Pooling_State.Cloud:
                if (cloudParent == null || !cloudParent.activeInHierarchy)
                {
                    cloudParent = GameObject.Find("구름Pool");
                    if (cloudParent == null)
                    {
                        cloudParent = new GameObject();
                        cloudParent.transform.name = "구름Pool";
                    }
                }

                for (int i = 0; i < cloudSize; i++)
                {
                    if (prefab == null)
                    {
                        Set_State(Pooling_State.Cloud);
                    }
                    clone = GameObject.Instantiate(prefab, cloudParent.transform.position, Quaternion.identity, cloudParent.transform);
                    clone.SetActive(false);
                    cloudPool.Enqueue(clone);
                }
                break;
            case Pooling_State.TurnCloud:
                if (cloudParent == null || !cloudParent.activeInHierarchy)
                {
                    cloudParent = GameObject.Find("구름Pool");
                    if (cloudParent == null)
                    {
                        cloudParent = new GameObject();
                        cloudParent.transform.name = "구름Pool";
                    }
                }

                for (int i = 0; i < cloudSize; i++)
                {
                    if (prefab == null)
                    {
                        Set_State(Pooling_State.Cloud);
                    }
                    clone = GameObject.Instantiate(prefab, cloudParent.transform.position, Quaternion.identity, cloudParent.transform);
                    clone.SetActive(false);
                    cloudPool.Enqueue(clone);
                }
                break;
            case Pooling_State.Bullet:
                if (bulletParent == null || !bulletParent.activeInHierarchy)
                {
                    bulletParent = GameObject.Find("총알Pool");
                    if (bulletParent == null)
                    {
                        bulletParent = new GameObject();
                        bulletParent.transform.name = "총알Pool";
                    }
                }

                for (int i = 0; i < bulletSize; i++)
                {
                    if (prefab == null)
                    {
                        Set_State(Pooling_State.Bullet);
                    }
                    clone = GameObject.Instantiate(prefab, bulletParent.transform.position, Quaternion.Euler(-90f, 0f, 0f), bulletParent.transform);
                    clone.SetActive(false);
                    bulletPool.Enqueue(clone);
                }
                break;
            case Pooling_State.StartBullet:
                if (bulletParent == null || !bulletParent.activeInHierarchy)
                {
                    bulletParent = GameObject.Find("총알Pool");
                    if (bulletParent == null)
                    {
                        bulletParent = new GameObject();
                        bulletParent.transform.name = "총알Pool";
                    }
                }

                for (int i = 0; i < bulletSize; i++)
                {
                    if (prefab == null)
                    {
                        Set_State(Pooling_State.StartBullet);
                    }
                    clone = GameObject.Instantiate(prefab, bulletParent.transform.position, Quaternion.Euler(-90f, 0f, 0f), bulletParent.transform);
                    clone.SetActive(false);
                    bulletPool.Enqueue(clone);
                }
                break;
        }
    }


    public void Creation(GameObject _prefab, CloudManager cm)
    {
        switch (state)
        {
            case Pooling_State.Cloud:
                if (cloudParent == null || !cloudParent.activeInHierarchy)
                {
                    cloudParent = GameObject.Find("구름Pool");
                    if (cloudParent == null)
                    {
                        cloudParent = new GameObject();
                        cloudParent.transform.name = "구름Pool";
                    }
                }

                for (int i = 0; i < cloudSize; i++)
                {
                    if (prefab == null)
                    {
                        Set_State(Pooling_State.Cloud);
                    }
                    clone = GameObject.Instantiate(_prefab, cloudParent.transform.position, Quaternion.identity, cloudParent.transform);
                    clone.SetActive(false);
                    cloudPool.Enqueue(clone);
                }
                break;
            case Pooling_State.TurnCloud:
                if (cloudParent == null || !cloudParent.activeInHierarchy)
                {
                    cloudParent = GameObject.Find("구름Pool");
                    if (cloudParent == null)
                    {
                        cloudParent = new GameObject();
                        cloudParent.transform.name = "구름Pool";
                    }
                }

                for (int i = 0; i < cloudSize; i++)
                {
                    if (prefab == null)
                    {
                        Set_State(Pooling_State.Cloud);
                    }
                    clone = GameObject.Instantiate(_prefab, cloudParent.transform.position, Quaternion.identity, cloudParent.transform);
                    clone.GetComponent<Cloud_Move>().SetManager(cm);
                    clone.SetActive(false);
                    cloudPool.Enqueue(clone);
                }
                break;
        }
    }
    #endregion

    #region PUSH
    //오브젝트 넣기
    public void Push(GameObject temp)
    {
        //Debug.Log("push");
        if (temp.activeSelf)
        {
            temp.SetActive(false);
        }
        switch (state)
        {
            case Pooling_State.Cloud:
                cloudPool.Enqueue(temp);
                break;
            case Pooling_State.TurnCloud:
                cloudPool.Enqueue(temp);
                break;
            case Pooling_State.Bullet:
                bulletPool.Enqueue(temp);
                break;
            case Pooling_State.StartBullet:
                bulletPool.Enqueue(temp);
                break;
        }
    }

    #endregion

    #region POP
    //오브젝트 빼기
    public GameObject Pop()
    {
        GameObject temp;
        switch (state)
        {
            case Pooling_State.Cloud:
                if (cloudPool.Count == 0)
                {
                    Creation();
                }
                temp = cloudPool.Dequeue();
                break;
            case Pooling_State.TurnCloud:
                if (cloudPool.Count == 0)
                {
                    Creation();
                }
                temp = cloudPool.Dequeue();
                break;
            case Pooling_State.Bullet:
                if (bulletPool.Count == 0)
                {
                    Creation();
                }
                temp = bulletPool.Dequeue();
                break;
            case Pooling_State.StartBullet:
                if (bulletPool.Count == 0)
                {
                    Creation();
                }
                temp = bulletPool.Dequeue();
                break;
            default:
                temp = null;
                break;
        }
        return temp;
    }
    #endregion
}


