using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.U2D;

public class TileManager : MonoBehaviour
{
    public SpriteAtlas spriteAtlas;
    public Dictionary<int, string> tileSprites;
    public int totalTiles;
    void Awake()
    {
        tileSprites = new Dictionary<int, string>();
        LoadTileSprite();
    }
    public Sprite GetSpriteById(int tileId)
    {
       if(tileSprites==null){
        LoadTileSprite();
       }

        string spriteName = tileSprites[tileId];
        Sprite sprite = spriteAtlas.GetSprite(spriteName);
        return sprite;

    }
    void LoadTileSprite()
    {
        tileSprites = new Dictionary<int, string>
        {
            {1,"001"},
            {2,"002"},
            {3,"003"},
            {4,"004"},
            {5,"005"},
            {6,"006"},
            {7,"007"},
            {8,"008"},
            {9,"009"},
            {10,"010"},
            {11,"011"},
            {12,"012"},
            {13,"013"},
            {14,"014"},
            {15,"015"},
            {16,"016"},
            {17,"017"},
            {18,"018"},
            {19,"019"},
            {20,"020"},
            {21,"021"},
            {22,"022"},
            {23,"023"},
            {24,"024"},
            {25,"025"},
            {26,"026"},
            {27,"027"},
            {28,"028"},
            {29,"029"},
            {30,"030"},
            {31,"031"},
            {32,"032"},
            {33,"033"},
            {34,"034"},
            {35,"035"},
            {36,"036"},
        };
    }
    public List<int> GetTilesIds(LevelData levelData){
        totalTiles = levelData.CountTotalTile();
        if(totalTiles %3!=0)
        {
            Debug.Log("Khong thoa man dieu kien chia het cho 3");
            return null;
        }
        List<int> tilesId = new List<int>();
        List<int> availableTileIds = new List<int>(tileSprites.Keys);
         while (tilesId.Count < totalTiles)
      {
        int randomTileId = availableTileIds[Random.Range(0, availableTileIds.Count)];

        int count = tilesId.Count(id => id==randomTileId);
        if (count % 3 == 0 || count == 0)
        {
            for (int i = 0; i < 3; i++)
            {
                if (tilesId.Count <= totalTiles)
                {
                    tilesId.Add(randomTileId);
                }
            }
        }
    }

    Shuffle(tilesId);

        return tilesId;
    }
    public void Shuffle(List<int> list)
    {
      for (int i = list.Count - 1; i > 0; i--)
      {
        int rnd = Random.Range(0, i + 1);
        int temp = list[i];
        list[i] = list[rnd];
        list[rnd] = temp;
      }
   }
}
