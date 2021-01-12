using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PriorityQueueDescending<T> : PriorityQueue<T>
{
    protected override bool valueNotInPosition(float value, float newValue) {
        return newValue < value;
    }
}
