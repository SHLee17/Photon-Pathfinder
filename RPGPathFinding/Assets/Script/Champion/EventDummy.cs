using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventDummy : Champion
{
    protected override void Start()
    {
        isMoving = false;
        isFindingSuccess = false;
    }
}
