using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Knight : Champion
{

    protected override void Start()
    {
        base.Start();
        star = 1;
        Range = 0;
    }


}
