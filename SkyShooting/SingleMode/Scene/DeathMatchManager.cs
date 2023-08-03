using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class DeathMatchManager : Singleton<DeathMatchManager>
{
    SoundManager SM;

    public Map map;
    public Material[] materials;
    public GameObject Player;
    public int maxAI;

    private PlaneManager PM;
    private GameObject[] AIs = new GameObject[50];
    private float playTime;
    private int aiCount;

    private void Awake()
    {
        SM = SoundManager.Instance;
        PM = PlaneManager.Instance;

        SingleGamePlay.init();
        SingleGamePlay.Make_RandomName();
        aiCount = 0;
    }

    void Start()
    {
        //방 입장시 MasterClient && 로컬 오브젝트 일 경우 AI 생성

        StartCoroutine(AICreation()); //AI생성
        PlayerCreation(); //Player생성
        SM.BGMPlay(SoundManager.BGM.DeathMatch);
    }

    void Update()
    {
        playTime += Time.deltaTime;
        if(playTime > 10f)
        {
            if (aiCount < maxAI)
            {
                StartCoroutine(AICreation()); //AI생성
            }
            else
            {
                playTime = 0f;
            }
        }
    }
    #region 비행기 생성
    //1.AI생성
    IEnumerator AICreation()
    {
        while(aiCount< maxAI)
        {
            //비행기 Pop
            GameObject cloneAI = PM.Plane_Pop(ObjectPooling.PlaneState.AI);
            //material 설정
            int randomColor = Random.Range(0, 4);
            cloneAI.GetComponent<Renderer>().material = materials[randomColor];
            //색상FX와 부스터 설정 그리고 게임 참가 준비
            cloneAI.GetComponent<PlaneBase>().GamePreparation(randomColor, Random.Range(0, 4));

            //위치 설정   
            cloneAI.transform.position = map.Random_Position();
            //Object Active
            cloneAI.SetActive(true);
            //Test
            cloneAI.transform.name = aiCount + " AI";
            AIs[aiCount] = cloneAI;

            aiCount++;
        }
        yield return new WaitForEndOfFrame();

    }

    //2.Player 생성
    public void PlayerCreation()
    {
        //GameObject player = PM.Plane_Pop(ObjectPooling.PlaneState.Player);

        Player.SetActive(false);
        //material 설정
        Player.GetComponent<Renderer>().material = materials[GameManager.planeNumber];
        //색상FX와 부스터 설정 그리고 게임 참가 준비
        Player.GetComponent<PlaneBase>().GamePreparation(GameManager.planeNumber, GameManager.busterNumber);
        //플레이어 랭킹 이름 설정
        Player.GetComponent<PlaneBase>().profile.UpdateName(GameManager.playerName);
        //위치 설정   
        Player.transform.position = map.Random_Position();
        //Object Active
        Player.SetActive(true);
    }
    #endregion
    
    public void MinusAiCount()
    {
        aiCount--;
    }
}
