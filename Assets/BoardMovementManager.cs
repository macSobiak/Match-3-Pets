using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BoardMovementManager : MonoBehaviour
{
    //sets of blocks to move and destroy
    public BlocksRuntimeSet SelectedBlocks;
    public BlocksRuntimeSet BlocksToDestroy;

    //variables to control the animation from inspector
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

    //swap selected blocks and check matches after swapping
    public void SwapSelectedBlocks()
    {
        if (SelectedBlocks.Items.Count >= 2)
        {
            SwapBlocks(SelectedBlocks.Items[0], SelectedBlocks.Items[1], OnCheckMatches);
        }
    }

    //revert swap after no matches found
    public void RevertSelectedBlocks()
    {
        if (SelectedBlocks.Items.Count >= 2)
        {
            SwapBlocks(SelectedBlocks.Items[0], SelectedBlocks.Items[1], OnMovementEnded);
        }
    }

    private void SwapBlocks(BlockElement elementsToSwap1, BlockElement elementsToSwap2, GameEvent EndAnimationEvent)
    {
        OnMovementStarted.Raise(); //raise event, input handler should not allow mouse interaction

        //remember position of blocks to swap
        float x1 = elementsToSwap1.transform.position.x;
        float y1 = elementsToSwap1.transform.position.y;
        int col1 = elementsToSwap1.Column;
        int row1 = elementsToSwap1.Row;

        float x2 = elementsToSwap2.transform.position.x;
        float y2 = elementsToSwap2.transform.position.y;
        int col2 = elementsToSwap2.Column;
        int row2 = elementsToSwap2.Row;

        //swap element on the grid table
        GameBoardToUpdate.Grid[col1, row1] = elementsToSwap2;
        GameBoardToUpdate.Grid[col2, row2] = elementsToSwap1;

        //also swap the block position parameters
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
        OnMovementStarted.Raise(); //raise event, input handler should not allow mouse interaction

        //rotating and scaling down block for a cool destroy animation
        for (int i = BlocksToDestroy.Items.Count - 1; i >= 0; i--)
        {
            LeanTween.rotateAround(BlocksToDestroy.Items[i].gameObject, Vector3.forward, DestroyRotateValue.Value, DestroyBlockTimeSec.Value).setEase(LeanTweenType.easeInQuad);
            LeanTween.scaleX(BlocksToDestroy.Items[i].gameObject, DestroyScaleValue.Value, DestroyBlockTimeSec.Value).setEase(LeanTweenType.easeInQuad);

            //if it is the last iteration, send the end signal after movement
            if (i == 0)
                LeanTween.scaleY(BlocksToDestroy.Items[i].gameObject, DestroyScaleValue.Value, DestroyBlockTimeSec.Value).setEase(LeanTweenType.easeInQuad).setOnComplete(DestroyBlocks);
            else
                LeanTween.scaleY(BlocksToDestroy.Items[i].gameObject, DestroyScaleValue.Value, DestroyBlockTimeSec.Value).setEase(LeanTweenType.easeInQuad);
            
        }
    }
    private void DestroyBlocks()
    {
        //destroy the blocks from the matches set, ensure that after destroy, grid object is null immediately
        for (int i = BlocksToDestroy.Items.Count - 1; i >= 0; i--)
        {
            int col = BlocksToDestroy.Items[i].Column;
            int row = BlocksToDestroy.Items[i].Row;
            Destroy(GameBoardToUpdate.Grid[col, row].gameObject);
            GameBoardToUpdate.Grid[col, row] = null;

        }
        //bring back input handler, call for board refresh with new blocks
        OnMovementEnded.Raise();
        OnBoardRefresh.Raise();
    }

    public void DropBlocksToProperPlace()
    {
        OnMovementStarted.Raise(); //raise event, input handler should not allow mouse interaction

        for (int col = 0; col < GameBoardToUpdate.HorizontalSize.Value; col++)
        {
            for (int row = 0; row < GameBoardToUpdate.VerticalSize.Value; row++)
            {
                //ensure that every block will have proper position on board according to its properties
                var block = GameBoardToUpdate.Grid[col, row];
                Vector3 destinationPosition = GameBoardToUpdate.GetPositionOnBoardFromCoordinates(block.Column, block.Row, 0); ;

                //if it is the last iteration, send the end signal after movement
                if (col == GameBoardToUpdate.HorizontalSize.Value-1 && row == GameBoardToUpdate.VerticalSize.Value-1)
                    LeanTween.moveY(block.gameObject, destinationPosition.y, FallBlockTimeSec.Value).setEase(LeanTweenType.easeInQuad).setOnComplete(SignalDropEnded);
                else
                    LeanTween.moveY(block.gameObject, destinationPosition.y, FallBlockTimeSec.Value).setEase(LeanTweenType.easeInQuad);
            }
        }
    }

    private void SignalDropEnded()
    {
        //bring back input handler, check for matches after drop
        OnMovementEnded.Raise();
        OnCheckMatches.Raise();
    }
}
