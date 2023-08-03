using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireController : MonoBehaviour
{
    [Header("UI관련")]
    public RightPanel_Control rightPanel; //발사

    private MuzzleController muzzle;
    private MagazineController magazine;

    private float fireTime; // 발사 시간
    private float fireReloadTime; //재장전 시간
    private float fireWaitTime;  //발사 후 대기 시간
    private float fireSpeedTime; //발사 속도 시간
    private int fireCount; //총 발사 횟수

    private bool trigger; //총 발사

    private void Awake()
    {
        //발사 설정
        fireReloadTime = 2f;
        fireWaitTime = 0.2f;
        fireSpeedTime = 5f;
    }

    private void Start()
    {
        muzzle = GetComponent<MuzzleController>();
        magazine = GetComponent<MagazineController>();
    }
    // Update is called once per frame
    void Update()
    {
        //발사!!
        if (trigger)
        {
            Shooting();
        }
    }
    #region Player 발사 전용
    // Player : 발사 버튼 Trigger  
    public void Player_FireTrigger()
    {
        fireTime += Time.deltaTime * fireSpeedTime;

        if (fireTime >= fireReloadTime)
        {
            //부스터 사용 안하고 있을 때
            if (!rightPanel.buster)
            {
                // Mobile || PC
                if (rightPanel.fire && magazine.get_Fireable() || Input.GetKeyDown(KeyCode.RightShift) && magazine.get_Fireable())
                {
                    trigger = true;
                    magazine.Shot(); //탄창 탄알 사용
                }
            }
        }
    }
    #endregion

    #region AI 발사 전용
    // AI : 발사 Trigger
    public void AI_FireTrigger( )
    {
        fireTime += Time.deltaTime * fireSpeedTime;
        if (fireTime >= fireReloadTime)
        {   // Mobile || PC

            trigger = true;
            magazine.Shot(); //탄창 탄알 사용
        }
    }

    public bool IsRemainMagazine()
    {
        return magazine.get_Fireable();
    }
    #endregion
    // 발사 버튼 눌렀다. 발사!!
    private void Shooting()
    {
        if (fireCount >= muzzle.Get_BulletCount())
        {
            trigger = false;
            fireCount = 0;
            fireTime = 0f;
        }
        else
        {
            if (fireTime >= fireWaitTime)
            {
                muzzle.Fire(); //총구에서 발사
                fireCount++;
                fireTime = 0f;
            }
        }
    }

    public int Get_BulletCount()
    {
        return magazine.bulletCount;
    }
}
