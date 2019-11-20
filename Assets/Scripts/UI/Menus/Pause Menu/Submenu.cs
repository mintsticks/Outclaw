using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using Outclaw.UI;
using Utility;

namespace Outclaw {
  public class Submenu : Menu {

    [SerializeField] 
    private EventMenuItem exitItem;
    
    private List<IMenuItem> items;
    protected override IMenuItem this[int i] { get => items[i]; }

    protected override int ItemCount() => items.Count;

    public bool Active
    {
      get => active;
      set => active = value;
    }

    private void Awake() {
      items = new List<IMenuItem> {exitItem};
      currentIndex = 0;
      items[0].Hover();
      //contents.alpha = 0;
    }
    
    void Update() {
      CheckSelectionState();
    }
  }
}