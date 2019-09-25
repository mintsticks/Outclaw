using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Outclaw.City
{

    public interface IPauseMenuManager
    {
        bool IsPaused { get; }
    }
    public class PauseMenuManager : MonoBehaviour, IPauseMenuManager
    {
        [SerializeField] private Canvas canvas;
        
        public bool IsPaused { get; set; }
        
        // Start is called before the first frame update
        void Start()
        {
            IsPaused = false;
            canvas.gameObject.SetActive(false);
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                IsPaused = !IsPaused;
                canvas.gameObject.SetActive(!canvas.gameObject.activeSelf);
            }
        }
    }
}
