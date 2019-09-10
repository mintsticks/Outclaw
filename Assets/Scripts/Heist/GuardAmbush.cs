using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Outclaw.Heist
{
    public class GuardAmbush : MonoBehaviour
    {

        private CapsuleCollider2D col;

        // Start is called before the first frame update
        void Start()
        {
            col = GetComponent<CapsuleCollider2D>();
        }

        // Update is called once per frame
        void Update()
        {
            
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                other.gameObject.GetComponent<PlayerController>().
                    InteractWithObject(PlayerController.InteractableType.GUARD, this.gameObject.transform.parent.parent.gameObject);
                other.gameObject.GetComponent<PlayerController>().IsInteracting = true;
            }
        }

        void OnTriggerExit2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                other.gameObject.GetComponent<PlayerController>().IsInteracting = false;
            }
        }
    }
}