using System.Collections.Generic;
using UnityEngine;

public class RuntimeSet<T> : ScriptableObject
{
    public List<T> Items = new List<T>();
    public void Add(T t)
    {
        if (t != null && !Items.Contains(t)) Items.Add(t);
    }
    public void AddRange(List<T> t)
    {
        foreach (var element in t)
            Add(element);
    }
    public void Remove(T t)
    {
        if (Items.Contains(t)) Items.Remove(t);
    }
}
