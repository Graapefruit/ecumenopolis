using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pair {
    public int x;
    public int y;

    public Pair(int x, int y) {
        this.x = x;
        this.y = y;
    }

    public virtual bool equals(Pair pair) {
        return (this.x == pair.x) && (this.y == pair.y);
    }
}
