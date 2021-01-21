using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockHighlighter : MonoBehaviour
{
    public BlocksRuntimeSet SelectedBlocksToHighlight;
    public GameEvent OnBlockReadyToSwap;

    // Start is called before the first frame update
    public void HighlighObject(GameObject objectToHighlight)
    {
        var block = objectToHighlight.GetComponent<BlockElement>();
        if (block)
        {
            if (SelectedBlocksToHighlight.Items.Count >= 2)
            {
                DeselectAndClearAll();
            }
            if (SelectedBlocksToHighlight.Items.Contains(block))
            {
                block.DeselectBlock();
                SelectedBlocksToHighlight.Remove(block);
            }
            else
            {
                CheckIfAdjacentAndSelect(block);
            }
            if (SelectedBlocksToHighlight.Items.Count >= 2)
            {
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
        if (SelectedBlocksToHighlight.Items.Count == 0
            || (Mathf.Abs(SelectedBlocksToHighlight.Items[0].Column - block.Column) == 1 && SelectedBlocksToHighlight.Items[0].Row == block.Row)
            || (Mathf.Abs(SelectedBlocksToHighlight.Items[0].Row - block.Row) == 1 && SelectedBlocksToHighlight.Items[0].Column == block.Column))
        {
            block.SelectBlock();
            SelectedBlocksToHighlight.Add(block);
        }
        else
            DeselectAndClearAll();
    }
}
