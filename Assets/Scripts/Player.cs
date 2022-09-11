using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Transform cam;
    public Map map;
    public CubeList cubeList;
    public float walkSpeed = 10f;
    public float sprintSpeed = 20f;
    public float jumpForce = 10f;
    public float gravity = -9.8f;
    public float playerWidth = 0.3f;
    public float boundsTolerance = 0.1f;
    public bool isGrouned;
    public bool isSprinting;


    private float horizontal;
    private float vertical;
    private float mouseHorizontal;
    private float mouseVertical;
    private Vector3 velocity;
    private float verticalMomentum = 0;
    private bool jumpRequest;

    void Update()
    {
        GetPlayerInputs();

        velocity = ((transform.forward * vertical) + (transform.right * horizontal)) * Time.deltaTime * walkSpeed;
        velocity += Vector3.up * gravity * Time.deltaTime;

        velocity.y = checkDownSpeed(velocity.y);

        transform.Rotate(Vector3.up * mouseHorizontal);
        cam.Rotate(Vector3.right * -mouseVertical);
        transform.Translate(velocity, Space.World);
    }

    private void GetPlayerInputs()
    {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");
        mouseHorizontal = Input.GetAxis("Mouse X");
        mouseVertical = Input.GetAxis("Mouse Y");

        if (Input.GetButtonDown("Sprint"))
            isSprinting = true;
        if (Input.GetButtonUp("Sprint"))
            isSprinting = false;

        if (isGrouned && Input.GetButtonDown("Jump"))
            jumpRequest = true;

    }

    private float checkDownSpeed(float downSpeed)
    {
        if (map.checkForVoxel(new Vector3(transform.position.x - playerWidth, transform.position.y + downSpeed, transform.position.z - playerWidth))
         || map.checkForVoxel(new Vector3(transform.position.x + playerWidth, transform.position.y + downSpeed, transform.position.z - playerWidth))
         || map.checkForVoxel(new Vector3(transform.position.x - playerWidth, transform.position.y + downSpeed, transform.position.z + playerWidth))
         || map.checkForVoxel(new Vector3(transform.position.x + playerWidth, transform.position.y + downSpeed, transform.position.z + playerWidth)))
        {
            isGrouned = true;
            return 0f;
        }
        else
        {
            isGrouned = true;
            return downSpeed;
        }
    }
}
