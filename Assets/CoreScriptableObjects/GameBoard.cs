using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class GameBoard : ScriptableObject
{
    public BlockElement[,] Grid;
    public IntVariable HorizontalSize;
    public IntVariable VerticalSize;
}
