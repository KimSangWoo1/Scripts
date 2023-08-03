using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public partial class ObjectPooling
{ 
    public enum PlaneState{Player, AI}; //오브젝트 풀링 상태

    private PlaneState planeState;

    private int playerSize = 10; //플레이어 사이즈
    private int AISize = 55; //AI 사이즈

    //풀링 셋팅
    private GameObject playerParent;//플레이어 부모
    private GameObject AIParent;//AI 부모

    //실제 풀 Queue 리스트
    private Queue<GameObject> playerPool = new Queue<GameObject>(); //플레이어 Pool
    private Queue<GameObject> AIPool = new Queue<GameObject>(); // AI Pool

    //풀링 대상 오브젝트 resource 참조
    public void Set_PlaneState(PlaneState _state)
    {
        planeState = _state;
        switch (planeState)
        {
            case PlaneState.Player:
                prefab = Resources.Load("PhotonPrefab/Plane/Resources/Player") as GameObject;
                break;
            case PlaneState.AI:
                prefab = Resources.Load("PhotonPrefab/Plane/Resources/AI") as GameObject;
                break;
        }
    }

    public PlaneState Get_PlaneState()
    {
        return planeState;
    }

    #region 비행기 (AI,Player) 생성
    //오브젝트 생성
    public void PlaneCreation()
    {
        switch (planeState)
        {
            case PlaneState.Player:
                if (playerParent == null || !playerParent.activeInHierarchy)
                {
                    playerParent = GameObject.Find("Player Pool");
                    if (playerParent == null)
                    {
                        playerParent = new GameObject();
                        playerParent.transform.name = "Player Pool";
                        playerParent.transform.position = new Vector3(0f, 2f, 0f);
                    }
                }

                for (int i = 0; i < playerSize; i++)
                {
                    if (prefab == null)
                    {
                        Set_PlaneState(PlaneState.Player);
                    }
                    clone = GameObject.Instantiate(prefab, playerParent.transform.position, Quaternion.identity, playerParent.transform);
                    clone.SetActive(false);
                    playerPool.Enqueue(clone);
                }
                break;
            case PlaneState.AI:
                if (AIParent == null || !AIParent.activeInHierarchy)
                {
                    AIParent = GameObject.Find("AI Pool");
                    if (AIParent == null)
                    {
                        AIParent = new GameObject();
                        AIParent.transform.name = "AI Pool";
                        AIParent.transform.position = new Vector3(0f, 2f, 0f);
                    }
                }

                for (int i = 0; i < AISize; i++)
                {
                    if (prefab == null)
                    {
                        Set_PlaneState(PlaneState.AI);
                    }
                    clone = GameObject.Instantiate(prefab, AIParent.transform.position, Quaternion.identity, AIParent.transform);
                    clone.SetActive(false);
                    AIPool.Enqueue(clone);
                }
                break;
        }
    }
    #endregion

    #region PUSH
    //오브젝트 넣기
    public void PlanePush(GameObject temp)
    {
        //Debug.Log("push");
        if (temp.activeSelf)
        {
            temp.SetActive(false);
        }
        switch (planeState)
        {
            case PlaneState.Player:
                playerPool.Enqueue(temp);
                break;
            case PlaneState.AI:
                AIPool.Enqueue(temp);
                break;
        }
    }

    #endregion

    #region POP
    //오브젝트 빼기
    public GameObject PlanePop()
    {
        GameObject temp;
        switch (planeState)
        {
            case PlaneState.Player:
                if (playerPool.Count == 0)
                {
                    PlaneCreation();
                }
                temp = playerPool.Dequeue();
                break;
            case PlaneState.AI:
                if (AIPool.Count == 0)
                {
                    PlaneCreation();
                }
                temp = AIPool.Dequeue();
                break;
            default:
                temp = null;
                break;
        }
        return temp;
    }
    #endregion
}


