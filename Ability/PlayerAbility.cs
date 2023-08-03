using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerAbility 
{
    [SerializeField]
    private List<StatusAbility> _statusAbilities = new List<StatusAbility>();
    [SerializeField]
    private List<SkillAbility> _skillAbilities = new List<SkillAbility>();
    [SerializeField]
    BuffSystem _buffSystem;

    public void Init()
    {
        _buffSystem = new BuffSystem();
        _buffSystem.Init();
    }

    public void AddPlayerAbility(IEventData eventData)
    {
        var abilityData = (EventData.AddAbilityData)eventData;
        var ability = DataManager.Instance.AbilityDataDic[abilityData.Id];
        eMainAbType mainAbType = ability.GetMainAbType();

        switch (mainAbType)
        {
            case eMainAbType.Status:
                StatusAbility statusAbility = ability as StatusAbility;
                if(!_statusAbilities.Contains(statusAbility))
                    _statusAbilities.Add(statusAbility);

                EventManager.Instance.Notify(EventType.UpdateStatus, new EventData.UpdateStatusData(statusAbility.AbType, statusAbility.GetValue()));
                break;
            case eMainAbType.Buff:
                BuffAbilityBase buffAbility = ability as BuffAbilityBase;
                _buffSystem.TryAdd(buffAbility);
                break;
            case eMainAbType.Skill:
                SkillAbility skillAbility = ability as SkillAbility;
                if (!_skillAbilities.Contains(skillAbility))
                    _skillAbilities.Add(skillAbility);

                EventManager.Instance.Notify(EventType.UpdateSkill, new EventData.UpdateSkillData(skillAbility.Id, skillAbility.SkillType, skillAbility.SkillAbType, skillAbility.GetValue()));
                break;
        }
    }
}
