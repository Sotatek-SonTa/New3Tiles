using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="LevelData",menuName ="ScriptableObject/LevelData")]
public class LevelData : ScriptableObject
{
    public List<ShapeSO> shapeSOs;
    public int totalTiles;
     public int CountTotalTile(){
        totalTiles =0;
        foreach(ShapeSO shape in shapeSOs){
            int blockCount = shape.GetBlockedTileCount();
            int availableTIles = (shape.rows * shape.columns) - blockCount;
            totalTiles += availableTIles;
        }
        return totalTiles;
    }   
}
