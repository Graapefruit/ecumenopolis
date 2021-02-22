using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class Item : ScriptableObject
{
    // private readonly int swapInTime;
    // private readonly int swapOutTime;
    // private readonly int weight; 
    // TODO: Item parent class to this?
    // TODO: Change to source/destinatio
    new public string name;
    public Vector3 pickupOffset;
    public Vector3 pickupRotation;
    public GameObject prefab;
    public Sprite avatar;
    public PlayerCharacterHoldTransform holdTransform;
    public string getName() {
        return this.name;
    }
    public Sprite getAvatar() {
        return this.avatar;
    }
    public abstract void primaryUsed(Shooter shooter, Vector3 source, Vector3 direction);
    //private abstract void secondaryUsed();
}
