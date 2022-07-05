using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;

public class PlayerMovementController : NetworkBehaviour
{
    public float Speed = 10f;
    public GameObject PlayerModel;

    private void Start()
    {
        PlayerModel.SetActive(false);
    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().name.StartsWith(Constants.ScenesName.Game))
        {
            if (!PlayerModel.activeSelf)
            {
                SetStartPosition();
                PlayerModel.SetActive(true);
            }
            
            if (hasAuthority)
            {
                Movement();
            }
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

        Vector3 moveDirection = new Vector3(xDirection, 0, zDirection);
        moveDirection.Normalize();
        //transform.position += moveDirection * Speed;

        if (moveDirection == Vector3.zero)
            return;

        var camToPlayer = transform.position - Camera.main.transform.position;
        var normalVector = Vector3.up;
        var projection = Vector3.ProjectOnPlane(camToPlayer, normalVector);
        projection.Normalize();

        var perpProjection = new Vector3(-projection.z, projection.y, projection.x);

        var final = moveDirection.x * perpProjection + moveDirection.z * projection;
        final.Normalize();

        //transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(final), 180 * Time.deltaTime);
        transform.position += final * Speed * Time.deltaTime;
    }
}
