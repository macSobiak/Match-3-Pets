using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BlockElement : MonoBehaviour
{
    public Block Block;
    public BlocksRuntimeSet SelectedBlocks;
    public int Column;
    public int Row;
    public bool IsSelected;
    private SpriteRenderer _renderer;

    private void OnEnable()
    {
        _renderer = GetComponent<SpriteRenderer>();
        _renderer.sprite = Block.Texture;
        Block.BlockSet.Add(this);
    }

    private void OnDisable()
    {
        Block.BlockSet.Remove(this);
        SelectedBlocks.Remove(this);
    }

    void Update()
    {
    }

    public void SelectBlock()
    {
        _renderer.color = Color.grey;
    }
    public void DeselectBlock()
    {
        _renderer.color = Color.white;
    }


}
