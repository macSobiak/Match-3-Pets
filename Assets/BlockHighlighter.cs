using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockHighlighter : MonoBehaviour
{
    public BlocksRuntimeSet SelectedBlocksToHighlight;
    public GameEvent OnBlockReadyToSwap;

    public void HighlighObject(GameObject objectToHighlight)
    {
        var block = objectToHighlight.GetComponent<BlockElement>();
        if (block)
        {
            if (SelectedBlocksToHighlight.Items.Count >= 2)
            {
                //if object is clicked and there are already 2, deselect the previous pair
                DeselectAndClearAll();
            }
            if (SelectedBlocksToHighlight.Items.Contains(block))
            {
                //deselect after clicking again
                block.DeselectBlock();
                SelectedBlocksToHighlight.Remove(block);
            }
            else
            {
                CheckIfAdjacentAndSelect(block);
            }
            if (SelectedBlocksToHighlight.Items.Count >= 2)
            {
                //if 2 blocks successfuly selected - send ready to swap signal
                OnBlockReadyToSwap.Raise();
            }
        }
    }

    public void DeselectAndClearAll()
    {
        foreach (var blockElement in SelectedBlocksToHighlight.Items)
            blockElement.DeselectBlock();
        SelectedBlocksToHighlight.Items.Clear();
    }

    private void CheckIfAdjacentAndSelect(BlockElement block)
    {
        //select block only if it is first to select or second one if adjacent to the first one (only those should be swappable)
        if (SelectedBlocksToHighlight.Items.Count == 0
            || (Mathf.Abs(SelectedBlocksToHighlight.Items[0].Column - block.Column) == 1 && SelectedBlocksToHighlight.Items[0].Row == block.Row)
            || (Mathf.Abs(SelectedBlocksToHighlight.Items[0].Row - block.Row) == 1 && SelectedBlocksToHighlight.Items[0].Column == block.Column))
        {
            block.SelectBlock();
            SelectedBlocksToHighlight.Add(block);
        }
        //if not adjacent block is clicked - reset selection
        else
            DeselectAndClearAll();
    }
}
