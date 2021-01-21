using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchScanner : MonoBehaviour
{
    public GameBoard BoardToScan;
    public BlocksRuntimeSet BlocksMatched;
    public GameEvent MatchFound;
    public GameEvent NoMatchFound;
    public void FindMatches()
    {
        for (int col = 0; col < BoardToScan.HorizontalSize.value; col++)
        {
            for (int row = 0; row < BoardToScan.VerticalSize.value; row++)
            {
                var currentBlockTypes = BoardToScan.Grid[col, row].Block.MatchingBlocks;

                var verScanRes = ScanVertical(col, row, currentBlockTypes);
                if (verScanRes.Count >= 2)
                {
                    BlocksMatched.Items.AddRange(verScanRes);
                    BlocksMatched.Items.Add(BoardToScan.Grid[col, row]);
                }

                var horScanRes = ScanHorizontal(col, row, currentBlockTypes);
                if (horScanRes.Count >= 2)
                {
                    BlocksMatched.Items.AddRange(horScanRes);
                    BlocksMatched.Items.Add(BoardToScan.Grid[col, row]);
                }
            }
        }
        if (BlocksMatched.Items.Count > 0)
            MatchFound.Raise();
        else
            NoMatchFound.Raise();
    }

    private List<BlockElement> ScanVertical(int col, int row, List<Block> currentBlockTypes)
    {
        Block matchType = null;
        List<BlockElement> matchedBlocksVerticalList = new List<BlockElement>();

        for (int i = row + 1; i < BoardToScan.VerticalSize.value; i++)
        {
            var nextBlockTypes = BoardToScan.Grid[col, i].Block.MatchingBlocks;
            bool matchFound = false;
            if (matchType)
            {
                if (nextBlockTypes.Contains(matchType))
                {
                    matchedBlocksVerticalList.Add(BoardToScan.Grid[col, i]);
                    matchFound = true;
                }
            }
            else
            {
                foreach (var blockType in currentBlockTypes)
                {
                    if (nextBlockTypes.Contains(blockType))
                    {
                        matchType = blockType;
                        matchedBlocksVerticalList.Add(BoardToScan.Grid[col, i]);
                        matchFound = true;
                        break;
                    }
                }
            }

            if (!matchFound)
                break;
        }
        return matchedBlocksVerticalList;
    }

    private List<BlockElement> ScanHorizontal(int col, int row, List<Block> currentBlockTypes)
    {
        Block matchType = null;
        List<BlockElement> matchedBlocksVerticalList = new List<BlockElement>();

        for (int i = col + 1; i < BoardToScan.HorizontalSize.value; i++)
        {
            var nextBlockTypes = BoardToScan.Grid[i, row].Block.MatchingBlocks;
            bool matchFound = false;
            if(matchType)
            {
                if (nextBlockTypes.Contains(matchType))
                {
                    matchedBlocksVerticalList.Add(BoardToScan.Grid[i, row]);
                    matchFound = true;
                }
            }
            else
            {
                foreach (var blockType in currentBlockTypes)
                {
                    if (nextBlockTypes.Contains(blockType))
                    {
                        matchType = blockType;
                        matchedBlocksVerticalList.Add(BoardToScan.Grid[i, row]);
                        matchFound = true;
                        break;
                    }

                }
            }

            if (!matchFound)
                break;
        }
        return matchedBlocksVerticalList;
    }
}
