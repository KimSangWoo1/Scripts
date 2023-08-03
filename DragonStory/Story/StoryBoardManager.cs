using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections.LowLevel.Unsafe;
public class StoryBoardManager : Singleton<StoryBoardManager>, ISubject
{
    [Header("SO")]
    public GameDataSO gameDataSO;
    public StorySO storySO;
    public StoryChannelSO storyChannelSO;
    public SceneChannelSO sceneChannelSO;
    public QusetManagerSO qusetManagerSO;
    [Header("���丮 ���� ������")]
    public List<Observer> observers;

    [Header("���丮 �뺻")]
    [SerializeField]
    private TextAsset story;
    public TextAsset Story
    {
        get
        {
            if (story == null)
            {
                ReadScript();
            }
            return story;
        }
    }
    
    [Header("���丮 ����")]
    public StoryBoard storyBoard;
    [Header("���� ����")]
    [SerializeField]
    private int storyStep = 0; //���� ���丮 �뺻 ���൵
    public Sequence sequence;

    private void OnEnable()
    {
        storyChannelSO.OnStoryRequested += StartNotify;
    }
    private void OnDisable()
    {
        storyChannelSO.OnStoryRequested -= StartNotify;
    }
    public void NextSequence()
    {
        if (storyStep < storyBoard.Sequence.Count)
        {
            sequence = storyBoard.GetSequence(storyStep);
            SequenceNotify();
            storyStep++;
        }
        else
        {
            EndNotify();
        }
    }

    //JSON���� ��ũ��Ʈ �б� - Inpectorâ���� ����     
    public void ReadScript()
    {
        string str = story.text;
        storyBoard = JsonUtility.FromJson<StoryBoard>(str);
        
        for (int i = storyBoard.Sequence.Count - 1; i >= 0; i--)
        {
            sequence = storyBoard.GetSequence(i);

            //Actor Setting
            if(Actor.NONE != (Actor)System.Enum.Parse(typeof(Actor), sequence.Cast.Actor))
            {
                sequence.Cast.actor = (Actor)System.Enum.Parse(typeof(Actor), sequence.Cast.Actor);
                sequence.Cast.emotion = (Emotion)System.Enum.Parse(typeof(Emotion), sequence.Cast.Emotion);
                sequence.Cast.act = (Act)System.Enum.Parse(typeof(Act), sequence.Cast.Act);
            }
            //Cam Setting
            sequence.Cinematography.shot = (Shot)System.Enum.Parse(typeof(Shot), sequence.Cinematography.Shot);

            //Auido Setting
            if (Actor.NONE != (Actor)System.Enum.Parse(typeof(Actor), sequence.Audio.Speaker))
            {
                sequence.Audio.speaker = (Actor)System.Enum.Parse(typeof(Actor), sequence.Audio.Speaker);
            }
            //Sequense Save
            storyBoard.Sequence[i] = sequence;
        }
    }

    #region ISubject - Method
    public void AddObserver(Observer observer)
    {
        observers.Add(observer);
    }

    public void RemoveObserver(Observer observer)
    {
        observers.Remove(observer);
    }

    // Script Camera Cast Audio ����
    public void StartNotify(Chapter _chapter, Part _part)
    {
        storyStep = 0;

        ChapterSO chapterSO = storySO.GetChpaterSO(_chapter, _part);
        story = chapterSO.story; //�뺻 �غ�
        
        ReadScript();

        observers[0].OnStartNotify();

        if (chapterSO.camGroup != null)
        {
            observers[1].OnStartNotify(chapterSO.castGroup); //���� ĳ���� �غ�
        }
        if(chapterSO.castGroup != null)
        {
            observers[2].OnStartNotify(chapterSO.camGroup); // ī�޶� ���� �غ�
        }

        NextSequence();
    }

    public void SequenceNotify()
    {
        for(int i=0; i< observers.Count; i++)
        {
            observers[i].OnNotify(sequence);
        }
    }

    public void EndNotify()
    {
        for (int i = 0; i < observers.Count; i++)
        {
            observers[i].OnStoryEnd();
        }
        if (gameDataSO.saveData.chapter == Chapter.THREE)
        {
            sceneChannelSO.RaiseEvent("GameScene");
        }
        qusetManagerSO.OnStoryQuestBeginning(UnsafeUtility.EnumToInt<Chapter>(gameDataSO.saveData.chapter), UnsafeUtility.EnumToInt<Part>(gameDataSO.saveData.part));

    }
    #endregion
}

