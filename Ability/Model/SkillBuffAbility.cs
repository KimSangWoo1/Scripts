using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SkillBuffAbility : BuffAbilityBase
{
    public eSkillType SkillType;
    public eSkillAbType SkillAbType;

    [SerializeField] private eBuffType _buffType = eBuffType.Skill;
    public override eBuffType GetBuffType() => _buffType;
}
