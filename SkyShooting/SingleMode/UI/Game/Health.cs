using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    public Image hp;
    float current_HP;
    public void ChangeHP(int amount)
    {
        current_HP = amount / 100f;
        hp.fillAmount = current_HP;
    }
}
