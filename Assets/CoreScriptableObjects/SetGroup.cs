using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu]
public class SetGroup : ScriptableObject
{
    [Tooltip("The Sets that are connected by a same feature.")]
    public List<BlocksRuntimeSet> GroupOfSets;
}
