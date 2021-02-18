using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item : ScriptableObject
{
    // private readonly int swapInTime;
    // private readonly int swapOutTime;
    // private readonly int weight; 
    // TODO: Item parent class to this?
    // TODO: Change to source/destinatio
    new public string name;
    public string getName() {
        return this.name;
    }
    public abstract void primaryUsed(Shooter shooter, Vector3 source, Vector3 direction);
    //private abstract void secondaryUsed();
}
