using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventData : IEventData 
{
    #region
    public struct HpData : IEventData
    {
        public int Hp;
        public int MaxHp;

        public HpData (int hp, int maxHp)
        {
            Hp = hp;
            MaxHp = maxHp;
        }
    }

    public struct MpData : IEventData
    {
        public int Mp;
        public int MaxMp;

        public MpData(int mp, int maxMp)
        {
            Mp = mp;
            MaxMp = maxMp;
        }
    }

    public struct DashData : IEventData
    {



    }

    public struct AddAbilityData : IEventData
    {
        public int Id;

        public AddAbilityData(int id)
        {
            Id = id;
        }
    }

    public struct UpdateStatusData : IEventData
    {
        public eAbType AbType;
        public int Value;

        public UpdateStatusData(eAbType abType, int value)
        {
            AbType = abType;
            Value = value;
        }
    }

    public struct UpdateSkillData : IEventData
    {
        public int Id;
        public eSkillType SkillType;
        public eSkillAbType SkillAbType;
        public int Value;

        public UpdateSkillData(int id, eSkillType skillType, eSkillAbType skillAbType, int value)
        {
            Id = id;
            SkillAbType = skillAbType;
            SkillType = skillType;
            Value = value;
        }
    }

    public struct SecondStatusData : IEventData
    {
        public eAbType AbType;
        public int Value;
        public float Second;

        public SecondStatusData(eAbType abType, int value, float second)
        {
            AbType = abType;
            Value = value;
            Second = second;
        }
    }

    public struct SecondSkillData : IEventData
    {
        public int Id;
        public eSkillType SkillType;
        public eSkillAbType SkillAbType;
        public int Value;
        public float Second;

        public SecondSkillData(int id, eSkillType skillType, eSkillAbType skillAbType, int value, float second)
        {
            Id = id;
            SkillType = skillType;
            SkillAbType = skillAbType;
            Value = value;
            Second = second;
        }
    }

    public struct CoolTimeData: IEventData
    {
        public KeyType KeyType;
        public float Value;

        public CoolTimeData(KeyType keyType, float value)
        {
            KeyType = keyType;
            Value = value;
        }
    }
    #endregion

    public struct StateChangeData : IEventData
    {
        public JToken SkillData;

        public StateChangeData(JToken skillData)
        {
            SkillData = skillData;
        }
    }
}
public interface IEventData
{

}