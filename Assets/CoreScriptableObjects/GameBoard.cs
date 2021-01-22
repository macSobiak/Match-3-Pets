using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class GameBoard : ScriptableObject
{
    public BlockElement[,] Grid;
    public IntVariable HorizontalSize;
    public IntVariable VerticalSize;

    public Block GetBlockTypeFromGrid(int col, int row)
    {
        if (col >= HorizontalSize.Value || col < 0
            || row >= VerticalSize.Value || row < 0)
            return null;
        return Grid[col, row].Block;
    }

    public BlockElement GetBlockFromGrid(int col, int row)
    {
        if (col >= HorizontalSize.Value || col < 0
            || row >= VerticalSize.Value || row < 0)
            return null;
        return Grid[col, row];
    }
}
