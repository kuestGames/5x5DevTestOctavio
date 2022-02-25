using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace DevTest.Gameplay
{
    public class Sc_PlayerController : MonoBehaviour
    {
        [SerializeField] private PlayerInput playerInput;

        private Vector3 inputVector;
        public int movementSpeed = 1;
        private Vector3 playerOrigin = new Vector3(0, 0, -12);
        private Vector3 targetPosition;
        private Vector3 currentPosition;
        [HideInInspector]public float fireDelay = 0.1f;
        private float delayTimer = 0;
        private bool canFire = true;
        public bool isPlaying = false;

        // Start is called before the first frame update
        void Start()
        {
            transform.position = playerOrigin;
        }


        private void OnMove(InputValue value)
        {
            Vector2 inputMovement = value.Get<Vector2>();
            inputVector = new Vector3(inputMovement.x, 0, 0);
        }

        private void OnFire(InputValue value)
        {
            if (canFire)
            {
                Sc_GameplayManager.Instance.FireBullet(transform.position);
                canFire = false;
            }
        }
        // Update is called once per frame
        void Update()
        {
            if (isPlaying)
            {
                if (inputVector != Vector3.zero)
                {

                    transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x + inputVector.x, playerOrigin.y, playerOrigin.z), Time.deltaTime * movementSpeed);
                }
                if (!canFire)
                {
                    delayTimer += Time.deltaTime;
                    if (delayTimer >= fireDelay)
                    {
                        delayTimer = 0;
                        canFire = true;
                    }
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.name == "PowerUp")
            {
                Sc_GameplayManager.Instance.PowerUp();
            }
        }
    }
}