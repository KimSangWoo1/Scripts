using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;

[Serializable]
public class PlayerStatus :IData
{
    // 기본 스탯 Value
    [SerializeField]
    private int _hp;
    [SerializeField]
    private int _mp;

    public int Hp
    {
        get
        {
            return _hp;
        }
        set
        {
            _hp = Math.Clamp(value, 0, MaxHp);
            EventManager.Instance.Notify(EventType.HPChanged, new EventData.HpData(_hp,MaxHp));
        }
    }

    public int Mp
    {
        get => _mp;
        set
        { 
            _mp = Math.Clamp(value, 0, MaxMp);
            EventManager.Instance.Notify(EventType.MPChanged, new EventData.MpData(_mp, MaxMp));
        }
    }

    [field: SerializeField] public int MaxHp { get; set; }                        // 최대 체력
    [field: SerializeField] public int MaxMp { get; set; }                        // 최대 마나
    [field: SerializeField] public int HpRegeneration { get; set; }               // 체력 회복력
    [field: SerializeField] public int MpRegeneration { get; set; }               // 마나 회복력

    [field: SerializeField] public int Power { get; set; }                       // 공격력
    [field: SerializeField] public int Damage { get; set; }                       // 피해량

    [field: SerializeField] public int Defense { get; set; }                      // 방어력
    [field: SerializeField] public int MoveSpeed { get; set; }                    // 이동속도
    [field: SerializeField] public int DashSpeed { get; set; }                    // 대쉬 속도
    [field: SerializeField] public int AttackSpeed { get; set; }                  // 공격 속도
    [field: SerializeField] public int ActionSpeed { get; set; }                  // Ani 속도 (스킬 시전 속도)

    // 기본 스탯 Percent
    [field: SerializeField] public float HpRate { get; set; }                     // 체력 증가율
    [field: SerializeField] public float MpRate { get; set; }                     // 마나 증가율
    [field: SerializeField] public float MaxHpRate { get; set; }                  // 최대 체력%
    [field: SerializeField] public float MaxMpRate { get; set; }                  // 최대 마나%
    [field: SerializeField] public float HpRegenerationRate { get; set; }         // 체력 회복률
    [field: SerializeField] public float MpRegenerationRate { get; set; }         // 마나 회복률 

    [field: SerializeField] public float PowerRate { get; set; }                  // 기본 공격력%
    [field: SerializeField] public float DamageRate { get; set; }                 // 피해량%

    [field: SerializeField] public float DefenseRate { get; set; }                // 방어%
    [field: SerializeField] public float MoveSpeedRate { get; set; }              // 이동속도%
    [field: SerializeField] public float AttackSpeedRate { get; set; }            // 공격 속도%
    [field: SerializeField] public float ActionSpeedRate { get; set; }            // 스킬 시전 속도%

    [field: SerializeField] public float CriticalRate { get; set; }               // 치명타 확률
    [field: SerializeField] public float CriticalDamageRate { get; set; }         // 치명타 데미지%
    [field: SerializeField] public float AvoidanceRate { get; set; }              // 회피%
    [field: SerializeField] public float LeechLifeRate { get; set; }             // 흡혈%  

    private CancellationTokenSource _cts;
    public CancellationTokenSource Cts { get => _cts; set => _cts = value; }

    #region Update
    public void UpdateStatus(IEventData eventData)
    {
        var abilityData = (EventData.UpdateStatusData)eventData;
        eAbType abType = abilityData.AbType;
        int value = abilityData.Value;

        UpdateStatus(abType, value);
    }

