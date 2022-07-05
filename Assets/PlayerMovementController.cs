using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;

public class PlayerMovementController : NetworkBehaviour
{
    public float Speed = 10f;
    public float RotationSpeed = 360f;

    public GameObject PlayerModel;

    private Rigidbody rb;

    private void OnEnable()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void OnDisable()
    {
        Cursor.lockState = CursorLockMode.None;
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        SetStartPosition();
        PlayerModel.SetActive(true);
    }

    private void FixedUpdate()
    {
        if (hasAuthority)
        {
            Movement();
        }
    }

    public void SetStartPosition()
    {
        transform.position = new Vector3(Random.Range(-5, 5), 0.8f, Random.Range(-15, 15));
    }

    public void Movement()
    {
        float xDirection = Input.GetAxis("Horizontal");
        float zDirection = Input.GetAxis("Vertical");

        Vector3 moveDirection = new Vector3(-xDirection, 0, zDirection);

        if (moveDirection == Vector3.zero)
            return;

        var camToPlayer = rb.position - Camera.main.transform.position;
        var normalVector = Vector3.up;
        var projection = Vector3.ProjectOnPlane(camToPlayer, normalVector);
        projection.Normalize();

        var perpProjection = new Vector3(-projection.z, projection.y, projection.x);

        var final = moveDirection.x * perpProjection + moveDirection.z * projection;
        final.Normalize();

        var targetRotation = Quaternion.LookRotation(final);
        targetRotation = Quaternion.RotateTowards(transform.rotation, targetRotation, RotationSpeed * Time.fixedDeltaTime);
        
        rb.MovePosition(rb.position + final * Speed * Time.fixedDeltaTime);
        rb.MoveRotation(targetRotation);
    }
}
