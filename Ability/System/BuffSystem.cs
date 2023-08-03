using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using Cysharp.Threading.Tasks;

[Serializable]
public class BuffSystem : IEvent
{
    [SerializeField]
    private Dictionary<eActConditionType, List<BuffAbilityBase>> _buffAbilityDic = new Dictionary<eActConditionType, List<BuffAbilityBase>>();

    [SerializeField]
    private List<BuffAbilityBase> _buffAbilityList = new();
    [SerializeField]
    private List<BuffAbilityBase> _activebuffAbilityList = new();

    public void Init()
    {
        eActConditionType actConditionType;
        for (int i = 1; i < Enum.GetValues(typeof(eActConditionType)).Length; i++)
        {
            actConditionType = (eActConditionType)(1 << i);
            _buffAbilityDic[actConditionType] = new List<BuffAbilityBase>();
        }
        // 두개 조건이 붙는 경우
        _buffAbilityDic[eActConditionType.Hp | eActConditionType.Damaged] = new List<BuffAbilityBase>();

        EventManager.Instance.Subscribe(EventType.HPChanged, OnEvent);
        EventManager.Instance.Subscribe(EventType.MPChanged, OnEvent);
        EventManager.Instance.Subscribe(EventType.Dash, OnEvent);
    }

    #region Add Remove
    public void TryAdd(BuffAbilityBase buffAbility)
    {
        List<BuffAbilityBase> buffAbilities = _buffAbilityDic[buffAbility.ActConditionType];

        if (!buffAbilities.Contains(buffAbility))
        {
            buffAbilities.Add(buffAbility);
            _buffAbilityList.Add(buffAbility);
        }
    }

    public void Remove(BuffAbilityBase buffAbility)
    {

    }

    #endregion

    public void OnEvent(EventType type, IEventData eventData)
    {
        switch (type)
        {
            case EventType.HPChanged:
                UpdateBuff(eActConditionType.Hp, eventData);
                break;
            case EventType.MPChanged:
                UpdateBuff(eActConditionType.Mp, eventData);
                break;
            case EventType.Dash:
                UpdateBuff(eActConditionType.DashUse, eventData);
                break;
            case EventType.Damaged:
                UpdateBuff(eActConditionType.Damaged, eventData);
                break;
        }
    }

    #region Buff
    public void UpdateBuff(eActConditionType actConditionType, IEventData eventData)
    {
        List<BuffAbilityBase> buffAbilities = _buffAbilityDic[actConditionType];
        if (buffAbilities != null && buffAbilities.Count>0)
        {
            switch (actConditionType)
            {
                case eActConditionType.Hp:
                    Debug.Log("1번");
                    var HpData = (EventData.HpData)eventData;
                    for (int i = 0; i < buffAbilities.Count; i++)
                    {
                        var buffAbility = buffAbilities[i] as StatusBuffAbility;
                        var conditionValue = GetConditionValue(buffAbility);
                        var buffValue = GetBuffValue(buffAbility);

                        if (IsHpMpBuff(buffAbility.CompareType, buffAbility.AbType, HpData.MaxHp, HpData.Hp, conditionValue))
                        {
                            if (!buffAbility.IsActive)
                            {
                                EventManager.Instance.Notify(EventType.UpdateStatus, new EventData.UpdateStatusData(buffAbility.AbType, buffValue));
                                BuffOn(actConditionType, buffAbility);
                            }
                        }
                        else
                        {
                            if (buffAbility.IsActive)
                            {
                                EventManager.Instance.Notify(EventType.UpdateStatus, new EventData.UpdateStatusData(buffAbility.AbType, buffValue));
                                BuffOff(actConditionType, buffAbility);
                            }
                        }
                    }
                    break;
                case eActConditionType.Mp:
                    var MpData = (EventData.MpData)eventData;
                    for (int i = 0; i < buffAbilities.Count; i++)
                    {
                        var buffAbility = buffAbilities[i] as StatusBuffAbility;
                        var conditionValue = GetConditionValue(buffAbility);
                        var buffValue = GetBuffValue(buffAbility);

                        if (IsHpMpBuff(buffAbility.CompareType, buffAbility.AbType, MpData.MaxMp, MpData.Mp, conditionValue))
                        {
                            if (!buffAbility.IsActive)
                            {
                                EventManager.Instance.Notify(EventType.UpdateStatus, new EventData.UpdateStatusData(buffAbility.AbType, buffValue));
                                BuffOn(actConditionType, buffAbility);
                            }
                        }
                        else
                        {
                            if (buffAbility.IsActive)
                            {
                                EventManager.Instance.Notify(EventType.UpdateStatus, new EventData.UpdateStatusData(buffAbility.AbType, buffValue));
                                BuffOff(actConditionType, buffAbility);
                            }
                        }
                    }
                    break;
                case eActConditionType.DashUse:
                    for (int i = 0; i < buffAbilities.Count; i++)
                    {
                        eBuffType buffType = buffAbilities[i].GetBuffType();
                        if (buffType == eBuffType.Status)
                        {
                            StatusBuffAbility statusBuffAbility = buffAbilities[i] as StatusBuffAbility;
                            StatusBuff(statusBuffAbility);
                        }
                        else
                        {
                            SkillBuffAbility skillBuffAbility = buffAbilities[i] as SkillBuffAbility;
                            SkillBuff(skillBuffAbility);
                        }
                    }
                    break;
                case eActConditionType.Hp | eActConditionType.Damaged:
                    Debug.Log("2번");
                    for (int i = 0; i < buffAbilities.Count; i++)
                    {
                        eBuffType buffType = buffAbilities[i].GetBuffType();
                        if (buffType == eBuffType.Status)
                        {
                            StatusBuffAbility statusBuffAbility = buffAbilities[i] as StatusBuffAbility;
                            StatusBuff(statusBuffAbility);


                        }
                        else
                        {
                            SkillBuffAbility skillBuffAbility = buffAbilities[i] as SkillBuffAbility;
                            SkillBuff(skillBuffAbility);
                        }
                    }
                    break;
                case eActConditionType.Damaged:
                    Debug.Log("3번");

                    break;
            }
        }
    }
     

