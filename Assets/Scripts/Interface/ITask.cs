using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITask
{
    string GetInfo();
    bool IsBroken();
    void OutlineTaskOn();
    void OutlineTaskOff();
    void Sabotage();
}
