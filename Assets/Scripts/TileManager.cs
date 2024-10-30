using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class TileManager : MonoBehaviour
{
    public SpriteAtlas spriteAtlas;
    public Dictionary<int, string> tileSprites;
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
}
