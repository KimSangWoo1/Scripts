using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingManager : Singleton<LoadingManager>
{
    public Image lodingPanel;
    public Text percentText;


    private void OnEnable()
    {
        //percentText.text = "Loding";
    }

    public void Get_Operation(float num)
    {
        num = num * 100f;
        //percentText.text = "Loding "+num;
    }

    public void StartLoading()
    {
        lodingPanel.gameObject.SetActive(true);
    }

    public void EndLoading()
    {
        lodingPanel.gameObject.SetActive(false);
    }
}

