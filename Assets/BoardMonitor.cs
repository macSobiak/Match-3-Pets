﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BoardMonitor : MonoBehaviour
{
    public BlocksRuntimeSet SelectedBlocks;
    public BlocksRuntimeSet BlocksToDestroy;

    public GameEvent OnCheckMatches;
    public GameEvent OnBoardRefresh;
    public GameEvent OnMovementEnded;
    public GameEvent OnMovementStarted;

    public GameBoard GameBoardToUpdate;

    public void SwapSelectedBlocks()
    {
        if (SelectedBlocks.Items.Count >= 2)
        {
            SwapBlocks(SelectedBlocks.Items[0], SelectedBlocks.Items[1], OnCheckMatches);
        }
    }

    public void RevertSelectedBlocks()
    {
        if (SelectedBlocks.Items.Count >= 2)
        {
            SwapBlocks(SelectedBlocks.Items[0], SelectedBlocks.Items[1], OnMovementEnded);
        }
    }

    private void SwapBlocks(BlockElement elementsToSwap1, BlockElement elementsToSwap2, GameEvent EndAnimationEvent)
    {
        OnMovementStarted.Raise();
        float x1 = elementsToSwap1.transform.position.x;
        float y1 = elementsToSwap1.transform.position.y;
        int col1 = elementsToSwap1.Column;
        int row1 = elementsToSwap1.Row;

        float x2 = elementsToSwap2.transform.position.x;
        float y2 = elementsToSwap2.transform.position.y;
        int col2 = elementsToSwap2.Column;
        int row2 = elementsToSwap2.Row;

        GameBoardToUpdate.Grid[col1, row1] = elementsToSwap2;
        GameBoardToUpdate.Grid[col2, row2] = elementsToSwap1;

        elementsToSwap1.Column = col2;
        elementsToSwap1.Row = row2;

        elementsToSwap2.Column = col1;
        elementsToSwap2.Row = row1;

        LeanTween.moveX(elementsToSwap1.gameObject, x2, 0.5f).setOnComplete(EndAnimationEvent.Raise);
        LeanTween.moveY(elementsToSwap1.gameObject, y2, 0.5f);
        LeanTween.moveX(elementsToSwap2.gameObject, x1, 0.5f);
        LeanTween.moveY(elementsToSwap2.gameObject, y1, 0.5f);
    }

    public void DestroyBlocks()
    {
        for (int i = BlocksToDestroy.Items.Count - 1; i >= 0; i--)
        {
            GameBoardToUpdate.Grid[BlocksToDestroy.Items[i].Column, BlocksToDestroy.Items[i].Row] = null;
            Destroy(BlocksToDestroy.Items[i].gameObject);
            BlocksToDestroy.Remove(BlocksToDestroy.Items[i]);
        }
        OnBoardRefresh.Raise();
    }
    public void DropBlocksToProperPlace()
    {
        OnMovementStarted.Raise();
        for (int col = 0; col < GameBoardToUpdate.HorizontalSize.value; col++)
        {
            for (int row = 0; row < GameBoardToUpdate.VerticalSize.value; row++)
            {
                var block = GameBoardToUpdate.Grid[col, row];
                new Vector3(block.Column - (GameBoardToUpdate.HorizontalSize.value / 2f) + 0.5f, -block.Row + (GameBoardToUpdate.VerticalSize.value / 2f) - 0.5f, 0f);
                if(col == GameBoardToUpdate.HorizontalSize.value-1 && row == GameBoardToUpdate.VerticalSize.value-1)
                    LeanTween.moveY(block.gameObject, -block.Row + (GameBoardToUpdate.VerticalSize.value / 2f) - 0.5f, 0.5f).setOnComplete(SignalDropEnded);
                else
                    LeanTween.moveY(block.gameObject, -block.Row + (GameBoardToUpdate.VerticalSize.value / 2f) - 0.5f, 0.5f);
            }
        }
    }

    private void SignalDropEnded()
    {
        OnCheckMatches.Raise();
        OnMovementEnded.Raise();
    }
}
