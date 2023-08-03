
using UnityEngine;

public abstract class Observer :MonoBehaviour
{
   public abstract void OnNotify(Sequence _sequence);
   public abstract void OnStoryEnd();

    public virtual void OnStartNotify(GameObject _object) { }
    public virtual void OnStartNotify() {
    }
}
