using System;
using UnityEngine;
using Newtonsoft.Json.Linq;
using UnityEditor;
using Unity.VisualScripting;

[Serializable]
public class SkillAbility : LvAbilityBase
{
    public eSkillType SkillType;
    public eSkillAbType SkillAbType;

    private eMainAbType mainAbType = eMainAbType.Skill;
    public override eMainAbType GetMainAbType()
    {
        return mainAbType;
    }
}
