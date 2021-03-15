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

    // https://stackoverflow.com/questions/5837572/generate-a-random-point-within-a-circle-uniformly
    // At ridiculously high radiuses, this probably gets inefficient as the circle "wraps around" the player, decreasing the probability of uniformly random point
    // Switch to random point in a section of a sphere
    public static Vector2 getRandomLocationInSphere(float radius) {
        float randomRadius = radius * Mathf.Sqrt(Random.Range(0.0f, 1.0f));
        float randomTheta = Random.Range(0.0f, 1.0f) * 2 * Mathf.PI;
        float x = Mathf.Cos(randomTheta) * randomRadius;
        float y = Mathf.Sin(randomTheta) * randomRadius;
        Debug.LogFormat("x: {0}, y: {1}", x, y);
        return new Vector2(x, y);
    }
}