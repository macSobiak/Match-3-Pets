using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Block : ScriptableObject
{
    public Sprite Texture;
    [Tooltip("The blocks that are matched with this block to a 3-Match.")]
    public List<Block> MatchingBlocks = new List<Block>();

    [Tooltip("The blocks that cannot be spawned at game start adjacent to it.")]
    public List<Block> CannotBeAjacentTo = new List<Block>();

    [Tooltip("All blocks spawned on board list.")]
    public BlocksRuntimeSet BlockSet;

    [Tooltip("Max occurences on board at the same time")]
    public IntVariable MaxOccurences;

    [Tooltip("The probability (0-1) that this block type will be taken into consideration when spawning next block")]
    public FloatVariable SpawnProbability;
}
