using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BoardMovementManager : MonoBehaviour
{
    //Set of blocks to move and destroy
    public BlocksRuntimeSet SelectedBlocks;
    public BlocksRuntimeSet BlocksToDestroy;


    public FloatVariable DestroyScaleValue;
    public FloatVariable DestroyRotateValue;
    public FloatVariable DestroyBlockTimeSec;
    public FloatVariable FallBlockTimeSec;
    public FloatVariable SwapBlocksTimeSec;

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

        LeanTween.moveX(elementsToSwap1.gameObject, x2, SwapBlocksTimeSec.Value).setOnComplete(EndAnimationEvent.Raise);
        LeanTween.moveY(elementsToSwap1.gameObject, y2, SwapBlocksTimeSec.Value);
        LeanTween.moveX(elementsToSwap2.gameObject, x1, SwapBlocksTimeSec.Value);
        LeanTween.moveY(elementsToSwap2.gameObject, y1, SwapBlocksTimeSec.Value);
    }

    public void DestroyAnimation()
    {
        OnMovementStarted.Raise();

        for (int i = BlocksToDestroy.Items.Count - 1; i >= 0; i--)
        {
            LeanTween.rotateAround(BlocksToDestroy.Items[i].gameObject, Vector3.forward, DestroyRotateValue.Value, DestroyBlockTimeSec.Value).setEase(LeanTweenType.easeInQuad);
            LeanTween.scaleX(BlocksToDestroy.Items[i].gameObject, DestroyScaleValue.Value, DestroyBlockTimeSec.Value).setEase(LeanTweenType.easeInQuad);

            if (i == 0)
            {
                LeanTween.scaleY(BlocksToDestroy.Items[i].gameObject, DestroyScaleValue.Value, DestroyBlockTimeSec.Value).setEase(LeanTweenType.easeInQuad).setOnComplete(DestroyBlocks); ;
            }
            else
            {
                LeanTween.scaleY(BlocksToDestroy.Items[i].gameObject, DestroyScaleValue.Value, DestroyBlockTimeSec.Value).setEase(LeanTweenType.easeInQuad);
            }
        }
    }
    private void DestroyBlocks()
    {
        for (int i = BlocksToDestroy.Items.Count - 1; i >= 0; i--)
        {
            GameBoardToUpdate.Grid[BlocksToDestroy.Items[i].Column, BlocksToDestroy.Items[i].Row] = null;
            Destroy(BlocksToDestroy.Items[i].gameObject);
        }
        OnMovementEnded.Raise();
        OnBoardRefresh.Raise();
    }

    public void DropBlocksToProperPlace()
    {
        OnMovementStarted.Raise();

        for (int col = 0; col < GameBoardToUpdate.HorizontalSize.Value; col++)
        {
            for (int row = 0; row < GameBoardToUpdate.VerticalSize.Value; row++)
            {
                var block = GameBoardToUpdate.Grid[col, row];
                Vector3 destinationPosition = GameBoardToUpdate.GetPositionOnBoardFromCoordinates(block.Column, block.Row, 0); ;
                if (col == GameBoardToUpdate.HorizontalSize.Value-1 && row == GameBoardToUpdate.VerticalSize.Value-1)
                    LeanTween.moveY(block.gameObject, destinationPosition.y, FallBlockTimeSec.Value).setEase(LeanTweenType.easeInQuad).setOnComplete(SignalDropEnded);
                else
                    LeanTween.moveY(block.gameObject, destinationPosition.y, FallBlockTimeSec.Value).setEase(LeanTweenType.easeInQuad);
            }
        }
    }

    private void SignalDropEnded()
    {
        OnMovementEnded.Raise();
        OnCheckMatches.Raise();
    }
}
