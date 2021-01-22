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
        GameBoard.Grid = new BlockElement[GameBoard.HorizontalSize.Value, GameBoard.VerticalSize.Value];

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
        for (int col = 0; col < GameBoard.HorizontalSize.Value; col++)
        {
            for (int row = 0; row < GameBoard.VerticalSize.Value; row++)
            {
                List<Block> possibleBlockTypes = GetPossibleBlockTypes();

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

                var blockSpawned = InstantiateBlock(possibleBlockTypes[Random.Range(0, possibleBlockTypes.Count)], new Vector3(col - (GameBoard.HorizontalSize.Value / 2f) + 0.5f, -row + (GameBoard.VerticalSize.Value / 2f) - 0.5f, 0f));
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
            var blockType = possibleBlockTypes[i];
            if (blockType.BlockSet.Items.Count >= blockType.MaxOccurences.Value)
                possibleBlockTypes.Remove(blockType);
            else
            {
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

        for (int col = 0; col < GameBoard.HorizontalSize.Value; col++)
        {
            int rowOffset = 0;

            for (int row = GameBoard.VerticalSize.Value - 1; row >= 0; row--)
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

                        var blockSpawned = InstantiateBlock(possibleBlockTypes[Random.Range(0, possibleBlockTypes.Count)], new Vector3(col - (GameBoard.HorizontalSize.Value / 2f) + 0.5f, -row + (GameBoard.VerticalSize.Value / 2f) - 0.5f + rowOffset, 0f));
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
