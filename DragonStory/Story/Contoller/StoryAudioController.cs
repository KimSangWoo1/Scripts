using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoryAudioController : Observer
{
    public Speaker speaker;

    public AudioSource speakerAudioSource;
    private AudioClip clip;

    #region Observer Method
    public override void OnNotify(Sequence sequence)
    {
        //���ϴ� ��� �Ҹ�
        if (Actor.NONE != (Actor)System.Enum.Parse(typeof(Actor), sequence.Audio.Speaker))
        {
            clip = speaker.GetSpeakerSound(sequence.Audio.speaker);
        }

        //BGM
        if(sequence.Audio.BackgroundPlaying != true)
        {
            if(sequence.Audio.BackgroundSoundNumber >= 0)
            {

            }
        }

        // ����Ʈ ����
        //if(sequence.Audio.EffectSound)
    }

    public override void OnStoryEnd()
    {
        //BGM Off

        //Effect Sound Off
    }
    #endregion
    public void VoiceSetting(Actor actor)
    {
        if (Actor.NONE != actor)
        {
            clip = speaker.GetSpeakerSound(actor);
        }
    }
    public void Speaking()
    {
        speakerAudioSource.PlayOneShot(clip);
    }
}
