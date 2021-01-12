﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Holdable
{
    // private readonly int swapInTime;
    // private readonly int swapOutTime;
    // private readonly int weight; 
    // TODO: Item parent class to this?
    // TODO: Change to source/destinatio
    public abstract string getName();
    public abstract void primaryUsed(Vector3 source, Vector3 destination);
    //private abstract void secondaryUsed();
}
