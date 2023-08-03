using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StorySwitcher : Singleton<StorySwitcher>
{
    public StoryChannelSO storyChannelSO;
    public GameDataSO gameDataSO;

    private void OnEnable()
    {
        storyChannelSO.OnStoryRequested += StoryChange;
    }

    private void OnDisable()
    {
        storyChannelSO.OnStoryRequested -= StoryChange;
    }

    public void StoryChange(Chapter _chapter, Part _part)
    {
        Debug.Log("Story Progress : Ch." + _chapter +" - "+ _part);
        gameDataSO.SaveStory(_chapter, _part);
    }
}
