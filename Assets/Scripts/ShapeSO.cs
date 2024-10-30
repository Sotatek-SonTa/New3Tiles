using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="ShapeSO",menuName ="ScriptableObject/ShapeData")]
public class ShapeSO : ScriptableObject
{
    public int rows;
    public int columns;
    public GameObject[,] matrix;
    public List<bool> blockedCells = new List<bool>();
    
    public void InitializeGrid()
    {
        blockedCells.Clear();
        for(int i = 0; i < rows * columns; i++)
        {
            blockedCells.Add(false);
        }
    }
    public bool IsBlocked(int row, int column)
    {
        int index = row * columns + column;
        return blockedCells[index];
    }
    public void SetBlocked(int row, int column, bool value)
    {
        int index = row * columns + column;
        blockedCells[index] = value;
    }
   public int GetBlockedTileCount(){
    int blockCount = 0;
    for(int i =0;i<rows;i++){
        for(int j=0;j<columns;j++){
            if(IsBlocked(i,j)){
                blockCount++;
            }
        }
    }
    return blockCount;
    }
}
