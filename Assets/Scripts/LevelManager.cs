using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG;
using DG.Tweening;
using System.Linq;

public class LevelManager : MonoBehaviour
{
    public List<Grid> grids;
    public List<Tile> tiles;
    public List<Tile> queueTiles;
    public List<int> tilesId;
    public LevelData levelData;
    public int currentLevel=0;
    public Transform spawnPos;
    public Grid gridObj;
    public GameObject QueueTile;
   
    int currentTileIndex = 0;
     public TileManager tileManager;
    void Start()
    {
    levelData = Resources.Load<LevelData>($"Levels/Level{currentLevel}");
    InitData();
    }
    void InitData()
    {
       tilesId = levelData.GetTileIds();
    
       for(int i =0;i<levelData.shapeSOs.Count;i++){
        Grid grid = Instantiate(gridObj,spawnPos);
        grid.shapeSO = levelData.shapeSOs[i];
        grid.OnTileClick += HanldSelectedTiled;
        grid.InitLayer(i);
        grid.gameObject.name = $"Grid{i+1}";
        grid.InitGrid();
        grid.AddToList(tiles);
       //AssignTileId(grid);
        grids.Add(grid);
      }
      DisableOverLappedTiles(grids);
    }

    //Truyen du lieu spirte cho tile
    void AssignTileSprite(Grid grid, List<int> gridTileIds){
      for(int i =0;i<grid.tiles.Count;i++)
      {
        int tileId = gridTileIds[i];
        Sprite tileSprite = tileManager.GetSpriteById(tileId);
        if(!grid.tiles[i].isBlocked){
             grid.tiles[i].SetTileSprite(tileSprite);
        }
      }
    }

   //Truyen du lieu Id cho tile
    void AssignTileId(Grid grid)
    {
       int totalTilesGrid = grid.tiles.Count;
        if (tilesId.Count < totalTilesGrid)
    {
        Debug.LogError("Not enough tile IDs to assign to the grid.");
        return;
    }
       List<int> gridTileIds = tilesId.GetRange(currentTileIndex,totalTilesGrid);
       grid.InitData(gridTileIds);
       currentTileIndex+=totalTilesGrid;
       AssignTileSprite(grid,gridTileIds);
    }
    public void HanldSelectedTiled(int id, Vector2Int location, int layer){
          foreach(Grid grid in grids){
            if(grid.layer == layer){
              grid.gridLayoutGroup.enabled = false;
              foreach(Tile tile in grid.tiles){
                if(tile.location == location){
                 tile.transform.DOMove(QueueTile.transform.position,0.5f).OnComplete(()
                 =>{
                    tile.transform.SetParent(QueueTile.transform,true);
                    tile.SetRayCast(false);
                    tile.isQueued = true;
                    queueTiles.Add(tile);
                    EnableRayCastOverLappedTiles(tile.GetLocation(),tile.GetLayer(),grids);
                    DisableOverLappedTiles(grids);
                    HandleThreeMatching(tile);
                 });
                }
              }
            }
          }
    }
    public void HandleThreeMatching(Tile tile){
        int currentTileId = tile.GetId();
        int countSameId = queueTiles.Count(t => t.GetId() == tile.GetId());
        if(countSameId >=3){
           var tilesToRemove = queueTiles.Where(t=>t.GetId() == tile.GetId()).ToList();
           foreach(Tile t in tilesToRemove){
              t.gameObject.SetActive(false);
           }
           queueTiles = queueTiles.Where(t=>t.GetId() != tile.GetId()).ToList(); 
           
        }  
      }
      public void DisableOverLappedTiles(List<Grid> gridList)
      {
      for (int i = 0; i<gridList.Count-1; i++)
      {
        //Checking lien ke
        Grid lowererGrid = gridList[i];
        for(int j =i+1 ; j<gridList.Count;j+=2)
         {
           Grid upperGrid = gridList[j];
           HandleGridOverlap4Tiles(upperGrid, lowererGrid);
         }
        //Checking dan cach  
        for(int j =i+2 ; j<gridList.Count;j+=2)
         {
            Grid upperGrid2 = gridList[j];    
           HandleGridOverlapOneTile(upperGrid2, lowererGrid);
            
          }
      }
    }

