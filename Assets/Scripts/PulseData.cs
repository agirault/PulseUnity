using System.Collections.Generic;
using UnityEngine;

// https://answers.unity.com/questions/289692/serialize-nested-lists.html
[System.Serializable]
public class FloatList
{
    public List<float> list;
    public FloatList() {
        list = new List<float>();
    }
    public FloatList(int capacity) {
        list = new List<float>(capacity);
    }
    public void Clear() {
        list.Clear();
    }
    public void Add(float value) {
        list.Add(value);
    }
    public void Set(int index, float value) {
        list[index] = value;
    }
    public float Get(int index) {
        return list[index];
    }
    public int Count {
        get {
            return list.Count;
        }
    }
    public bool IsEmpty() {
        return Count == 0;
    }
}

public class PulseData: ScriptableObject
{
    public string[] fields;
    public FloatList timeStampList;
    public List<FloatList> valuesTable;
}
