using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu]
public class LevelDefinition : ScriptableObject
{
    public List<Block> BlockTypesToSpawn = new List<Block>();
    public GameBoard GameBoard;
}
