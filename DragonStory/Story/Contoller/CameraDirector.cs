using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraDirector : Observer
{
    [SerializeField]
    private StoryCamGroup storyCamGroup;

    #region Observer Method
    public override void OnStartNotify(GameObject _object)
    {
        storyCamGroup = Instantiate(_object, transform).GetComponent<StoryCamGroup>();
    }

    public override void OnNotify(Sequence sequence)
    {
        if(Shot.TIMELINE != (Shot)System.Enum.Parse(typeof(Shot), sequence.Cinematography.Shot)){
            if(Shot.TITLE == (Shot)System.Enum.Parse(typeof(Shot), sequence.Cinematography.Shot))
            {
                StartCoroutine(Next(3.5f));
            }else if (Shot.DOLLY == (Shot)System.Enum.Parse(typeof(Shot), sequence.Cinematography.Shot))
            {
                StartCoroutine(Next(0.5f));
            }
            storyCamGroup.CameraShooting((Shot)System.Enum.Parse(typeof(Shot), sequence.Cinematography.Shot), sequence.Cinematography.Number);
        }
    }

    public override void OnStoryEnd()
    {
        if (storyCamGroup != null)
        {
            storyCamGroup.CameraOff();
        }
    }
    #endregion

    private IEnumerator Next(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        StoryBoardManager.Instance.NextSequence();
    }
}
