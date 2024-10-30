using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class Tile : MonoBehaviour
{
    [SerializeField]  int id;
    [SerializeField] Image myImage;
    [SerializeField] Image bg;
    [SerializeField] public Vector2Int location;
    [SerializeField] int layer;
    [SerializeField] public bool isBlocked;
    [SerializeField] public RectTransform rectTransform;

   [SerializeField] public int canvasOrder;
   [SerializeField] public bool isQueued;
   [SerializeField] public Button button;

    public event Action<int,Vector2Int,int> OnClick;
     
    void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(ButtonClick);
        rectTransform = GetComponent<RectTransform>();
    }
    public void Init(Vector2Int location, int layer)
    {
        this.location = location;
        this.layer = layer;
    }
    public void SetId(int id){
        this.id = id;
    }
    public int GetId()
    {
        return this.id;
    }
    public int GetLayer(){
        return this.layer;
    }
    public Vector2Int GetLocation(){
        return this.location;
    }
    public void SetTileSprite(Sprite sprite){
        myImage.sprite = sprite;
    }
    public void SetRayCast(bool status){
        myImage.raycastTarget = status;
        bg.raycastTarget = status;

    }
    public void SetInteractable(bool status)
    {
        button.interactable = status;
    }    
    public void SetStatus(bool clickAble)
    {
       isBlocked = clickAble;
       if(isBlocked)
       {
          GetComponent<Button>().interactable = false;
          myImage.enabled = false;
          bg.enabled = false;
       }
       else
       {
         GetComponent<Button>().interactable = true;
          myImage.enabled = true;
          bg.enabled = true;
       }
    }
    public void SetBackGround(){
        
    }
       void ButtonClick()
    {
        OnClick?.Invoke(id,location,layer);
    }

    internal float GetWidth()
    {
       return rectTransform.rect.width;
    }

    internal float GetHeight()
    {
       return rectTransform.rect.height;
    }
}
