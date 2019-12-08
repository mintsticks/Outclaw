using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Outclaw.UI{
  public abstract class AbstractMouseMenuItem : MonoBehaviour, IMenuItem, 
    IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler,
    ISubmitHandler, IPointerClickHandler
  {
    [SerializeField] private MenuItemText itemText;

    private Menu menu;
    private bool isMouseOver;
    private bool isHeld;

    public void InitMenu(Menu menu){
      this.menu = menu;
    }
    
    public abstract void Select();

    public virtual void Hover() {
      itemText.Hover();
    }

    public virtual void Unhover() {
      itemText.Unhover();
    }

    public virtual void Hold(){
      itemText.Hold();
    }

    public void OnPointerEnter(PointerEventData pointerEventData){
      if(!menu.Selectable){
        return;
      }
      menu.SelectItem(this);
      isMouseOver = true;

      pointerEventData.Use();
    }

    public void OnPointerExit(PointerEventData pointerEventData){
      if(!menu.Selectable){
        return;
      }
      isMouseOver = false;
      if(isHeld){
        isHeld = false;
        Hover();
      }
      pointerEventData.Use();
    }

    public void OnPointerDown(PointerEventData pointerEventData){
      if(!menu.Selectable || pointerEventData.button != PointerEventData.InputButton.Left){
        return;
      }
      isMouseOver = false;
      isHeld = true;
      Hold();

      pointerEventData.Use();
    }

    public void OnPointerUp(PointerEventData pointerEventData){
      if(!menu.Selectable || pointerEventData.button != PointerEventData.InputButton.Left){
        return;
      }

      if(isHeld){
        Hover();
      }
      isHeld = false;
      if(isMouseOver){
        Select();
      }

      pointerEventData.Use();
    }

    public void OnPointerClick(PointerEventData pointerEventData){
      if(!menu.Selectable || pointerEventData.button != PointerEventData.InputButton.Left){
        return;
      }
      Select();

      pointerEventData.Use();
    }

    public void OnSubmit(BaseEventData data){
      if(!menu.Selectable){
        return;
      }
      Select();

      data.Use();
    }
  }
}
