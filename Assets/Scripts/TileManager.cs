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
            {0,"Apple"},
            {1,"Banana"},
            {2,"Blueberry" },
            {3,"Cherry"},
            {4,"Coconut"},
            {5,"Dragonfruit" },
            {6,"Grape" },
            {7,"Kiwi" },
            {8,"Orange" },
            {9,"Peach" },
            {10,"Pepper" },
            {11,"Pineapple" },
            {12,"Raspberry" },
            {13,"Watermelon" },
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
