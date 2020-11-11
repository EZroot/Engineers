using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractive
{
    bool IsOn { get; set; }
    void On();
    void Off();
}
