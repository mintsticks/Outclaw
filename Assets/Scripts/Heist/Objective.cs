using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Outclaw.Heist
{
    public class Objective : MonoBehaviour
    {
        private CircleCollider2D col;
        private Rigidbody2D rbody;
        private SpriteRenderer spriteRend;
        private GameObject promptObj;
        public GameObject prompt;
        public float offset;

        private bool isComplete;

        public bool IsComplete
        {
            get
            {
                return isComplete;
            }
            set
            {
                isComplete = value;
                if (isComplete)
                {
                    promptObj.GetComponent<SpriteRenderer>().color = Color.green;
                }
                else
                {
                    promptObj.GetComponent<SpriteRenderer>().color = Color.red;
                }
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            col = GetComponent<CircleCollider2D>();
            rbody = GetComponent<Rigidbody2D>();
            spriteRend = GetComponent<SpriteRenderer>();
            isComplete = false;
        }

        // Update is called once per frame
        void Update()
        {
            
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            promptObj = Instantiate(prompt, new Vector2(transform.position.x, transform.position.y + offset),
            Quaternion.identity, this.transform);
            if (isComplete)
            {
                promptObj.GetComponent<SpriteRenderer>().color = Color.green;
            }
            else
            {
                promptObj.GetComponent<SpriteRenderer>().color = Color.red;
                other.gameObject.GetComponent<PlayerController>().
                    InteractWithObject(PlayerController.InteractableType.OBJECTIVE, this.gameObject);
                other.gameObject.GetComponent<PlayerController>().IsInteracting = true;
            }
        }

        void OnTriggerExit2D(Collider2D other)
        {
            Destroy(promptObj);
            other.gameObject.GetComponent<PlayerController>().IsInteracting = false;
        }

    }
}
