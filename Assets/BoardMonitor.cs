using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BoardMonitor : MonoBehaviour
{
    public BlocksRuntimeSet SelectedBlocks;
    public GameEvent OnBlockSwapEnded;
    public GameBoard GameBoardToUpdate;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SwapSelectedBlocks()
    {
        if (SelectedBlocks.Items.Count >= 2)
        {
            SwapBlocks(SelectedBlocks.Items[0], SelectedBlocks.Items[1]);
        }
    }

    private void SwapBlocks(BlockElement ElementsToSwap1, BlockElement ElementsToSwap2)
    {
        float x1 = ElementsToSwap1.transform.position.x;
        float y1 = ElementsToSwap1.transform.position.y;
        int col1 = ElementsToSwap1.Column;
        int row1 = ElementsToSwap1.Row;

        float x2 = ElementsToSwap2.transform.position.x;
        float y2 = ElementsToSwap2.transform.position.y;
        int col2 = ElementsToSwap2.Column;
        int row2 = ElementsToSwap2.Row;

        GameBoardToUpdate.Grid[col1, row1] = ElementsToSwap2;
        GameBoardToUpdate.Grid[col2, row2] = ElementsToSwap1;

        ElementsToSwap1.Column = col2;
        ElementsToSwap1.Row = row2;

        ElementsToSwap2.Column = col1;
        ElementsToSwap2.Row = row1;

        LeanTween.moveX(ElementsToSwap1.gameObject, x2, 0.5f).setOnComplete(OnBlockSwapEnded.Raise);
        LeanTween.moveY(ElementsToSwap1.gameObject, y2, 0.5f);
        LeanTween.moveX(ElementsToSwap2.gameObject, x1, 0.5f);
        LeanTween.moveY(ElementsToSwap2.gameObject, y1, 0.5f);
    }
    
    void FillEmptyPlaces()
    {
        for (int col = 0; col < GameBoardToUpdate.HorizontalSize.value; col++)
        {
            for (int row = GameBoardToUpdate.VerticalSize.value - 1; row >= 0; row--)
            {

            }
        }
    }
}
