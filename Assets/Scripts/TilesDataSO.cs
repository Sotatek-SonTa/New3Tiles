using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="TilDataSO",menuName ="ScriptableObject/TileCount")]
public class TilesDataSO : ScriptableObject
{
     public TileIdCount[] tileIdCounts;

}
[System.Serializable]
public class TileIdCount
{
    public int tileId;
    public int count;
}