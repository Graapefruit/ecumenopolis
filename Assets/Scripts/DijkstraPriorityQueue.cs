using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DijkstraPriorityQueue{

    private class Tuple<TFirst, TSecond> {
        public TFirst first;
        public TSecond second;
        public Tuple(TFirst f, TSecond s) {
            first = f;
            second = s;
        }
    }

    Tuple<int, float>[] minHeap;
    int[] vertexIndexToHeapIndex;

    public DijkstraPriorityQueue(int size) {
        minHeap = new Tuple<int, float>[size];
        vertexIndexToHeapIndex = new int[size];
        for(int i = 0; i < size; i++) {
            minHeap[i] = new Tuple<int, float>(i, Mathf.Infinity);
            vertexIndexToHeapIndex[i] = i;
        }
    }

    public void decreaseValue(int vertexIndex, float newValue) {
        int currentIndex = vertexIndexToHeapIndex[vertexIndex];
        minHeap[currentIndex] = new Tuple<int, float>(minHeap[currentIndex].first, newValue);
        while(currentIndex != 0 && parentIsLarger(currentIndex)) {
            int parentIndex = (currentIndex-1)/2;
            swap(currentIndex, parentIndex);
            currentIndex = parentIndex;
        }
    }

    public int popSmallest() {
        int currentIndex = 0;
        int smallest = minHeap[currentIndex].first;
        minHeap[currentIndex] = new Tuple<int, float>(minHeap[currentIndex].first, Mathf.Infinity);
        while (hasChildren(currentIndex)) {
            int smallerChild = getSmallerChild(currentIndex);
            swap(currentIndex, smallerChild);
            currentIndex = smallerChild;
        }
        return smallest;
    }

    private int getSmallerChild(int parentIndex) {
        int childIndex1 = (parentIndex * 2) + 1;
        int childIndex2 = (parentIndex * 2) + 2;
        if (childIndex2 >= minHeap.Length) {
            return childIndex1;
        } else {
            return (minHeap[childIndex1].second <= minHeap[childIndex2].second ? childIndex1 : childIndex2);
        }

    }

    private bool hasChildren(int currentIndex) {
        return (currentIndex * 2) + 1 < minHeap.Length;
    }

    private bool parentIsLarger(int idx) {
        return minHeap[idx].second < minHeap[(idx-1)/2].second;
    }

    private void swap(int idx1, int idx2) {
        int vertexSwap = vertexIndexToHeapIndex[minHeap[idx2].first];
        vertexIndexToHeapIndex[minHeap[idx2].first] = vertexIndexToHeapIndex[minHeap[idx1].first];
        vertexIndexToHeapIndex[minHeap[idx1].first] = vertexSwap;
        Tuple<int, float> heapSwap = minHeap[idx1];
        minHeap[idx1] = minHeap[idx2];
        minHeap[idx2] = heapSwap;
    }
}