using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PathingHelper {
    protected const int TERRAIN_LAYER_MASK = 1 << 9;
    protected float size;
    public PathingHelper (float size) {
        this.size = size;
    }
    public abstract Vector3 getNextWaypoint(Vector3 source, Vector3 destination);
}