    private void BuffOn(eActConditionType actConditionType, BuffAbilityBase buffAbility)
    {
        buffAbility.IsActive = true;

        _buffAbilityList.Remove(buffAbility);
        _activebuffAbilityList.Add(buffAbility);
    }

    private void BuffOff(eActConditionType actConditionType, BuffAbilityBase buffAbility)
    {
        buffAbility.IsActive = false;

        _activebuffAbilityList.Remove(buffAbility);
        _buffAbilityList.Add(buffAbility);
    }

    private bool IsHpMpBuff(eCompareType compareType, eAbType abType, int statusMaxPoint, int currentPoint, int ConditionValue)
    {
        float currentRate = (currentPoint / 100f) * statusMaxPoint;
        //Debug.Log($" {currentPoint / 100f} * {statusMaxPoint} = ..  {currentRate}??{ConditionValue}");
        if (CompareValues(compareType, currentRate, ConditionValue))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void StatusBuff(StatusBuffAbility statusBuffAbility)
    {
        var conditionValue = GetConditionValue(statusBuffAbility);
        var buffValue = GetBuffValue(statusBuffAbility);

        // 스킬 초기화 , Ab 능력 업
        // 
        if (statusBuffAbility.BuffActType == eBuffActType.Second)
        {
            EventManager.Instance.Notify(EventType.SecondUpdateStatus, new EventData.SecondStatusData(statusBuffAbility.AbType, buffValue, conditionValue));
        }
        else if (statusBuffAbility.BuffActType == eBuffActType.Probability)
        {
            if (MathUtils.Probibility(conditionValue))
                EventManager.Instance.Notify(EventType.UpdateStatus, new EventData.UpdateStatusData(statusBuffAbility.AbType, buffValue));
        }
        else
        {
            EventManager.Instance.Notify(EventType.UpdateStatus, new EventData.UpdateStatusData(statusBuffAbility.AbType, buffValue));
        }
    }

    private void SkillBuff(SkillBuffAbility skillBuffAbility)
    {
        var conditionValue = GetConditionValue(skillBuffAbility);
        var buffValue = GetBuffValue(skillBuffAbility);

        // 스킬 초기화 , Ab 능력 업
        // 
        if (skillBuffAbility.BuffActType == eBuffActType.Second)
        {
            EventManager.Instance.Notify(EventType.SecondUpdateSKill, new EventData.SecondSkillData(skillBuffAbility.Id, skillBuffAbility.SkillType, skillBuffAbility.SkillAbType, buffValue, conditionValue));
        }
        else if (skillBuffAbility.BuffActType == eBuffActType.Probability)
        {
            if (MathUtils.Probibility(conditionValue))
                EventManager.Instance.Notify(EventType.UpdateSkill, new EventData.UpdateSkillData(skillBuffAbility.Id, skillBuffAbility.SkillType,skillBuffAbility.SkillAbType, buffValue));
        }
        else
        {
            EventManager.Instance.Notify(EventType.UpdateSkill, new EventData.UpdateSkillData(skillBuffAbility.Id, skillBuffAbility.SkillType, skillBuffAbility.SkillAbType, buffValue));
        }
    }

    #endregion

    #region Get Value
    private int GetConditionValue(BuffAbilityBase buffAbility)
    {
        var value = buffAbility.ComparePosType switch
        {
            eComparePosType.Pre => buffAbility.GetValue(),
            eComparePosType.Post => buffAbility.ConditionValue
        };
        return value;
    }

    private int GetBuffValue(BuffAbilityBase buffAbility)
    {
        var value = buffAbility.ComparePosType switch
        {
            eComparePosType.Pre => buffAbility.ConditionValue,
            eComparePosType.Post => buffAbility.GetValue()
        };
        value *= (!buffAbility.IsActive ? 1 : -1);
        return value;
    }
    #endregion

    #region Compare
    private bool CompareValues(eCompareType compareType, float a, float b)
    {
        return compareType switch
        {
            eCompareType.Equal => a == b,
            eCompareType.Greater => a > b,
            eCompareType.Less => a < b,
            eCompareType.GreaterEqual => a >= b,
            eCompareType.LessEqual => a <= b,
            _ => false
        };
    }

    private bool CompareValues(eCompareType compareType, int a, int b)
    {
        return compareType switch
        {
            eCompareType.Equal => a == b,
            eCompareType.Greater => a > b,
            eCompareType.Less => a < b,
            eCompareType.GreaterEqual => a >= b,
            eCompareType.LessEqual => a <= b,
            _ => false
        };
    }
    #endregion

}
