using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NpcAbilityAddtiveProductSO", menuName = "ScriptableObjects/NpcAbilityAddtiveProductSO", order = 9998)]
public class NpcAbilityAddtiveProductSO : ScriptableObject
{
    public NpcAbilityAddtiveProduct _npcAbilityAddtiveProduct;
}

[System.Serializable]
public class NpcAbilityAddtiveProduct : NpcAbilityProduct
{
    public eAbType AbType;
    public int AbValue;
}