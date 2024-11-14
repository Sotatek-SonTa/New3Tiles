using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG;
using DG.Tweening;
using System.Linq;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public List<Grid> grids;
    public List<Tile> tiles;
    public List<Tile> queueTiles;
    public List<int> tilesId;
    public LevelData levelData;
    public Transform spawnPos;
    public Grid gridObj;
    public GameObject QueueTile;
    public TileManager tileManager;
    public UIManager uIManager;

    public static Vector2 firstPosition = new Vector2();

    public Tile lastSelectedTile;

    [Header("Ingame data")]
    public int currentLevel=0;
    [SerializeField]private int tileQueueContains = 6;
    void Start()
    {
    firstPosition.y = QueueTile.GetComponent<RectTransform>().anchoredPosition.y + QueueTile.GetComponent<RectTransform>().rect.height/2;
    uIManager.addOneMoreSlot.onClick.AddListener(AddOneMoreSlot);
    uIManager.reverseMove.onClick.AddListener(ReverseLastMove);
    uIManager.shuffleTiles.onClick.AddListener(ShuffleTiles);
    uIManager.NextLevel.onClick.AddListener(NextLevel);
    InitData();
    }
    void InitData()
    { 
       levelData = Resources.Load<LevelData>($"Levels/Level {currentLevel}");
       tileQueueContains = 6;
       tiles.Clear();
       tilesId.Clear();
       tilesId = tileManager.GetTilesIds(levelData);
       for(int i =0;i<levelData.shapeSOs.Count;i++){
        Grid grid = Instantiate(gridObj,spawnPos);
        grid.shapeSO = levelData.shapeSOs[i];
        grid.OnTileClick += HanldSelectedTiled;
        grid.InitLayer(i);
        grid.gameObject.name = $"Grid{i+1}";
        grid.InitGrid();
        grid.InitData(tilesId);
        grid.AddToList(tiles);
        grids.Add(grid);
      }
      AssignTileId();
      DisableOverLappedTiles(grids);
    }
 

   //Truyen du lieu sprite cho tile
    void AssignTileId()
    {
     List<int> tilesId = tileManager.GetTilesIds(levelData);
     for(int i =0;i<tiles.Count;i++)
     {
      int tileId = tiles[i].GetId();
      Sprite tileSprite = tileManager.GetSpriteById(tileId);
      if(!tiles[i].isBlocked)
      {
        tiles[i].SetTileSprite(tileSprite);
      }
     }
    }
    public void HanldSelectedTiled(int id, Vector2Int location, int layer){
          if(queueTiles.Count >tileQueueContains)
          {
            return;
          }   
          foreach(Grid grid in grids){
            if(grid.layer == layer){
              grid.gridLayoutGroup.enabled = false;
              foreach(Tile choosenTile in grid.tiles){
                if(choosenTile.location == location){
                  choosenTile.saveRectTransform = choosenTile.rectTransform.anchoredPosition;
                  lastSelectedTile = choosenTile;
                  Debug.Log(lastSelectedTile.saveRectTransform);
                  firstPosition.x = choosenTile.rectTransform.rect.width/2;
                  if(queueTiles.Count==0)
                  {
                      HandleTileBehaviour(choosenTile);
                      choosenTile.rectTransform.DOAnchorPos(firstPosition,0.2f);
                  }
                  else{
                    Tile findlastTile = queueTiles.FindLast(choosenTile => choosenTile.GetId() == id);
                    Vector2 nextTile = new Vector2();
                    if(findlastTile==null)
                    {
                       nextTile = queueTiles[queueTiles.Count-1].rectTransform.anchoredPosition;
                    }else
                    {
                       nextTile = findlastTile.rectTransform.anchoredPosition;
                    }    
                    nextTile.x += choosenTile.rectTransform.rect.width;
                    HandleTileBehaviour(choosenTile,findlastTile);

                    //Di chuyen các tile trong queueTile
                    for(int i = queueTiles.IndexOf(choosenTile)+1;i<queueTiles.Count;i++)
                    {
                     Vector2 newPosition = queueTiles[i].rectTransform.anchoredPosition;
                     newPosition.x += choosenTile.rectTransform.rect.width ;
                     newPosition.y = firstPosition.y;
                     queueTiles[i].rectTransform.DOAnchorPos(newPosition,0.2f);
                   }

                   //Di chuyen choosenTile
                    choosenTile.rectTransform.DOAnchorPos(nextTile,0.2f).OnComplete(()
                    =>{
                    HandleThreeMatching(choosenTile);
                    Debug.Log(queueTiles.IndexOf(choosenTile));

                    if(tiles.Count ==0)
                    {
                      uIManager.SetActiveUIWin(true);
                    }
                  });
                  }
                }
              }
            }
          }
    }
    public void HandleTileBehaviour(Tile tile, Tile lastTile =null)
    {

      int index = queueTiles.IndexOf(lastTile);
      tile.SetRayCast(false);
      tile.isQueued = true;
      tiles.Remove(tile);
      if(lastTile == null)
      {
        queueTiles.Add(tile);   
      }
      else
      {
       queueTiles.Insert(index+1,tile);
      }
      tilesId.Remove(tile.GetId());
      EnableRayCastOverLappedTiles(tile.GetLocation(),tile.GetLayer(),grids);
      DisableOverLappedTiles(grids);
    }
    public void HandleThreeMatching(Tile tile){
        int currentTileId = tile.GetId();
        int countSameId = queueTiles.Count(t => t.GetId() == tile.GetId());
        if(countSameId >=3){
           var tilesToRemove = queueTiles.Where(t=>t.GetId() == tile.GetId()).ToList();
             for(int i = queueTiles.IndexOf(tilesToRemove[0])+3;i<queueTiles.Count;i++)
            {
            queueTiles[i].rectTransform.DOAnchorPos(queueTiles[i-3].rectTransform.anchoredPosition,0.2f);
            }
            foreach(Tile t in tilesToRemove)
            {
              Destroy(t.gameObject);
              queueTiles.Remove(t);
              tiles.Remove(t);
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
            } else 
            {
              affectedPositions.Add(OverLapOneTile(upperTilePos));
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
    public void AddOneMoreSlot()
    {
      tileQueueContains = 7;
      uIManager.SetAddSlotButton(false);
    }
    #region Reverse
    public void ReverseLastMove()
    {
      if(queueTiles.Count==0)
      {
        return;
      }
      if(lastSelectedTile !=null)
      {
        HandleRevereseTile(lastSelectedTile);
        Debug.Log(queueTiles.IndexOf(lastSelectedTile));
        for(int i = queueTiles.IndexOf(lastSelectedTile)+1;i<queueTiles.Count;i++)
        {
            queueTiles[i].rectTransform.DOAnchorPos(queueTiles[i-1].rectTransform.anchoredPosition,0.2f);
        }
        lastSelectedTile = null;  
      }
      else
      {
      Tile reversedTile = queueTiles[queueTiles.Count-1];
      reversedTile.transform.SetParent(grids[reversedTile.GetLayer()].transform,true);
      reversedTile.rectTransform.DOAnchorPos(reversedTile.saveRectTransform,0.5f).OnComplete(()
        =>{
            tiles.Add(reversedTile);
            queueTiles.Remove(reversedTile);
            tilesId.Add(reversedTile.GetId());
            reversedTile.SetRayCast(true);
            reversedTile.SetInteractable(true);
            reversedTile.isQueued = false;
            DisableOverLappedTiles(grids);
          }
      );
      }
      
    }
    public void HandleRevereseTile(Tile reverseTile)
    {
       reverseTile.rectTransform.DOAnchorPos(reverseTile.saveRectTransform,0.5f).OnComplete(()
       =>
       {
         tiles.Add(reverseTile);
         queueTiles.Remove(reverseTile);
         tilesId.Add(reverseTile.GetId());
         reverseTile.SetRayCast(true);
         reverseTile.SetInteractable(true);
         DisableOverLappedTiles(grids);
       }
       );
    }
    #endregion
    #region  Shuffle
    public void ShuffleTiles()
    {
      tileManager.Shuffle(tilesId);
    for(int i =0;i<tiles.Count;i++)
     {
       if(!tiles[i].isQueued)
      {
        tiles[i].SetId(tilesId[i]);
        int tileId = tiles[i].GetId();
      Sprite tileSprite = tileManager.GetSpriteById(tileId);
      if(!tiles[i].isBlocked)
      {
        tiles[i].SetTileSprite(tileSprite);
      }
      }
     }
    }
    #endregion
    #region InnitNewLevel
    public void NextLevel()
    {
      uIManager.SetAddSlotButton(true);
      foreach(Grid grid in grids)
      {
        Destroy(grid.gameObject);

      }
      grids.Clear();
      currentLevel++;
      InitData();
      uIManager.SetActiveUIWin(false);
      uIManager.SetAddSlotButton(true);
    }
    #endregion
}


