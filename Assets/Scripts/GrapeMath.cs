using UnityEngine;

// TODO:
// 1. Get inverse of Normal Distribution CDF
// 2. Get random float between 0-1 on it
public static class GrapeMath {
    // 1 = a/l + a/r
    // 1 = (la + ra)/lr
    // lr = (l + r)a
    // lr/(l + r) = a
    // breakpoint = a/l
    // breakpoint = lr / l(l + r)

    // distances are squared to accentuate large distances
    public static bool goLeftAroundEdge(float leftDist, float rightDist) {
        leftDist *= leftDist;
        rightDist *= rightDist;
        float breakpoint = (leftDist * rightDist) / (leftDist * (leftDist + rightDist));
        float r = Random.Range(0.0f, 1.0f);
        return r <= breakpoint;
    }
}