using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BusterController : MonoBehaviour
{
    [Header("UI관련")]
    public RightPanel_Control rightPanel; //발사

    private float value;

    private bool possible;
    public bool buster;
    void Start()
    {
        value = 1f;
    }

    void Update()
    {
        
        //부스터 사용 가능 여부
        if (value <= 0f)
        {
            possible = false;
        }
        else
        {
            possible = true;
        }
    }

    //Player용 부스터  Mobile && PC
    public void PlayerBusterControl()
    {
        //부스터 사용 & 충전 
        if (rightPanel.buster || buster)
        {
            //부스터 게이지 다 사용했는지 검사 
            if (possible)
            {
                DIsCharge_Gage();
            }
            else
            {
                buster = false;
                rightPanel.buster = false;
            }
        }
        else
        {
            buster = false;
            Charge_Gage();
        }
    }

    //AI용 부스터
    public void AI_Buster_Control()
    {
        //부스터 사용 & 충전 
        if (buster)
        {
            //부스터 게이지 다 사용했는지 검사 
            if (possible)
            {
                buster = true; // 부스터 소진하도록
                DIsCharge_Gage();
            }
            else
            {
                buster = false;
            }
        }
        else
        {
            buster = false;
            Charge_Gage();
        }
    }
    //부스터 게이지 충전
    public void Charge_Gage()
    {
        // Max 충전 10초 걸림
        value += Time.deltaTime / 10f;
        value = Mathf.Clamp(value, 0, 1f);    
        
    }
    //부스터 게이지 방전
    public void DIsCharge_Gage() {
        // Max 사용 5초 
        value -=  Time.deltaTime / 5f;
        value = Mathf.Clamp(value, 0, 1f);
    }
    //부스터 사용가능 GET Method
    public bool Get_Possible()
    {
        return possible;
    }
    //Player PC Mobile용
    public bool Get_BusterClick()
    {
        return rightPanel.buster;
    }

    // Player 부스터 게이지 UI 전용 메소드
    public float Change_UI_Gage()
    {
        return value;
    }

    //AI용
    public float Get_BusterGage()
    {
        return value;
    }
}
