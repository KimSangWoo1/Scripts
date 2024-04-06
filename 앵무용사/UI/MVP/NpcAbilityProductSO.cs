using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;

[CreateAssetMenu (fileName = "NpcAbilityProductSO", menuName = "ScriptableObjects/NpcAbilityProductSO", order =9998)]
public class NpcAbilityProductSO : ScriptableObject
{
    public NpcAbilityProduct _npcAbilityProduct;
}

[System.Serializable]
public class NpcAbilityProduct
{
    public Sprite AbilityIcon;
    public LocalizedString LocalAbilityName;
    public LocalizedString LocalAbilityDescript;
    public int Money;
}