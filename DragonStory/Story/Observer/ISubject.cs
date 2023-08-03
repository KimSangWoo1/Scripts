using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISubject 
{
    void AddObserver(Observer observer);

    void RemoveObserver(Observer observer);

    void StartNotify(Chapter chapter, Part part);
    void SequenceNotify();

    void EndNotify();
}
