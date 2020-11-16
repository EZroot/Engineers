using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPowered
{
    void SetPower(bool isPowered);
    bool IsPowered();
}
