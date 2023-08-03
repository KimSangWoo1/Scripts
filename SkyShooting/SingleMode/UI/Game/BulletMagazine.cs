using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class BulletMagazine : MonoBehaviour
{
    public MagazineController magazineController;

    [Header("탄창 총알 이미지들")]
    public Image bullet_One;
    public Image bullet_Two;
    public Image bullet_Three;

    private Color alpha_Half; //알파 반값
    private Color alpha_Full; //알파 풀값

    int bulletCount;
    int UI_Bullet;
    float chargeTime;

    private void OnEnable()
    {
        UI_Bullet = 3;
    }
    void Start()
    {  //이거 빼고
        alpha_Half = new Color(255f, 255f, 255f, 0.5f);
        alpha_Full = new Color(255f, 255f, 255f, 1f);
    }

    private void Update()
    {
        bulletCount = magazineController.bulletCount;
        chargeTime = magazineController.chargeTime;
        //총 쐈을 경우
        if(bulletCount < UI_Bullet)
        {
            Dischare_Bullet();
        }
        UI_Bullet = bulletCount;

        Charge_Bullet(); // UI 충전하기
        Bullet_Remain(); // 현재 상태 UI 그리기
    }
    //UI 탄알 장전
    private void Charge_Bullet()
    {
        switch (UI_Bullet)
        {
            case 3:
                bullet_One.fillAmount = 1f;
                break;
            case 2:
                bullet_One.fillAmount = chargeTime;
                bullet_Two.fillAmount = 1f;
                break;
            case 1:
                bullet_Two.fillAmount = chargeTime;
                bullet_Three.fillAmount = 1f;
                break;
            case 0:
                bullet_Three.fillAmount = chargeTime;
                break;
            default:
                break;
        }
    }

    //탄알 UI 알파 조절 
    private void Bullet_Remain()
    {
        switch (UI_Bullet)
        {
            case 3:
                bullet_One.color = alpha_Full;
                bullet_Two.color = alpha_Full;
                bullet_Three.color = alpha_Full;
                break;
            case 2:

                bullet_One.color = alpha_Half;
                bullet_Two.color = alpha_Full;
                bullet_Three.color = alpha_Full;
                break;
            case 1:

                bullet_One.color = alpha_Half;
                bullet_Two.color = alpha_Half;
                bullet_Three.color = alpha_Full;
                break;
            case 0:
                bullet_One.color = alpha_Half;
                bullet_Two.color = alpha_Half;
                bullet_Three.color = alpha_Half;
                break;
            default:
                break;
        }
    }

    // 탄알 사용 후 UI 처리
    private void Dischare_Bullet()
    {
        switch (UI_Bullet)
        {
            case 3:
                break;
            case 2:
                bullet_One.fillAmount = 0f;
                break;
            case 1:
                bullet_One.fillAmount = 0f;
                bullet_Two.fillAmount = 0f;
                break;
            case 0:
                bullet_One.fillAmount = 0f;
                bullet_Two.fillAmount = 0f;
                bullet_Three.fillAmount = 0f;
                break;
            default:
                break;
        }
    }
}
