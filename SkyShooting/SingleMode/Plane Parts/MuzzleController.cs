using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MuzzleController : MonoBehaviour
{
    BulletManager BM; //총알Manager

    Profile profile;
    // [Header("Player 총구 위치")]
    private Transform leftMuzzle;
    private Transform rightMuzzle;
    private Transform centerMuzzle;

    private GameObject bullet1;
    private GameObject bullet2;
    private GameObject bullet3;

    enum Muzzle_Number {one, two, three}
    Muzzle_Number current_Muzzle;

    private int muzzle_Count; //총구 갯수
    private int bullet_Count; //총알 갯수

    private void Awake()
    {
        
    }

    private void OnEnable()
    {
        init(); //초기 설정
    }
    void Start()
    {
        BM = BulletManager.Instance;
        profile = transform.parent.GetComponent<PlaneBase>().profile;

        leftMuzzle = gameObject.transform.GetChild(0);
        rightMuzzle = gameObject.transform.GetChild(1);
        centerMuzzle = gameObject.transform.GetChild(2);

    }

    void Update()
    {
        // 총구 갯수 최소 최대 1~3
        muzzle_Count = Mathf.Clamp(muzzle_Count, 1, 3);
        // 총알 갯수 최소 최대 1~3
        bullet_Count = Mathf.Clamp(bullet_Count, 1, 3);

        Muzzle_Setting(); //총구 설정
        Muzzle_Update();  //총구 변경
    }

    //초기 설정
    private void init()
    {
        muzzle_Count = 1;
        bullet_Count = 1;

        current_Muzzle = Muzzle_Number.one;
    }
    //총구 설정
    private void Muzzle_Setting()
    {
        switch (muzzle_Count)
        {
            case 1:
                current_Muzzle = Muzzle_Number.one;
                break;
            case 2:
                current_Muzzle = Muzzle_Number.two;
                break;
            case 3:
                current_Muzzle = Muzzle_Number.three;
                break;
            default:
                break;
        }   
    }
    //총구 변경
    private void Muzzle_Update()
    {
        //총구 갯수 별 총구 활성화
        switch (current_Muzzle)
        {
            case Muzzle_Number.one:
                centerMuzzle.gameObject.SetActive(true);
                leftMuzzle.gameObject.SetActive(false);
                rightMuzzle.gameObject.SetActive(false);
                break;
            case Muzzle_Number.two:
                centerMuzzle.gameObject.SetActive(false);
                leftMuzzle.gameObject.SetActive(true);
                rightMuzzle.gameObject.SetActive(true);
                break;
            case Muzzle_Number.three:
                centerMuzzle.gameObject.SetActive(true);
                leftMuzzle.gameObject.SetActive(true);
                rightMuzzle.gameObject.SetActive(true);
                break;
        }
    }

    //발사
    public void Fire()
    {
        //총구 갯수 별 총구에서 총알 발사
        switch (current_Muzzle)
        {
            case Muzzle_Number.one:
                bullet1 = BM.bullet_Fire(centerMuzzle);
                
                bullet1.GetComponent<BulletController>().Set_PlayerProfile(profile);
                break;
            case Muzzle_Number.two:
                bullet1 = BM.bullet_Fire(leftMuzzle);
                bullet2 = BM.bullet_Fire(rightMuzzle);

                bullet1.GetComponent<BulletController>().Set_PlayerProfile(profile);
                bullet2.GetComponent<BulletController>().Set_PlayerProfile(profile);
                break;
            case Muzzle_Number.three:
                bullet1 = BM.bullet_Fire(centerMuzzle);
                bullet2 = BM.bullet_Fire(leftMuzzle);
                bullet3 = BM.bullet_Fire(rightMuzzle);

                bullet1.GetComponent<BulletController>().Set_PlayerProfile(profile);
                bullet2.GetComponent<BulletController>().Set_PlayerProfile(profile);
                bullet3.GetComponent<BulletController>().Set_PlayerProfile(profile);
                break;
        }
    }

    //총구 증가
    public void Add_Muzzle()
    {
        muzzle_Count += 1;
    }
    //총알 증가
    public void Add_Bullet()
    {
        bullet_Count += 1;
    }

    //총알 갯수 Get Method
    public int Get_BulletCount()
    {
        return bullet_Count;
    }
}
