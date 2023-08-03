using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BusterGage : MonoBehaviour
{
    public BusterController busterController;
    private Image gage;

    private Color green;
    private Color red;
    private Color orange;

    private float currentGage;
    void Start()
    {
        gage = GetComponent<Image>();
        green = new Color(21f/255f, 233f/255f, 140f/255f);
        red = new Color(245f / 255f, 57f / 255f, 6f/255f);
        orange = new Color(245f / 255f, 182f / 255f, 7f / 255f);
        gage.color = green;
  
    }
    private void Update()
    {
        currentGage = busterController.Change_UI_Gage();
      
        //부스터 색 변화
        Change_Color();
        Change_Gage();
    }

    private void Change_Gage()
    {
        float amount = currentGage;
        gage.fillAmount = amount;
    }
    //부스터 색 변화
    private void Change_Color()
    {
        if (gage.fillAmount >= 0.7f)
        {
            gage.color = green;
        }else if(gage.fillAmount >= 0.3f)
        {
            gage.color = orange;
        }
        else
        {
            gage.color = red;
        }
    }

}
