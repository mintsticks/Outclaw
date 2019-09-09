using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Outclaw.Heist
{
   public class PlayerController : MonoBehaviour
    {

        public float speed;
        private Rigidbody2D rbody;
        private bool isInteracting;
        private InteractableType interactType;
        private GameObject interactObj;

        public enum InteractableType
        {
            NONE,
            OBJECTIVE,
            VENT,
            GUARD,
            COVER,
            EXIT
        }

        public bool IsInteracting
        {
            get 
            {
                return isInteracting;
            }
            set 
            {
                // isInteracting = value;
                // if (isInteracting)
                // {
                //     Debug.Log("Interact!");
                // }
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            rbody = GetComponent<Rigidbody2D>();
            isInteracting = false;
            interactType = InteractableType.NONE;
            interactObj = null;
        }

        // Update is called once per frame
        void Update()
        {
            if (isInteracting && Input.GetKey(KeyCode.Space))
            {
                if (interactType == InteractableType.OBJECTIVE)
                {
                    interactObj.GetComponent<Objective>().IsComplete = true;
                    interactObj = null;
                    interactType = InteractableType.NONE;
                    isInteracting = false;
                }
            }
        }

        void FixedUpdate()
        {
            float moveHorizontal = Input.GetAxis("Horizontal");
            float moveVertical = Input.GetAxis("Vertical");

            Vector2 movement = new Vector2(moveHorizontal, moveVertical);

            rbody.velocity = (movement * speed);
        }

        public void InteractWithObject(InteractableType type, GameObject obj)
        {
            interactType = type;
            interactObj = obj;
        }
    } 
}


