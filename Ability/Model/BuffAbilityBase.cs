using System;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class BuffAbilityBase : LvAbilityBase, IBuff
{
    [HideInInspector]
    public float PositionX;
    [HideInInspector]
    public float PositionY;
    [HideInInspector]
    public float SizeX;
    [HideInInspector]
    public float SizeY;

    public eBuffEffectType BuffEffectType;
    public eActConditionType ActConditionType;
    public eCompareType CompareType;
    public eComparePosType ComparePosType;
    public int ConditionValue;
    public eBuffActType BuffActType;

    public bool IsActive { get; set; }
    private eMainAbType mainAbType = eMainAbType.Buff;

    public override eMainAbType GetMainAbType()
    {
        return mainAbType;
    }

    public virtual eBuffType GetBuffType()
    {
        return default;
    }
}

public interface IBuff
{
    eBuffType GetBuffType();
}
