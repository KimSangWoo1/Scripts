using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeartControl : MonoBehaviour
{
    public Image[] heartsParent;
    private Image[] hearts;

    private void Awake()
    {
        hearts = new Image[heartsParent.Length];
    }

    private void Start()
    {
        for (int i = 0; i < heartsParent.Length; i++)
        {
            hearts[i] = heartsParent[i].transform.GetChild(0).GetComponent<Image>();
            //초기 HP 3개 이외의 UI Off
            if (i == 3 || i == 4)
            {
                heartsParent[i].gameObject.SetActive(false);
                hearts[i].gameObject.SetActive(false);
            }
        }
    }
    //추가 체력
    public void AddHeart()
    {
        if (!heartsParent[3].gameObject.activeSelf)
        {
            heartsParent[3].gameObject.SetActive(true);
        }
        else if (!heartsParent[4].gameObject.activeSelf)
        {
            heartsParent[4].gameObject.SetActive(true);
        }
    }

    //추가 체력 삭제
    public void RemoveHeart()
    {
        if (heartsParent[4].gameObject.activeSelf)
        {
            heartsParent[4].gameObject.SetActive(false);
        }
        else if (heartsParent[3].gameObject.activeSelf)
        {
            heartsParent[3].gameObject.SetActive(false);
        }
    }

    //체력 회복,감소
    public void ChangeHP(int currentHP)
    {
        for (int i = 0; i < heartsParent.Length; i++)
        {
            if (i <= currentHP - 1)
            {
                hearts[i].gameObject.SetActive(true);
            }
            else
            {
                hearts[i].gameObject.SetActive(false);
            }
        }
    }
}