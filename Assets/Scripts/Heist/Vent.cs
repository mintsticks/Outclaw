using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Outclaw.Heist
{
    public class Vent : MonoBehaviour
    {

        private CircleCollider2D col;

        // Start is called before the first frame update
        void Start()
        {
            col = GetComponent<CircleCollider2D>();
        }

        // Update is called once per frame
        void Update()
        {
            
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            other.gameObject.GetComponent<PlayerController>().
                InteractWithObject(PlayerController.InteractableType.VENT, this.gameObject);
            other.gameObject.GetComponent<PlayerController>().IsInteracting = true;
        }

        void OnTriggerExit2D(Collider2D other)
        {
            other.gameObject.GetComponent<PlayerController>().IsInteracting = false;
        }
    }
}
