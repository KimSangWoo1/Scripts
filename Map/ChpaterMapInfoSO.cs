using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "ChpaterMapInfoSO", menuName = "ScriptableObjects/ChpaterMapInfoSO", order = 1)]
public class ChpaterMapInfoSO : ScriptableObject
{
    public int Chapter;
    public int RoomCount;
    public GameObject ChapterRoom;
    public RuleTile[] ChpaterRuleTiles;
    public ChapterRoomInfo ChapterRoomInfo;
    public List<GameObject> Props;
    public List<GameObject> Trabs;
    //public List<GameObject> Monsters; // 방마다 몬스터 정해지면 필요 없음
    public List<EventRoomInfo> EventRooms;
    public List<MonsterRoomInfo> MonsterRooms;
    public List<SpecialRoomInfo> SpecialRoomInfos;

    public List<GameObject> GetMonsters(eMonsterRoomType monsterRoomType)
    {
        var monsterRoomInfo = MonsterRooms.FirstOrDefault(r => r.monsterRoomType == monsterRoomType);
        return monsterRoomInfo.Monster;
    }
}

[System.Serializable]
public struct ChapterRoomInfo
{
    public int PropCostMin;
    public int PropCostMax;
    public int TrapCostMin;
    public int TrapCostMax;
    public int MonsterCostMin;
    public int MonsterCostMax;
}

[System.Serializable]
public class EventRoomInfo 
{
    public eEventRoomType EventRoomType;
    public List<GameObject> Npcs;
    public List<GameObject> Props;
}

[System.Serializable]
public class SpecialRoomInfo
{
    public eSpecialRoomType SpecialRoomType;
    public eEventRoomType CriteriaRoomType;
    public eMonsterRoomType monsterRoomType;
    public int Distance;
    public float RandomValue; // 0 ~ 100
    [SerializeField]
    private bool isIncluded;
    public bool IsIncluded { get => isIncluded; set => isIncluded = value; }
    public List<MonsterCreateInfo> SpecialMonsters;
}

[System.Serializable]
public class MonsterRoomInfo 
{
    public eMonsterRoomType monsterRoomType;
    public List<GameObject> Monster;
    public int RoomCreateCount;
}

[System.Serializable]
public class MonsterCreateInfo
{
    public GameObject Monster;
    public int Count;
}

public enum eRoomType
{
    Normal, // 기본 전투방
    Event,  // 전투 없는 방
    Special // 특정 몬스터가 포함된 전투 방 ( 추가방 개념)
}

public enum eEventRoomType
{
    None,
    NPC1,
    NPC2,
    NPC3,
    NPC4,
    NPC5,
    NPC6,
    NPC7,
    Start,
    BossEntrance
}

public enum eSpecialRoomType
{
    None,
    SubBoss,
    Gold
}

public enum eMonsterRoomType
{
    None,
    Easy,
    Normal,
    Hard,
    Crazy
}

public enum eRoomLevelType
{
    None,
    Easy,
    Normal,
    Hard
}
