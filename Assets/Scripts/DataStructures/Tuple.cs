using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tuple<T, Y> {
    public T first;
    public Y second;

    public Tuple(T first, Y second) {
        this.first = first;
        this.second = second;
    }
}