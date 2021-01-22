using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchScanner : MonoBehaviour
{
    public GameBoard BoardToScan;

    public BlocksRuntimeSet BlocksMatched;
    public BlocksRuntimeSet BlocksSwapped;
    public BlocksRuntimeSet AllColorBlocks;
    public BlocksRuntimeSet BombBlocks;

    public GameEvent MatchFound;
    public GameEvent NoMatchFound;
    public void FindMatches()
    {
        FindMatchesFromSwappingSpecialBlocks();

        FindAdjacentMatches();

        if (BlocksMatched.Items.Count > 0)
        {
            FindMatchesForExplodingBombBlockIfAvailable();
            MatchFound.Raise();
        }
        else
            NoMatchFound.Raise();
    }

    private void FindMatchesForExplodingBombBlockIfAvailable()
    {
        List<BlockElement> additionalBlockFromSpecialBehaviors = new List<BlockElement>();
        foreach (var matchedBlock in BlocksMatched.Items)
        {
            if (BombBlocks.Items.Contains(matchedBlock))
            {
                additionalBlockFromSpecialBehaviors = GetSurroundingBlocks(matchedBlock, 1);
            }
        }
        BlocksMatched.AddRange(additionalBlockFromSpecialBehaviors);
    }

    private void FindAdjacentMatches()
    {
        for (int col = 0; col < BoardToScan.HorizontalSize.Value; col++)
        {
            for (int row = 0; row < BoardToScan.VerticalSize.Value; row++)
            {
                var currentBlockTypes = BoardToScan.Grid[col, row].Block.MatchingBlocks;

                var verScanRes = ScanVertical(col, row, currentBlockTypes);
                if (verScanRes.Count >= 2)
                {
                    BlocksMatched.AddRange(verScanRes);
                    BlocksMatched.Add(BoardToScan.Grid[col, row]);
                }

                var horScanRes = ScanHorizontal(col, row, currentBlockTypes);
                if (horScanRes.Count >= 2)
                {
                    BlocksMatched.AddRange(horScanRes);
                    BlocksMatched.Add(BoardToScan.Grid[col, row]);
                }
            }
        }
    }

    private void FindMatchesFromSwappingSpecialBlocks()
    {
        if (BlocksSwapped.Items.Count >= 2)
        {
            if (AllColorBlocks.Items.Contains(BlocksSwapped.Items[0]))
            {
                BlocksMatched.AddRange(BlocksSwapped.Items[1].Block.BlockSet.Items);
                BlocksMatched.Add(BlocksSwapped.Items[0]);
            }
            else if (AllColorBlocks.Items.Contains(BlocksSwapped.Items[1]))
            {
                BlocksMatched.AddRange(BlocksSwapped.Items[0].Block.BlockSet.Items);
                BlocksMatched.Add(BlocksSwapped.Items[1]);
            }
        }
    }

    private List<BlockElement> GetSurroundingBlocks(BlockElement matchedBlock, int range)
    {
        List<BlockElement> surroundingBlockList = new List<BlockElement>();

        for(int i=1; i <= range; i++)
        {
            surroundingBlockList.Add(BoardToScan.GetBlockFromGrid(matchedBlock.Column - i, matchedBlock.Row - i));
            surroundingBlockList.Add(BoardToScan.GetBlockFromGrid(matchedBlock.Column - i, matchedBlock.Row));
            surroundingBlockList.Add(BoardToScan.GetBlockFromGrid(matchedBlock.Column - i, matchedBlock.Row + i));
            surroundingBlockList.Add(BoardToScan.GetBlockFromGrid(matchedBlock.Column, matchedBlock.Row - i));
            surroundingBlockList.Add(BoardToScan.GetBlockFromGrid(matchedBlock.Column, matchedBlock.Row + i));
            surroundingBlockList.Add(BoardToScan.GetBlockFromGrid(matchedBlock.Column + i, matchedBlock.Row - i));
            surroundingBlockList.Add(BoardToScan.GetBlockFromGrid(matchedBlock.Column + i, matchedBlock.Row));
            surroundingBlockList.Add(BoardToScan.GetBlockFromGrid(matchedBlock.Column + i, matchedBlock.Row + i));
        }


        return surroundingBlockList;
    }

    private List<BlockElement> ScanVertical(int col, int row, List<Block> currentBlockTypes)
    {
        Block matchType = null;
        List<BlockElement> matchedBlocksVerticalList = new List<BlockElement>();

        for (int i = row + 1; i < BoardToScan.VerticalSize.Value; i++)
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
                        if(nextBlockTypes.Count == 1)
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

        for (int i = col + 1; i < BoardToScan.HorizontalSize.Value; i++)
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
                        if (nextBlockTypes.Count == 1)
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
