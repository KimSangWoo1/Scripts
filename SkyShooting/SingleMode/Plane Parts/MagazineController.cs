using UnityEngine;
using UnityEngine.UI;

public class MagazineController : MonoBehaviour
{
    public float chargeTime; //탄알 충전 시간
    public int bulletCount; //총알 잔여량

    private bool fireable; //발사 가능 여부

    public float chargeSpeed;

    private void OnEnable()
    {
        bulletCount = 3; //초기화
        chargeSpeed = 2f; // 기존 3f
    }

    void Update()
    {
        //탄알 1개 충전 시간 3초
        chargeTime += Time.deltaTime /chargeSpeed;

        chargeTime = Mathf.Clamp(chargeTime, 0f, 1f);
        bulletCount = Mathf.Clamp(bulletCount, 0, 3);
        
        Charge_Bullet(); //탄알 장전
        Ready_Bullet();
    }

    //발사
    public void Shot()
    {
        bulletCount -= 1;
    }
    //장전 시스템
    private void Charge_Bullet()
    {
        if (chargeTime >= 1f)
        {
            bulletCount += 1;
            chargeTime = 0f;
        }
    }

    //탄창 준비하기
    private void Ready_Bullet()
    {
        switch (bulletCount)
        {
            case 3:
                chargeTime = 0f;
                fireable = true;
                break;
            case 2:
                fireable = true;
                break;
            case 1:
                fireable = true;
                break;
            case 0:
                fireable = false;
                break;
            default:
                fireable = false;
                break;
        }
    }

    //사격 사용 가능 여부 GET Method
    public bool get_Fireable()
    {
        return fireable;
    }
}


