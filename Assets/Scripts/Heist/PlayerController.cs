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
        private bool isObjectiveComplete;
        public float ambushCooldown;
        public float ventCooldown;
        
        private bool VentUsable
        {
            get;
            set;
        }

        private bool AmbushUsable
        {
            get;
            set;
        }

        [SerializeField] private GameObject visuals = null;
        private PlayerAnimController animController = null;

        [SerializeField] private SceneSwitcher switcher = null;
        public string exitScene = "Main";


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
            isObjectiveComplete = false;
            animController = visuals.GetComponent<PlayerAnimController>();
            VentUsable = true;
            AmbushUsable = true;
        }

        // Update is called once per frame
        void Update()
        {
            if (isInteracting && Input.GetKeyDown(KeyCode.Space))
            {
                if (interactType == InteractableType.OBJECTIVE)
                {
                    interactObj.GetComponent<Objective>().IsComplete = true;
                    isObjectiveComplete = true;
                    interactObj = null;
                    interactType = InteractableType.NONE;
                    isInteracting = false;
                }
                else if (interactType == InteractableType.VENT)
                {
                    if (VentUsable)
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

                        StartCoroutine(VentCooldown(ventCooldown));
                    }
                }
                else if (interactType == InteractableType.GUARD)
                {
                    if (AmbushUsable)
                    {
                        Destroy(interactObj);

                        interactObj = null;
                        interactType = InteractableType.NONE;
                        isInteracting = false;

                        StartCoroutine(AmbushCooldown(ambushCooldown));
                    }
                }
                else if (interactType == InteractableType.EXIT)
                {
                    if (isObjectiveComplete)
                    {
                        //TODO SCENE EXIT
                        switcher.SwitchToScene(exitScene);
                    }
                }
            }

            if (Input.GetKeyDown(KeyCode.X))
            {
                this.gameObject.GetComponent<SenseAbility>().UseAbility();
            }
        }

        void FixedUpdate()
        {
            float moveHorizontal = Input.GetAxis("Horizontal");
            float moveVertical = Input.GetAxis("Vertical");

            Vector2 movement = new Vector2(moveHorizontal, moveVertical);

            Vector2 vel = (movement * speed);
            rbody.velocity = vel;
            animController.SetHorizontalVelocity(vel.magnitude);
        }

        public void InteractWithObject(InteractableType type, GameObject obj)
        {
            interactType = type;
            interactObj = obj;
        }

        private IEnumerator VentCooldown(float time){
            VentUsable = false;
            yield return new WaitForSeconds(time);
            VentUsable = true;
            yield break;
        }

        private IEnumerator AmbushCooldown(float time){
            AmbushUsable = false;
            yield return new WaitForSeconds(time);
            AmbushUsable = true;
            yield break;
        }
    } 
}


