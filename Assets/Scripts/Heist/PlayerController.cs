using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Outclaw.Heist
{
   public class PlayerController : MonoBehaviour
    {

        public Vector3 ventOffset;
        public float speed;
        private Rigidbody2D rbody;
        private bool isInteracting;
        private InteractableType interactType;
        private GameObject interactObj;

        [SerializeField] private GameObject visuals = null;
        private Animator anim = null;

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
                isInteracting = value;
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            rbody = GetComponent<Rigidbody2D>();
            isInteracting = false;
            interactType = InteractableType.NONE;
            interactObj = null;

            anim = visuals.GetComponent<Animator>();
        }

        // Update is called once per frame
        void Update()
        {
            if (isInteracting && Input.GetKeyDown(KeyCode.Space))
            {
                if (interactType == InteractableType.OBJECTIVE)
                {
                    interactObj.GetComponent<Objective>().IsComplete = true;
                    interactObj = null;
                    interactType = InteractableType.NONE;
                    isInteracting = false;
                }
                else if (interactType == InteractableType.VENT)
                {
                    //move position to other vent
                    string vent1 = "Vent1";
                    string vent2 = "Vent2";
                    string ventName = interactObj.name;
                    string otherVentName = (ventName == vent1 ? vent2 : vent1);
                    GameObject otherVent = interactObj.transform.parent.Find(otherVentName).gameObject;

                    this.gameObject.transform.position = otherVent.transform.position + ventOffset;

                    interactObj = null;
                    interactType = InteractableType.NONE;
                    isInteracting = false;
                }
                else if (interactType == InteractableType.GUARD)
                {
                    Destroy(interactObj);

                    interactObj = null;
                    interactType = InteractableType.NONE;
                    isInteracting = false;
                }
                else if (interactType == InteractableType.EXIT)
                {
                    //TODO SCENE EXIT
                }
            }
        }

        void FixedUpdate()
        {
            float moveHorizontal = Input.GetAxis("Horizontal");
            float moveVertical = Input.GetAxis("Vertical");

            Vector2 movement = new Vector2(moveHorizontal, moveVertical);

            Vector2 vel = (movement * speed);
            rbody.velocity = vel;
            anim.SetFloat("moveSpeed", vel.magnitude);
        }

        public void InteractWithObject(InteractableType type, GameObject obj)
        {
            interactType = type;
            interactObj = obj;
        }
    } 
}


