using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;

public class LevelCreator : MonoBehaviour
{
    public GameObject entityToSpawn;

    public LevelDefinition LevelDefinition;
    public BlocksRuntimeSet BlocksInGame;

    public GameBoard  GameBoard;
    // Start is called before the first frame update
    void Start()
    {
        GameBoard.Grid = new BlockElement[GameBoard.HorizontalSize.value, GameBoard.VerticalSize.value];

        PlaceBlocks();
    }

    private void InstantiateBlock(Block blockType)
    {
        var blockToSpawn = entityToSpawn.GetComponent<BlockElement>();
        blockToSpawn.Block = blockType;

        GameObject currentEntity = Instantiate(entityToSpawn, Vector3.zero, Quaternion.identity);

        currentEntity.name = blockType.name;
    }

    private void PlaceBlocks()
    {
        int blockIdx = 0;
        for (int col = 0; col < GameBoard.HorizontalSize.value; col++)
        {
            for (int row = 0; row < GameBoard.VerticalSize.value; row++)
            {
                List<Block> possibleBlockTypes = new List<Block>(LevelDefinition.BlockTypesToSpawn);
                for (int i = possibleBlockTypes.Count - 1; i >= 0; i--)
                {
                    if (possibleBlockTypes[i].BlockSet.Items.Count >= possibleBlockTypes[i].MaxOccurences.value)
                        possibleBlockTypes.Remove(possibleBlockTypes[i]);
                }


                //Choose what sprite to use for this cell
                Block left1 = GetBlockTypeFromGrid(col - 1, row); //2
                Block left2 = GetBlockTypeFromGrid(col - 2, row);
                if(left1 != null)
                {
                    foreach (var blockToRemove in left1.CannotBeAjacentTo)
                        possibleBlockTypes.Remove(blockToRemove);
                }

                if (left2 != null) // 3
                {
                    foreach (var matchingBlock in left1.MatchingBlocks)
                    {
                        if(left2.MatchingBlocks.Contains(matchingBlock))
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

                Block down1 = GetBlockTypeFromGrid(col, row - 1); // 5
                Block down2 = GetBlockTypeFromGrid(col, row - 2);
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

                InstantiateBlock(possibleBlockTypes[Random.Range(0, possibleBlockTypes.Count)]);

                BlocksInGame.Items[blockIdx].transform.position = new Vector3(col - (GameBoard.HorizontalSize.value / 2f) + 0.5f, -row + (GameBoard.VerticalSize.value / 2f) - 0.5f, 0f);
                BlocksInGame.Items[blockIdx].Column = col;
                BlocksInGame.Items[blockIdx].Row = row;

                GameBoard.Grid[col, row] = BlocksInGame.Items[blockIdx];

                print(GameBoard.Grid[col, row].GetComponent<BlockElement>().name + " (" + col + ", " + row + ")");

                blockIdx++;

            }
        }
    }

    Block GetBlockTypeFromGrid(int col, int row)
    {
        if (col > GameBoard.HorizontalSize.value || col < 0
            || row > GameBoard.VerticalSize.value || row < 0)
            return null;
        return GameBoard.Grid[col, row].Block;
    }

}
