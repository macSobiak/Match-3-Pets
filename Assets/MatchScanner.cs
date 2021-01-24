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
            AddSurroundingBlocksIfBombIsMatched(BlocksMatched.Items);
            MatchFound.Raise();
        }
        else
            NoMatchFound.Raise();
    }

    private void AddSurroundingBlocksIfBombIsMatched(List<BlockElement> matchedBlocksToCheck)
    {
        List<BlockElement> additionalBlockFromSpecialBehaviors = new List<BlockElement>();

        //check all matched blocks if any is a special bomb block
        foreach (var matchedBlock in matchedBlocksToCheck)
        {
            if (BombBlocks.Items.Contains(matchedBlock))
            {
                foreach(var surroundingBlock in GetSurroundingBlocks(matchedBlock, 1))
                {
                    //add only block that were not added before (it gets rid of stack overflow problem when 2 bombs are triggering eachother endlessly)
                    if(!BlocksMatched.Items.Contains(surroundingBlock))
                        additionalBlockFromSpecialBehaviors.Add(surroundingBlock);
                }
            }
        }
        BlocksMatched.AddRange(additionalBlockFromSpecialBehaviors);

        //If exploded bomb also triggered other bomb - check it and make a chain reaction
        if(additionalBlockFromSpecialBehaviors.Count > 0)
        {
            AddSurroundingBlocksIfBombIsMatched(additionalBlockFromSpecialBehaviors);
        }
    }

    private void FindAdjacentMatches()
    {
        for (int col = 0; col < BoardToScan.HorizontalSize.Value; col++)
        {
            for (int row = 0; row < BoardToScan.VerticalSize.Value; row++)
            {
                //check what types of blocks is current block matched with
                var currentBlockTypes = BoardToScan.Grid[col, row].Block.MatchingBlocks;

                //check vertical matches
                var verScanRes = ScanVertical(col, row, currentBlockTypes);
                if (verScanRes.Count >= 2)
                {
                    BlocksMatched.AddRange(verScanRes);
                    BlocksMatched.Add(BoardToScan.Grid[col, row]);
                }

                //check horizontal matches
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
        //if one of swapped block is "all color block" (it is removing all blocks from the block type it was swapped with), 
        //take the other ones type and add all of them to match list (together with itself)
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
        //get all blocks around the bomb block, it can be made larger in cross (bomberman style) pattern based on range parameter
        List<BlockElement> surroundingBlockList = new List<BlockElement>();

        for(int i=1; i <= range; i++)
        {
            //GetBlockFromGrid returns a null if grid size is exceeded, so no additional checks
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
        List<BlockElement> matchedBlocksList = new List<BlockElement>();

        //scan all blocks starting from the given one
        for (int i = row + 1; i < BoardToScan.VerticalSize.Value; i++)
        {
            var nextBlockTypes = BoardToScan.Grid[col, i].Block.MatchingBlocks;
            bool matchFound = false;

            //if match type is found, next one needs to be same (in case some blocks with more than 1 matching blocks is found - examples:
            //correct match - type A/B, type A, type A
            //incorrect match - type A/B, type A, type B
            if (!matchType)
            {
                foreach (var blockType in currentBlockTypes)
                {
                    if (nextBlockTypes.Contains(blockType))
                    {
                        if (nextBlockTypes.Count == 1)
                            matchType = blockType;
                        matchedBlocksList.Add(BoardToScan.Grid[col, i]);
                        matchFound = true;
                        break;
                    }
                }
            }
            else
            {
                if (nextBlockTypes.Contains(matchType))
                {
                    matchedBlocksList.Add(BoardToScan.Grid[col, i]);
                    matchFound = true;
                }
            }

            //if no match found on block, break the scaning of this match
            if (!matchFound)
                break;
        }
        return matchedBlocksList;
    }

    private List<BlockElement> ScanHorizontal(int col, int row, List<Block> currentBlockTypes)
    {
        Block matchType = null;
        List<BlockElement> matchedBlocksList = new List<BlockElement>();

        for (int i = col + 1; i < BoardToScan.HorizontalSize.Value; i++)
        {
            var nextBlockTypes = BoardToScan.Grid[i, row].Block.MatchingBlocks;
            bool matchFound = false;

            //if match type is found, next one needs to be same (in case some blocks with more than 1 matching blocks is found - examples:
            //correct match - type A/B, type A, type A
            //incorrect match - type A/B, type A, type B
            if (!matchType)
            {
                foreach (var blockType in currentBlockTypes)
                {
                    if (nextBlockTypes.Contains(blockType))
                    {
                        if (nextBlockTypes.Count == 1)
                            matchType = blockType;
                        matchedBlocksList.Add(BoardToScan.Grid[i, row]);
                        matchFound = true;
                        break;
                    }

                }
            }
            else
            {
                if (nextBlockTypes.Contains(matchType))
                {
                    matchedBlocksList.Add(BoardToScan.Grid[i, row]);
                    matchFound = true;
                }
            }

            //if no match found on block, break the scaning of this match
            if (!matchFound)
                break;
        }
        return matchedBlocksList;
    }
}
