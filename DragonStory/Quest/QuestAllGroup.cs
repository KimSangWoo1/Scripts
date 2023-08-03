using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "QuestAllGroup", menuName = "ScriptableObjects/QuestAllGroup", order =1)]
public class QuestAllGroup : ScriptableObjectSingleton<QuestAllGroup>
{
    public List<QuestSO> mainQuestList;
    public List<QuestSO> worldQuestList;
    public List<QuestSO> normalQuestList;


    public QuestSO GetMainQuest(string key)
    {
        if(mainQuestList.Exists(q=>q.AddressableGuid == key)){
            int index = mainQuestList.FindIndex(q => q.AddressableGuid == key);
            return mainQuestList[index];
        }
        return null;
    }


    public QuestSO GetWorldQuest(string key)
    {
        if (worldQuestList.Exists(q => q.AddressableGuid == key))
        {
            int index = worldQuestList.FindIndex(q => q.AddressableGuid == key);
            return worldQuestList[index];
        }
        return null;
    }


    public QuestSO GetNormalQuest(string key)
    {
        if (normalQuestList.Exists(q => q.AddressableGuid == key))
        {
            int index = normalQuestList.FindIndex(q => q.AddressableGuid == key);
            return normalQuestList[index];
        }
        return null;
    }
}
