using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Test Class
/// </summary>
[System.Serializable]
public class PlayerSkill
{
    [SerializeField]
    private List<SkillData> _skillDatas = new List<SkillData>();

    [field: SerializeField] public int AttackDamage { get; set; }
    [field: SerializeField] public int SkillDamage { get; set; }

    [field: SerializeField] public int AttackDamageRate { get; set; }
    [field: SerializeField] public int SkillDamageRate { get; set; }

    [field: SerializeField] public int Sk1CoolTime { get; set; }                // ��ų1 ��Ÿ�� ��
    [field: SerializeField] public int Sk2CoolTime { get; set; }                // ��ų2 ��Ÿ�� ��
    [field: SerializeField] public int Sk3CoolTime { get; set; }                // ��ų3 ��Ÿ�� ��
    [field: SerializeField] public int Sk4CoolTime { get; set; }                // ��ų4 ��Ÿ�� ��
    [field: SerializeField] public int DashCoolTime { get; set; }               // �뽬 ��Ÿ�� ��
    [field: SerializeField] public int AllattackSkillTime { get; set; }         // ��ü ���� ��ų ��Ÿ�� �� (�⺻ ����, �뽬 ����)
    [field: SerializeField] public int AllSkillCoolTime { get; set; }           // ��ü ��ų ��Ÿ�� ��      (�⺻ ���� ����)

    [field: SerializeField] public int Sk1CoolTimeRate { get; set; }            // ��ų1 ��Ÿ��%
    [field: SerializeField] public int Sk2CoolTimeRate { get; set; }            // ��ų2 ��Ÿ��%
    [field: SerializeField] public int Sk3CoolTimeRate { get; set; }            // ��ų3 ��Ÿ��%
    [field: SerializeField] public int Sk4CoolTimeRate { get; set; }            // ��ų4 ��Ÿ��%
    [field: SerializeField] public int DashCoolTimeRate { get; set; }           // �뽬 ��Ÿ��%
    [field: SerializeField] public int AllAttackSkillTimeRate { get; set; }     // ��ü ���� ��ų ��Ÿ��% (�⺻ ����, �뽬 ����)
    [field: SerializeField] public int AllSkillCoolTimeRate { get; set; }       // ��ü ��ų ��Ÿ��%      (�⺻ ���� ����)

    /*
    [field: SerializeField] public int DashCoolTimeSlot { get; set; }           // ��ų1 ��Ÿ��Slot
    [field: SerializeField] public int Sk1CoolTimeSlot { get; set; }            // ��ų2 ��Ÿ��Slot
    [field: SerializeField] public int Sk2CoolTimeSlot { get; set; }            // ��ų3 ��Ÿ��Slot
    [field: SerializeField] public int Sk3CoolTimeSlot { get; set; }            // ��ų4 ��Ÿ��Slot
    [field: SerializeField] public int Sk4CoolTimeSlot { get; set; }            // �뽬 ��Ÿ��Slot
    [field: SerializeField] public int AllAttackSkillTimeSlot { get; set; }     // ��ü ���� ��ų ��Ÿ��Slot (�⺻ ����, �뽬 ����)
    [field: SerializeField] public int AllSkillCoolTimeSlot { get; set; }       // ��ü ��ų ��Ÿ��Slot      (�⺻ ���� ����)
    */

    public void UpdateSkill(IEventData eventData)
    {
        var skillAbData = (EventData.UpdateSkillData)eventData;
        if(skillAbData.SkillAbType != eSkillAbType.AddSkill)
        {
            UpdateAssistanceData(skillAbData.SkillAbType, skillAbData.Value);
        }
        else
        {
            AddSkill(skillAbData.Id);
        }
    }

    //Data ���� ���� �� ���
    private void UpdateSkillData(int id, eSkillType skillType, eSkillAbType skillAbType, int value)
    {
        var skillData = DataManager.Instance.GetSkill(id);

    }

