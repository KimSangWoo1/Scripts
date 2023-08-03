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
        //말하는 사람 소리
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

        // 이팩트 사운드
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