    #region OverlapHandle
    private void HandleGridOverlap4Tiles(Grid upperGrid, Grid lowerGrid)
{
     List<Vector2Int> affectedPositions = new List<Vector2Int>();
    foreach (Tile upperTile in upperGrid.tiles)
    {
        if (!upperTile.isBlocked)
        {
            Vector2Int upperTilePos = upperTile.location;
            if(upperGrid.shapeSO.columns > lowerGrid.shapeSO.columns)
            {
               affectedPositions = BiggerOverLapSmaller(upperTilePos);
            }
            else if(upperGrid.shapeSO.columns< lowerGrid.shapeSO.columns)
            {
              affectedPositions = OverLapFourTiles(upperTilePos);
            }
            foreach (Vector2Int pos in affectedPositions)
            {
                Tile lowerTile = lowerGrid.GetTileAtPosition(pos);
                if (lowerTile != null && !upperTile.isQueued)
                {
                    lowerTile.SetRayCast(false);
                    lowerTile.SetInteractable(false);
                }
            }
        }
    }
} 
 private void HandleGridOverlapOneTile(Grid upperGrid,Grid lowerGrid)
 {
    foreach(Tile upperTile in upperGrid.tiles)
    {
      if(!upperTile.isBlocked)
      {
          Vector2Int upperTileLocation = upperTile.location;
          Vector2Int afftectedTIle = OverLapOneTile(upperTileLocation);

          Tile lowerTile = lowerGrid.GetTileAtPosition(afftectedTIle);
          if(lowerTile!=null && !upperTile.isQueued)
          {
              lowerTile.SetRayCast(false);
              lowerTile.SetInteractable(false);
          }
      }
    }
 }
 #endregion
      #region  OverLapPosition
      public List<Vector2Int> OverLapFourTiles(Vector2Int higherTilePos){
         List<Vector2Int> affectedTiles = new List<Vector2Int>();
           affectedTiles.Add(higherTilePos);
           affectedTiles.Add(new Vector2Int(higherTilePos.x + 1, higherTilePos.y)); 
           affectedTiles.Add(new Vector2Int(higherTilePos.x, higherTilePos.y + 1));
           affectedTiles.Add(new Vector2Int(higherTilePos.x + 1, higherTilePos.y + 1));
      return affectedTiles;
      }
      public List<Vector2Int> BiggerOverLapSmaller(Vector2Int higherTilePos)
      {
         List<Vector2Int> affectedTiles = new List<Vector2Int>();
           affectedTiles.Add(higherTilePos);
           affectedTiles.Add(new Vector2Int(higherTilePos.x - 1, higherTilePos.y)); 
           affectedTiles.Add(new Vector2Int(higherTilePos.x - 1, higherTilePos.y - 1)); 
           affectedTiles.Add(new Vector2Int(higherTilePos.x, higherTilePos.y - 1));
         return affectedTiles;
      }
      public Vector2Int OverLapOneTile(Vector2Int higherTilePos)
      {
        return higherTilePos;
      }
      #endregion

      #region EnableRayCast
 public void EnableRayCastOverLappedTiles(Vector2Int location, int layer, List<Grid> gridList)
  {
      for(int i = layer;i>=0;i--)
      {
        //Check lien ke
        Grid currentGrid = gridList[layer];
        for(int j =i-1;j>=0;j-=2)
        {
             Grid lowerGridNear = gridList[j];
             EnableRayCastOver4Tiles(currentGrid,lowerGridNear,location);
        }
        //Check dan cach
        for(int j =i-2;j>=0;j-=2)
        {
            Grid lowerGridDistance = gridList[j];
            EnableOverlapOneTile(lowerGridDistance,location);
        }

      }  
  }
    void EnableRayCastOver4Tiles(Grid currentGrid, Grid lowerGrid, Vector2Int tileLocation)
    {
      List<Vector2Int> affectedPositions = new List<Vector2Int>();
   
            Vector2Int upperTilePos = tileLocation;
            if(currentGrid.shapeSO.columns > lowerGrid.shapeSO.columns)
            {
               affectedPositions = BiggerOverLapSmaller(upperTilePos);
            }
            else if(currentGrid.shapeSO.columns< lowerGrid.shapeSO.columns)
            {
              affectedPositions = OverLapFourTiles(upperTilePos);
            }
            foreach (Vector2Int pos in affectedPositions)
            {
                Tile lowerTile = lowerGrid.GetTileAtPosition(pos);
                if (lowerTile != null)
                {
                    lowerTile.SetRayCast(true);
                    lowerTile.SetInteractable(true);
                }
            }
        }
     private void EnableOverlapOneTile(Grid lowerGrid, Vector2Int tileLocation)
 {
          Vector2Int upperTileLocation = tileLocation;
          Vector2Int afftectedTIle = OverLapOneTile(upperTileLocation);

          Tile lowerTile = lowerGrid.GetTileAtPosition(afftectedTIle);
          if(lowerTile!=null)
          {
              lowerTile.SetRayCast(true);
              lowerTile.SetInteractable(true);
          }
 }
      #endregion
}


