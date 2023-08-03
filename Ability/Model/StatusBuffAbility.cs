using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StatusBuffAbility : BuffAbilityBase
{
    public eAbType AbType;
    [SerializeField] private eBuffType _buffType = eBuffType.Status;
    public override eBuffType GetBuffType() => _buffType;
}
