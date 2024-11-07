using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;


public class Grid : MonoBehaviour
{
    public List<Tile> tiles;
    public ShapeSO shapeSO;
    public int layer;
    public GridLayoutGroup gridLayoutGroup;
    public Tile tileObj;
    internal object canvasOrder;

    public event Action<int, Vector2Int, int> OnTileClick;
    void Start()
    {
       gridLayoutGroup = GetComponent<GridLayoutGroup>();
    }
    public void InitGrid()
    {
         gridLayoutGroup.constraintCount = shapeSO.rows;
        for (int i = 0; i < shapeSO.rows; i++)
        {
            for (int j = 0; j < shapeSO.columns; j++)
            {
                Tile tile = Instantiate(tileObj, transform);
                bool isBlocked = shapeSO.IsBlocked(i,j);
                Vector2Int locaion = new Vector2Int(i,j);
                tile.Init(locaion,layer);
                tile.SetStatus(isBlocked);
                if(!tile.isBlocked){
                     tiles.Add(tile);
                    tile.OnClick += OnGridTileClick;
                }
            }
        }
    }
    public void AddToList(List<Tile> tiles){
       foreach(Tile tile in this.tiles){
         tiles.Add(tile);
       }
    }
    public void InitLayer(int layer){
        this.layer = layer;
    }
    public void InitData(List<int> tilesId){;
    for(int i =0;i<tiles.Count;i++){
        if(!tiles[i].isQueued)
        {
            tiles[i].SetId(tilesId[i]);
        }
     }
    }
    public Tile GetTileAtPosition(Vector2Int position)
{
    foreach (Tile tile in tiles)
    {
        if (tile.location == position)
        {
            return tile;
        }
    }
    return null;
}
   
    public void OnGridTileClick(int id,Vector2Int location,int layer)
    {
        OnTileClick?.Invoke(id, location, layer);
    }
    
}
