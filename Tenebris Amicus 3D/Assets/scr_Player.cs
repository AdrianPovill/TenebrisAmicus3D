using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;

public class scr_Player : MonoBehaviour
{
    private CharacterController characterController;

    [SerializeField] Camera mainCam;

    [SerializeField] float rotationSpeed;
    [SerializeField] float speed;
    [SerializeField] float gravity;

    [SerializeField] GameObject ammo;

    private bool isGrounded;
    private bool movement;

    private Vector3 initialPosition;
    private Vector3 moveDirection;
    private Vector3 verticalSpeed;

    private GameObject shotPoint;

    void Start()
    {
        shotPoint = transform.Find("ShotPoint").gameObject;
        movement = true;
        initialPosition = transform.position;
        characterController = GetComponent<CharacterController>();      
    }
    void Update()
    {
        if (movement)
        {
            float moveX = Input.GetAxisRaw("Horizontal");
            float moveY = Input.GetAxisRaw("Vertical");

            if (isGrounded)
            {
                verticalSpeed = new Vector3(0f, 0f, 0f);
            }
            else
            {
                verticalSpeed = new Vector3(0f, 0f, gravity * Time.deltaTime);
            }

            if (Input.GetMouseButtonDown(0))
            {
                Instantiate(ammo, shotPoint.transform.position, transform.rotation);
            }

            moveDirection = new Vector3(moveX, moveY, 0f).normalized;

            // Paso 1: Obtener la posición del mouse en pantalla
            Vector3 mousePosition = Input.mousePosition;

            // Paso 2: Lanzar un rayo desde la cámara hacia el cursor del mouse
            Ray ray = mainCam.ScreenPointToRay(mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                // Paso 3: Obtener la posición en el mundo del punto de impacto del rayo
                Vector3 targetPosition = hit.point;

                // Paso 4: Calcular la dirección horizontal desde la posición del jugador hacia el punto de impacto
                Vector3 direction = targetPosition - transform.position;
                direction.z = 0f; // Establecer la dirección en el eje Y como cero para evitar la rotación vertical

                // Paso 5: Calcular el ángulo en radianes en el eje Z
                float angle = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;

                // Paso 6: Crear una rotación en el eje Z
                Quaternion targetRotation = Quaternion.Euler(0f, 0f, -angle);

                // Paso 7: Interpolar la rotación suavemente
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
        }       
    }

    private void FixedUpdate()
    {
        if (movement)
        {
            characterController.Move((moveDirection + verticalSpeed) * speed * Time.deltaTime);
        }      
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Floor"))
        {
            isGrounded = true;
        }
        else if (other.CompareTag("Fall"))
        {
            StartCoroutine(Restart());
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Floor"))
        {
            isGrounded = false;
        }
    }

    IEnumerator Restart()
    {
        movement = false;

        yield return new WaitForSeconds(0.3f);

        transform.position = initialPosition;
        verticalSpeed = new Vector3(0f, 0f, 0f);

        yield return new WaitForSeconds(0.2f);

        movement = true;
    }
}
