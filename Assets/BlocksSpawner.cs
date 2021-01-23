using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlocksSpawner : MonoBehaviour
{
    //object to instantiate
    public GameObject entityToSpawn;

    public LevelDefinition LevelDefinition;
    public GameBoard GameBoard;

    public GameEvent BoardUpdated;
    public GameEvent BlocksGenerated;

    void Start()
    {
        //initialize grid with proper size
        GameBoard.Grid = new BlockElement[GameBoard.HorizontalSize.Value, GameBoard.VerticalSize.Value];
        PlaceBlocks();
    }

    private BlockElement InstantiateBlock(Block blockType, int column, int row, int rowOffset)
    {
        Vector3 position = GameBoard.GetPositionOnBoardFromCoordinates(column, row, rowOffset);
        BlockElement blockToSpawn = entityToSpawn.GetComponent<BlockElement>();

        blockToSpawn.Block = blockType;
        blockToSpawn.Column = column;
        blockToSpawn.Row = row;

        blockToSpawn.name = blockToSpawn.Block.name;
        return Instantiate(entityToSpawn, position, Quaternion.identity).GetComponent<BlockElement>();
    }

    private void PlaceBlocks()
    {
        for (int col = 0; col < GameBoard.HorizontalSize.Value; col++)
        {
            for (int row = 0; row < GameBoard.VerticalSize.Value; row++)
            {
                List<Block> possibleBlockTypes = GetPossibleBlockTypes();

                //get 2 block on the left to check which next block can be spawned to ensure that there will be no match
                Block leftBlock1 = GameBoard.GetBlockTypeFromGrid(col - 1, row);
                Block leftBlock2 = GameBoard.GetBlockTypeFromGrid(col - 2, row);

                if (leftBlock1 != null)
                    RemoveBlockTypesThatCannotBeAdjacentAtStart(possibleBlockTypes, leftBlock1);
                
                if (leftBlock2 != null) //if 2 places from block is not null, it means tha 1 place is also not null
                    RemoveBlockTypesToAvoid3MatchesAtStart(possibleBlockTypes, leftBlock1, leftBlock2);

                //get 2 block on the top to check which next block can be spawned to ensure that there will be no match
                Block upBlock1 = GameBoard.GetBlockTypeFromGrid(col, row - 1);
                Block upBlock2 = GameBoard.GetBlockTypeFromGrid(col, row - 2);

                if (upBlock1 != null)
                    RemoveBlockTypesThatCannotBeAdjacentAtStart(possibleBlockTypes, upBlock1);

                if (upBlock2 != null)
                    RemoveBlockTypesToAvoid3MatchesAtStart(possibleBlockTypes, upBlock1, upBlock2);
                
                GameBoard.Grid[col, row] = InstantiateBlock(possibleBlockTypes[Random.Range(0, possibleBlockTypes.Count)], col, row, 0);
            }
        }
        BoardUpdated.Raise(); //raise event to check for matches - there should be none if block types are defined properly in the inspector
    }

    private static void RemoveBlockTypesThatCannotBeAdjacentAtStart(List<Block> possibleBlockTypes, Block leftBlock1)
    {
        foreach (var blockToRemove in leftBlock1.CannotBeAjacentTo) //remove immediately block types that were defined that can not spawn on start adjacent to the nearest block
            possibleBlockTypes.Remove(blockToRemove);
    }

    private static void RemoveBlockTypesToAvoid3MatchesAtStart(List<Block> possibleBlockTypes, Block adjacentBlock1, Block adjacentBlock2)
    {
        foreach (var matchingBlock in adjacentBlock1.MatchingBlocks) //blocks can have more than 1 match, all of them need checking
        {
            if (adjacentBlock2.MatchingBlocks.Contains(matchingBlock))
            {
                //if both blocks have a match, remove both of them from list (they can be different blocks that have more than 1 match, that is why remove both)
                RemoveBlockTypeFromList(possibleBlockTypes, adjacentBlock1);
                RemoveBlockTypeFromList(possibleBlockTypes, adjacentBlock2);

            }
        }
    }

    private static void RemoveBlockTypeFromList(List<Block> possibleBlockTypes, Block blockTypeToRemove)
    {
        for (int i = possibleBlockTypes.Count - 1; i >= 0; i--)
        {
            if (possibleBlockTypes[i].MatchingBlocks.Contains(blockTypeToRemove)) // remove only if still available on the list
                possibleBlockTypes.Remove(possibleBlockTypes[i]);
        }
    }

    private List<Block> GetPossibleBlockTypes()
    {
        List<Block> possibleBlockTypes = new List<Block>(LevelDefinition.BlockTypesToSpawn);
        for (int i = possibleBlockTypes.Count - 1; i >= 0; i--)
        {
            var blockType = possibleBlockTypes[i];

            //remove block types immediately if max allowed quanityt is reached
            if (blockType.BlockSet.Items.Count >= blockType.MaxOccurences.Value)
                possibleBlockTypes.Remove(blockType);
            else
            {
                //check if block has some probability defined (1 means that it will be taken into consideration always)
                if (blockType.SpawnProbability.Value != 1f)
                {
                    var roll = Random.Range(0f, 1f);
                    if (roll > blockType.SpawnProbability.Value)
                    {
                        possibleBlockTypes.Remove(blockType); // ensure that after unsuccessful roll the block will not be considered for placing
                    }
                }
            }
        }
        return possibleBlockTypes;
    }

    public void FillEmptyPlaces()
    {
        //scan the board by cloumns from bottom to the top
        for (int col = 0; col < GameBoard.HorizontalSize.Value; col++)
        {
            //initialize row offset - value that defines where to spawn new block (to look like it fall from the top of the board)
            int rowOffset = 0;

            for (int row = GameBoard.VerticalSize.Value - 1; row >= 0; row--)
            {

                //if current block is null (has been destroyed during matching)
                if (GameBoard.Grid[col, row] == null)
                {
                    //initialize blockFound flag - it defines if block for replacement has been found on the top or from existing blocks
                    bool blockFound = false;
                    //scan the blocks on top of empty place
                    for (int i = row - 1; i >= 0; i--)
                    {
                        //take the first block on the way to the top
                        if(GameBoard.Grid[col, i] != null)
                        {
                            //place the block on the empty place in grid found before, update its position properties
                            GameBoard.Grid[col, row] = GameBoard.Grid[col, i];
                            GameBoard.Grid[col, row].Column = col;
                            GameBoard.Grid[col, row].Row = row;

                            //make this block and empty place to allow other blocks on the top to take its place
                            GameBoard.Grid[col, i] = null;
                            blockFound = true;
                            break;
                        }
                    }
                    //if no block was found, new ones need to be spawned
                    if(!blockFound)
                    {
                        //calculate the offset position based on row which blocks will be dropped into (basically first block found from the bottom is the offset)
                        rowOffset = (row + 1) > rowOffset ? (row + 1) : rowOffset;

                        List<Block> possibleBlockTypes = GetPossibleBlockTypes();
                        //spawn block with offset
                        var blockSpawned = InstantiateBlock(possibleBlockTypes[Random.Range(0, possibleBlockTypes.Count)], col, row, rowOffset);
                        //place in grid
                        GameBoard.Grid[col, row] = blockSpawned;
                    }
                }

            }
        }
        BlocksGenerated.Raise(); //raise an event for initiate block drop
    }
}