    private void UpdateAssistanceData(eSkillAbType skillAbType, int value)
    {
        switch (skillAbType)
        {
            case eSkillAbType.AttackDamage:
                AttackDamage += value;
                break;
            case eSkillAbType.SkillDamage:
                SkillDamage += value;
                break;
            case eSkillAbType.AttackDamageRate:
                AttackDamageRate += value;
                break;
            case eSkillAbType.DashCoolTime:
                DashCoolTime += value;
                break;
            case eSkillAbType.SkillDamageRate:
                SkillDamageRate += value;
                break;
            case eSkillAbType.Sk1CoolTime:               
                Sk1CoolTime += value;
                break;
            case eSkillAbType.Sk2CoolTime:              
                Sk2CoolTime += value;
                break;
            case eSkillAbType.Sk3CoolTime:              
                Sk3CoolTime += value;
                break;
            case eSkillAbType.Sk4CoolTime:               
                Sk4CoolTime += value;
                break;
            case eSkillAbType.AllAttackSkillTime:
                AllattackSkillTime += value;
                break;
            case eSkillAbType.AllSkillCoolTime:          
                AllSkillCoolTime += value;
                break;
            case eSkillAbType.DashCoolTimeRate:              
                DashCoolTimeRate += value;
                break;
            case eSkillAbType.Sk1CoolTimeRate:
                Sk1CoolTimeRate += value;
                break;
            case eSkillAbType.Sk2CoolTimeRate:
                Sk2CoolTimeRate += value;
                break;
            case eSkillAbType.Sk3CoolTimeRate:
                Sk3CoolTimeRate += value;
                break;
            case eSkillAbType.Sk4CoolTimeRate:
                Sk4CoolTimeRate += value;
                break;
            case eSkillAbType.AllSkillCoolTimeRate:
                AllSkillCoolTimeRate += value;
                break;
            case eSkillAbType.AllAttackSkillTimeRate:
                AllAttackSkillTimeRate += value;
                break;
            case eSkillAbType.DashCoolTimeSlot:
                EventManager.Instance.Notify(EventType.CoolTimeChanged, new EventData.CoolTimeData(KeyType.Dash, value));
                break;
            case eSkillAbType.Sk1CoolTimeSlot:
                EventManager.Instance.Notify(EventType.CoolTimeChanged, new EventData.CoolTimeData(KeyType.Skill1, value));
                break;
            case eSkillAbType.Sk2CoolTimeSlot:
                EventManager.Instance.Notify(EventType.CoolTimeChanged, new EventData.CoolTimeData(KeyType.Skill2, value));
                break;
            case eSkillAbType.Sk3CoolTimeSlot:
                EventManager.Instance.Notify(EventType.CoolTimeChanged, new EventData.CoolTimeData(KeyType.Skill3, value));
                break;
            case eSkillAbType.Sk4CoolTimeSlot:
                EventManager.Instance.Notify(EventType.CoolTimeChanged, new EventData.CoolTimeData(KeyType.Skill4, value));
                break;
            case eSkillAbType.AllSkillCoolTimeSlot:
                EventManager.Instance.Notify(EventType.CoolTimeChanged, new EventData.CoolTimeData(KeyType.Dash, value));
                EventManager.Instance.Notify(EventType.CoolTimeChanged, new EventData.CoolTimeData(KeyType.Skill1, value));
                EventManager.Instance.Notify(EventType.CoolTimeChanged, new EventData.CoolTimeData(KeyType.Skill2, value));
                EventManager.Instance.Notify(EventType.CoolTimeChanged, new EventData.CoolTimeData(KeyType.Skill3, value));
                EventManager.Instance.Notify(EventType.CoolTimeChanged, new EventData.CoolTimeData(KeyType.Skill4, value));
                break;
            case eSkillAbType.AllAttackSkillTimeSlot:
                EventManager.Instance.Notify(EventType.CoolTimeChanged, new EventData.CoolTimeData(KeyType.Skill1, value));
                EventManager.Instance.Notify(EventType.CoolTimeChanged, new EventData.CoolTimeData(KeyType.Skill2, value));
                EventManager.Instance.Notify(EventType.CoolTimeChanged, new EventData.CoolTimeData(KeyType.Skill3, value));
                EventManager.Instance.Notify(EventType.CoolTimeChanged, new EventData.CoolTimeData(KeyType.Skill4, value));
                break;
        }
    }
    
    public void AddSkill(int id)
    {
#if UNITY_EDITOR
        SkillData skillData = ConvertToSkillData(id);
        _skillDatas.Add(skillData);
#else

#endif
    }

    public SkillData ConvertToSkillData(int id)
    {
        var jSkilData = DataManager.Instance.GetSkill(id);
        var asset = ScriptableObject.CreateInstance<SkillData>();
        asset.JSkillData = jSkilData;
        asset.ID = jSkilData["id"].Value<int>();
        asset.Name = jSkilData["name"].Value<string>();
        asset.Icon = Resources.Load<Sprite>($"SkillIcon/SkillIcon{asset.ID}");

        //asset.Position = new Vector2(jSkilData["posX"][0].Value<float>(), jSkilData["posY"][1].Value<float>());
        //asset.Size = new Vector2(jSkilData["sizeX"][0].Value<float>(), jSkilData["sizeY"][1].Value<float>());
        //asset.Damage = jSkilData["damage"].Value<int>();
        //asset.AttackCount = jSkilData["attackCount"].Value<int>();
        //asset.MaxTarget = jSkilData["maxTarget"].Value<int>();
        asset.CoolTime = jSkilData["coolTime"].Value<float>();
        asset.MPCost = jSkilData["mpCost"].Value<int>();
        //asset.HitEffectType = (HitEffectType)Enum.Parse(typeof(HitEffectType), jSkilData["hitEffectType"].Value<string>());
        asset.name = asset.Name;
        return asset;
    }
}
