using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu]
public class Block : ScriptableObject
{
    public Sprite Texture;
    public Sprite HighlightTexture;
    public List<Block> MatchingBlocks = new List<Block>();
    public List<Block> CannotBeAjacentTo = new List<Block>();
    public IntVariable MaxOccurences;
    public BlocksRuntimeSet BlockSet;

}
