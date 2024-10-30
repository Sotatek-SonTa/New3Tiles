using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="LevelData",menuName ="ScriptableObject/LevelData")]
public class LevelData : ScriptableObject
{
    public List<ShapeSO> shapeSOs;
    public int totalTiles;
    public int totalTilesType;
    public TilesDataSO tilesDataSO;
    public Dictionary<int,int> tileidCountDictionary = new Dictionary<int, int>();
    public void CountTotalTile(){
        totalTiles =0;
        foreach(ShapeSO shape in shapeSOs){
            int blockCount = shape.GetBlockedTileCount();
            int availableTIles = (shape.rows * shape.columns) - blockCount;
            totalTiles += availableTIles;
        }
        foreach(TileIdCount tileIdCount in tilesDataSO.tileIdCounts)
        {
            tileidCountDictionary.Add(tileIdCount.tileId, tileIdCount.count);
        }
    }
    public List<int> GetTileIds(){
        List<int> tilesId = new List<int>();
        CountTotalTile();
        if(tilesId!=null){
        }
          foreach (KeyValuePair<int, int> tileIdCount in tileidCountDictionary)
        {
        int tileId = tileIdCount.Key;
        int count = tileIdCount.Value;

        for (int i = 0; i < totalTiles; i++)
        {
            tilesId.Add(tileId);
        }
       }
        Shuffle(tilesId);
        DebugTilesId(tilesId);
        Debug.Log(tilesId.Count);
        return tilesId;
  
    }
    private void Shuffle(List<int> list)
{
    for (int i = list.Count - 1; i > 0; i--)
    {
        int rnd = Random.Range(0, i + 1);
        int temp = list[i];
        list[i] = list[rnd];
        list[rnd] = temp;
    }
}
private void DebugTilesId(List<int> tilesId)
{

    string tilesIdString = string.Join(", ", tilesId);
    Debug.Log("tilesId: [" + tilesIdString + "]");
}

}
