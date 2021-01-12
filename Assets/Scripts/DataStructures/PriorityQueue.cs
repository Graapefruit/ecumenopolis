using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PriorityQueue<T> {
    private List<Tuple<T, float>> queue;
    public PriorityQueue() {
        this.queue = new List<Tuple<T, float>>();
    }

    protected abstract bool valueNotInPosition(float value, float newValue);

    // TODO: Binary Insert
    public void add(T key, float value) {
        int i = 0;
        while (i < this.queue.Count && valueNotInPosition(this.queue[i].second, value)) {
            i++;
        }
        this.queue.Insert(i, new Tuple<T, float>(key, value));
    }

    public bool empty() {
        return this.queue.Count == 0;
    }

    public Tuple<T, float> peek() {
        return this.queue[0];
    }

    public T pop() {
        T toReturn = this.queue[0].first;
        this.queue.RemoveAt(0);
        return toReturn;
    }
}
