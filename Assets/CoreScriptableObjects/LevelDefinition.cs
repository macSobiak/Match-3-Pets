using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu]
public class LevelDefinition : ScriptableObject
{
    [Tooltip("List of Blocks that will be taken into consideration when building a level.")]
    public List<Block> BlockTypesToSpawn = new List<Block>();
    [Tooltip("The GameBoard object that stores the size and grid with Block Elements.")]
    public GameBoard GameBoard;
}
