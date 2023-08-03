using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections.LowLevel.Unsafe;

public class CastSupport : Observer
{
    [SerializeField]
    private StoryCastGroup storyCastGroup;

    public Face face;

    #region Observer Method
    public override void OnStartNotify(GameObject _object)
    {
        storyCastGroup  = Instantiate(_object, transform).GetComponent<StoryCastGroup>();
    }

    public override void OnNotify(Sequence sequence)
    {
        if ((Actor)System.Enum.Parse(typeof(Actor), sequence.Cast.Actor) != Actor.NONE)
        {
            //Actor Ready
             storyCastGroup.ActorReady(sequence.Cast.actor);

            //Emotion
            storyCastGroup.SetEmotion(face.GetFace(sequence.Cast.emotion));

            // ActionReady
            storyCastGroup.ActionReady(sequence.Cast.act);
        }
    }

    public override void OnStoryEnd()
    {
        if (storyCastGroup != null)
        {
            storyCastGroup.StoryEnd();
        }
    }
    #endregion
}
