using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockHighlighter : MonoBehaviour
{
    public BlocksRuntimeSet SelectedBlocksToHighlit;
    public GameEvent OnBlockReadyToSwap;

    // Start is called before the first frame update
    public void HighlighObject(GameObject objectToHighlight)
    {
        var block = objectToHighlight.GetComponent<BlockElement>();
        if (block)
        {
            block.IsSelected = !block.IsSelected;

            if (SelectedBlocksToHighlit.Items.Count >= 2)
            {
                DeselectAndClearAll();
            }
            if (SelectedBlocksToHighlit.Items.Contains(block))
            {
                block.DeselectBlock();
                SelectedBlocksToHighlit.Remove(block);
            }
            else
            {
                CheckIfAdjacentAndSelect(block);
            }
            if (SelectedBlocksToHighlit.Items.Count >= 2)
            {
                OnBlockReadyToSwap.Raise();
            }
        }

    }

    public void DeselectAndClearAll()
    {
        foreach (var blockElement in SelectedBlocksToHighlit.Items)
            blockElement.DeselectBlock();
        SelectedBlocksToHighlit.Items.Clear();
    }

    private void CheckIfAdjacentAndSelect(BlockElement block)
    {
        if (SelectedBlocksToHighlit.Items.Count == 0
            || (Mathf.Abs(SelectedBlocksToHighlit.Items[0].Column - block.Column) == 1 && SelectedBlocksToHighlit.Items[0].Row == block.Row)
            || (Mathf.Abs(SelectedBlocksToHighlit.Items[0].Row - block.Row) == 1 && SelectedBlocksToHighlit.Items[0].Column == block.Column))
        {
            block.SelectBlock();
            SelectedBlocksToHighlit.Add(block);
        }
        else
            DeselectAndClearAll();
    }
}
