using UnityEngine;
using System.Collections;
using System;

public class Heap<T> where T : Node
{

    T[] items;
    int currentItemCount;

    public Heap(int maxHeapSize)
    {
        items = new T[maxHeapSize];
    }

    public void Add(T item)
    {
        item.values.HeapIndex = currentItemCount;
        items[currentItemCount] = item;
        SortUp(item);
        currentItemCount++;
    }

    public T RemoveFirst()
    {
        T firstItem = items[0];
        currentItemCount--;
        items[0] = items[currentItemCount];
        items[0].values.HeapIndex = 0;
        SortDown(items[0]);
        return firstItem;
    }

    public void UpdateItem(T item)
    {
        SortUp(item);
    }

    public int Count
    {
        get
        {
            return currentItemCount;
        }
    }

    public bool Contains(T item)
    {
        return Equals(items[item.values.HeapIndex], item);
    }

    void SortDown(T item)
    {
        while (true)
        {
            int childIndexLeft = item.values.HeapIndex * 2 + 1;
            int childIndexRight = item.values.HeapIndex * 2 + 2;
            int swapIndex = 0;

            if (childIndexLeft < currentItemCount)
            {
                swapIndex = childIndexLeft;

                if (childIndexRight < currentItemCount)
                {
                    if (items[childIndexLeft].values.CompareTo(items[childIndexRight].values) < 0)
                    {
                        swapIndex = childIndexRight;
                    }
                }

                if (item.values.CompareTo(items[swapIndex].values) < 0)
                {
                    Swap(item, items[swapIndex]);
                }
                else
                {
                    return;
                }

            }
            else
            {
                return;
            }
        }
    }

    void SortUp(T item)
    {
        int parentIndex = (item.values.HeapIndex - 1) / 2;

        while (true)
        {
            T parentItem = items[parentIndex];
            if (item.values.CompareTo(parentItem.values) > 0)
            {
                Swap(item, parentItem);
            }
            else
            {
                break;
            }
            parentIndex = (item.values.HeapIndex - 1) / 2;
        }
    }

    void Swap(T itemA, T itemB)
    {
        items[itemA.values.HeapIndex] = itemB;
        items[itemB.values.HeapIndex] = itemA;
        int itemAIndex = itemA.values.HeapIndex;
        itemA.values.HeapIndex = itemB.values.HeapIndex;
        itemB.values.HeapIndex = itemAIndex;
    }
}
public interface IHeapItem<T> : IComparable<T>
{
    int HeapIndex
    {
        get;
        set;
    }
}
