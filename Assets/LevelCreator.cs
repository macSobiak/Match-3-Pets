using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;

public class LevelCreator : MonoBehaviour
{
    public GameObject entityToSpawn;

    public LevelDefinition LevelDefinition;

    public GameBoard GameBoard;
    public GameEvent BoardUpdated;
    public GameEvent BlocksGenerated;
    // Start is called before the first frame update
    void Start()
    {
        GameBoard.Grid = new BlockElement[GameBoard.HorizontalSize.value, GameBoard.VerticalSize.value];

        PlaceBlocks();
    }

    private BlockElement InstantiateBlock(Block blockType, Vector3 position)
    {
        var blockToSpawn = entityToSpawn.GetComponent<BlockElement>();
        blockToSpawn.Block = blockType;

        GameObject currentEntity = Instantiate(entityToSpawn, position, Quaternion.identity);
        return currentEntity.GetComponent<BlockElement>();
    }

    private void PlaceBlocks()
    {
        for (int col = 0; col < GameBoard.HorizontalSize.value; col++)
        {
            for (int row = 0; row < GameBoard.VerticalSize.value; row++)
            {
                List<Block> possibleBlockTypes = GetPossibleBlockTypes();


                //Choose what sprite to use for this cell
                Block left1 = GameBoard.GetBlockTypeFromGrid(col - 1, row); //2
                Block left2 = GameBoard.GetBlockTypeFromGrid(col - 2, row);
                if (left1 != null)
                {
                    foreach (var blockToRemove in left1.CannotBeAjacentTo)
                        possibleBlockTypes.Remove(blockToRemove);
                }

                if (left2 != null) // 3
                {
                    foreach (var matchingBlock in left1.MatchingBlocks)
                    {
                        if (left2.MatchingBlocks.Contains(matchingBlock))
                        {
                            for (int i = possibleBlockTypes.Count - 1; i >= 0; i--)
                            {
                                if (possibleBlockTypes[i].MatchingBlocks.Contains(left1))
                                    possibleBlockTypes.Remove(possibleBlockTypes[i]);
                            }
                            for (int i = possibleBlockTypes.Count - 1; i >= 0; i--)
                            {
                                if (possibleBlockTypes[i].MatchingBlocks.Contains(left2))
                                    possibleBlockTypes.Remove(possibleBlockTypes[i]);
                            }
                        }
                    }

                }

                Block down1 = GameBoard.GetBlockTypeFromGrid(col, row - 1); // 5
                Block down2 = GameBoard.GetBlockTypeFromGrid(col, row - 2);
                if (down1 != null)
                {
                    foreach (var blockToRemove in down1.CannotBeAjacentTo)
                        possibleBlockTypes.Remove(blockToRemove);
                }

                if (down2 != null)
                {
                    foreach (var matchingBlock in down1.MatchingBlocks)
                    {
                        if (down2.MatchingBlocks.Contains(matchingBlock))
                        {
                            for (int i = possibleBlockTypes.Count - 1; i >= 0; i--)
                            {
                                if (possibleBlockTypes[i].MatchingBlocks.Contains(down1))
                                    possibleBlockTypes.Remove(possibleBlockTypes[i]);
                            }
                            for (int i = possibleBlockTypes.Count - 1; i >= 0; i--)
                            {
                                if (possibleBlockTypes[i].MatchingBlocks.Contains(down2))
                                    possibleBlockTypes.Remove(possibleBlockTypes[i]);
                            }
                        }
                    }
                }

                var blockSpawned = InstantiateBlock(possibleBlockTypes[Random.Range(0, possibleBlockTypes.Count)], new Vector3(col - (GameBoard.HorizontalSize.value / 2f) + 0.5f, -row + (GameBoard.VerticalSize.value / 2f) - 0.5f, 0f));
                blockSpawned.Column = col;
                blockSpawned.Row = row;

                GameBoard.Grid[col, row] = blockSpawned;
            }
        }
        BoardUpdated.Raise();
    }

    private List<Block> GetPossibleBlockTypes()
    {
        List<Block> possibleBlockTypes = new List<Block>(LevelDefinition.BlockTypesToSpawn);
        for (int i = possibleBlockTypes.Count - 1; i >= 0; i--)
        {
            if (possibleBlockTypes[i].BlockSet.Items.Count >= possibleBlockTypes[i].MaxOccurences.value)
                possibleBlockTypes.Remove(possibleBlockTypes[i]);
        }

        return possibleBlockTypes;
    }

    //Block GetBlockTypeFromGrid(int col, int row)
    //{
    //    if (col > GameBoard.HorizontalSize.value || col < 0
    //        || row > GameBoard.VerticalSize.value || row < 0)
    //        return null;
    //    return GameBoard.Grid[col, row].Block;
    //}

    public void FillEmptyPlaces()
    {

        for (int col = 0; col < GameBoard.HorizontalSize.value; col++)
        {
            int rowOffset = 0;

            for (int row = GameBoard.VerticalSize.value - 1; row >= 0; row--)
            {
                if (GameBoard.Grid[col, row] == null)
                {
                    bool blockFound = false; ;
                    for(int i = row - 1; i >= 0; i--)
                    {
                        if(GameBoard.Grid[col, i] != null)
                        {
                            GameBoard.Grid[col, row] = GameBoard.Grid[col, i];
                            GameBoard.Grid[col, i].Column = col;
                            GameBoard.Grid[col, i].Row = row;
                            GameBoard.Grid[col, i] = null;
                            blockFound = true;
                            break;
                        }
                    }
                    if(!blockFound)
                    {
                        rowOffset = (row + 1) > rowOffset ? (row + 1) : rowOffset;
                        List<Block> possibleBlockTypes = GetPossibleBlockTypes();

                        var blockSpawned = InstantiateBlock(possibleBlockTypes[Random.Range(0, possibleBlockTypes.Count)], new Vector3(col - (GameBoard.HorizontalSize.value / 2f) + 0.5f, -row + (GameBoard.VerticalSize.value / 2f) - 0.5f + rowOffset, 0f));
                        blockSpawned.Column = col;
                        blockSpawned.Row = row;

                        GameBoard.Grid[col, row] = blockSpawned;
                    }
                }

            }
        }
        BlocksGenerated.Raise();
    }
}