    public void UpdateStatus(eAbType abType, int value)
    {
        switch (abType)
        {
            case eAbType.Hp:                        // 체력
                Hp += value;
                break;
            case eAbType.Mp:                        // 마나
                Mp += value;
                break;
            case eAbType.HpRegeneration:            // 체력 회복력
                HpRegeneration += value;
                break;
            case eAbType.MpRegeneration:            // 마나 회복력
                MpRegeneration += value;
                break;
            case eAbType.Power:                     // 기본 공격력 (힘)
                Power += value;
                break;
            case eAbType.Damage:                    // 피해량
                Damage += value;
                break;
            case eAbType.Defense:                   // 방어력
                Defense += value;
                break;
            case eAbType.MoveSpeed:                 // 이동속도
                MoveSpeed += value;
                break;
            case eAbType.DashSpeed:                 // 대시 속도
                DashSpeed += value;
                break;
            case eAbType.AttackSpeed:               // 공격속도
                AttackSpeed += value;
                break;
            case eAbType.ActionSpeed:               // 스킬속도
                ActionSpeed += value;
                break;
            case eAbType.MaxHp:                     // 최대 체력
                MaxHp += value;
                break;
            case eAbType.MaxMp:                     // 최대 마나
                MaxMp += value;
                break;
            case eAbType.HpRate:                    // 체력 증가율
                HpRate += value;
                break;
            case eAbType.MpRate:                    // 마나 증가율
                MpRate += value;
                break;
            case eAbType.HpRegenerationRate:        // 체력 회복률
                HpRegenerationRate += value;
                break;
            case eAbType.MpRegenerationRate:        // 마나 회복률 
                MpRegenerationRate += value;
                break;
            case eAbType.DamageRate:                // 피해 데미지%
                DamageRate += value;
                break;
            case eAbType.DefenseRate:               // 방어%
                DefenseRate += value;
                break;
            case eAbType.MoveSpeedRate:             // 이동속도%
                MoveSpeedRate += value;
                break;
            case eAbType.AttackSpeedRate:           // 공격 속도%
                AttackSpeedRate += value;
                break;
            case eAbType.ActionSpeedRate:           // 스킬 시전 속도%
                ActionSpeedRate += value;
                break;
            case eAbType.CriticalRate:              // 치명타 확률
                CriticalRate += value;
                break;
            case eAbType.CriticalDamageRate:        // 치명타 데미지%
                CriticalDamageRate += value;
                break;
            case eAbType.AvoidanceRate:             // 회피%
                AvoidanceRate += value;
                break;
            case eAbType.LeechLifeRate:             // 흡혈%  
                LeechLifeRate += value;
                break;
            case eAbType.MaxHpRate:                 // 최대 체력%
                MaxHpRate += value;
                break;
            case eAbType.MaxMpRate:                 // 최대 마나%
                MaxMpRate += value;
                break;
        }
    }

    #endregion

    #region Second
    public void SecondUpdateStatus(IEventData eventData)
    {
        var abilityData = (EventData.SecondStatusData)eventData;
        eAbType abType = abilityData.AbType;
        int value = abilityData.Value;
        float second = abilityData.Second;

        AsyncShortTimeUpdateStatus(abType, value, second).Forget();
    }

    private async UniTaskVoid AsyncShortTimeUpdateStatus(eAbType abType, int value, float second)
    {
        UpdateStatus(abType, value);
        await UniTask.Delay(TimeSpan.FromSeconds(second), ignoreTimeScale: false);
        UpdateStatus(abType, -value);
    }
    #endregion

    #region Hp
    public void HpChanged()
    {
        if (Hp < MaxMp)
        {
            if (Cts != null) return;

            Cts = new CancellationTokenSource();

            RegenHP().Forget();
        }
    }

    private async UniTaskVoid RegenHP()
    {
        while (Hp < MaxHp)
        {
            Hp += HpRegeneration + (int)(HpRegeneration * HpRegenerationRate);
            await UniTask.Delay(TimeSpan.FromSeconds(1f), cancellationToken: Cts.Token);
        }

        Cts.Dispose();
        Cts = null;
    }
    #endregion

    #region Mp
    public void MpChanged()
    {
        if (Mp < MaxMp)
        {
            if (Cts != null) return;

            Cts = new CancellationTokenSource();

            RegenMP().Forget();
        }
    }

    private async UniTaskVoid RegenMP()
    {
        while (Mp < MaxMp)
        {
            Mp += (int)(MpRegeneration * MpRegenerationRate);
            await UniTask.Delay(TimeSpan.FromSeconds(1f), cancellationToken: Cts.Token);
        }

        Cts.Dispose();
        Cts = null;
    }
    #endregion
}
