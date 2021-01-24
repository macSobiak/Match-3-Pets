using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BlockElement : MonoBehaviour
{
    public Block Block;
    public BlocksRuntimeSet SelectedBlocks;
    public BlocksRuntimeSet MatchedBlocks;
    public int Column;
    public int Row;
    public GameEvent OnBlockDestroyed;
    public SpriteRenderer Renderer;

    private void OnEnable()
    {
        Renderer = GetComponent<SpriteRenderer>();
        Renderer.sprite = Block.Texture;
        //Add the block to the block type set
        Block.BlockSet.Add(this);
    }

    private void OnDisable()
    {
        Block.BlockSet.Remove(this);
        SelectedBlocks.Remove(this);
        MatchedBlocks.Remove(this);

        OnBlockDestroyed.Raise();
    }
    public void SelectBlock()
    {
        Renderer.color = Color.gray;
    }
    public void DeselectBlock()
    {
        Renderer.color = Color.white;
    }


}
