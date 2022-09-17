using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Transform cam;
    public float walkSpeed = 10f;
    public float sprintSpeed = 20f;
    public float jumpForce = 10f;
    public float gravity = -9.8f;
    public float playerWidth = 0.3f;
    public float boundsTolerance = 0.1f;
    public bool isGrouned;
    public bool isSprinting;
    public Transform highlightBlock;
    public Transform placeBlock;
    public float checkIncrement = .1f;
    public float reach = 8f;


    private float horizontal;
    private float vertical;
    private float mouseHorizontal;
    private float mouseVertical;
    private Vector3 velocity;
    private float verticalMomentum = 0;
    private bool jumpRequest;


    void FixedUpdate()
    {
        handleVelocity();
        if (jumpRequest)
            Jump();

        transform.Rotate(Vector3.up * mouseHorizontal);
        cam.Rotate(Vector3.right * -mouseVertical);
        transform.Translate(velocity, Space.World);
    }

    void Update()
    {
        GetPlayerInputs();
        placeCursorBlock();
        DebugPanelGlobals.playerPos = new IntVector3(transform.position);
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void placeCursorBlock()
    {
        float step = checkIncrement;
        Vector3 lastPos = new Vector3();

        while (step < reach)
        {
            Vector3 pos = cam.position + (cam.forward * step);

            if (Map.getBlockAtPos(pos).isSolid)
            {
                highlightBlock.position = new Vector3(Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.y), Mathf.FloorToInt(pos.z));
                placeBlock.position = lastPos;

                highlightBlock.gameObject.SetActive(true);
                placeBlock.gameObject.SetActive(true);

                return;
            }
            lastPos = new Vector3(Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.y), Mathf.FloorToInt(pos.z));
            step += checkIncrement;
        }
        highlightBlock.gameObject.SetActive(false);
        placeBlock.gameObject.SetActive(false);
    }

    private void Jump()
    {
        verticalMomentum = jumpForce;
        isGrouned = false;
        jumpRequest = false;
    }

    private void handleVelocity()
    {
        if (verticalMomentum > gravity)
            verticalMomentum += Time.fixedDeltaTime * gravity;

        if (isSprinting)
            velocity = ((transform.forward * vertical) + (transform.right * horizontal)) * Time.fixedDeltaTime * sprintSpeed;
        else
            velocity = ((transform.forward * vertical) + (transform.right * horizontal)) * Time.fixedDeltaTime * walkSpeed;

        velocity += Vector3.up * verticalMomentum * Time.fixedDeltaTime;

        if ((velocity.z > 0 && front) || (velocity.z < 0 && back))
            velocity.z = 0;
        if ((velocity.x > 0 && right) || (velocity.x < 0 && left))
            velocity.x = 0;
        if (velocity.y < 0)
            velocity.y = checkDownSpeed(velocity.y);
        else if (velocity.y > 0)
            velocity.y = checkUpSpeed(velocity.y);
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

        if (highlightBlock.gameObject.activeSelf)
        {
            if (Input.GetMouseButtonDown(0))
            {
                IntVector3 chunkCoord = Globals.posToChunkCoord(highlightBlock.position);
                Map.map(chunkCoord.x, chunkCoord.y, chunkCoord.z).EditVoxel(highlightBlock.position, 0);
            }
            if (Input.GetMouseButtonDown(1))
            {
                IntVector3 chunkCoord = Globals.posToChunkCoord(placeBlock.position);
                Map.map(chunkCoord.x, chunkCoord.y, chunkCoord.z).EditVoxel(placeBlock.position, 4);
            }
        }
    }

    private float checkDownSpeed(float downSpeed)
    {
        if (Map.getBlockAtPos(new Vector3(transform.position.x - playerWidth, transform.position.y + downSpeed, transform.position.z - playerWidth)).isSolid
         || Map.getBlockAtPos(new Vector3(transform.position.x + playerWidth, transform.position.y + downSpeed, transform.position.z - playerWidth)).isSolid
         || Map.getBlockAtPos(new Vector3(transform.position.x - playerWidth, transform.position.y + downSpeed, transform.position.z + playerWidth)).isSolid
         || Map.getBlockAtPos(new Vector3(transform.position.x + playerWidth, transform.position.y + downSpeed, transform.position.z + playerWidth)).isSolid)
        {
            isGrouned = true;
            return 0f;
        }
        else
        {
            isGrouned = false;
            return downSpeed;
        }
    }

    private float checkUpSpeed(float upSpeed)
    {
        if (Map.getBlockAtPos(new Vector3(transform.position.x - playerWidth, transform.position.y + Globals.playerHeight + upSpeed, transform.position.z - playerWidth)).isSolid
         || Map.getBlockAtPos(new Vector3(transform.position.x + playerWidth, transform.position.y + Globals.playerHeight + upSpeed, transform.position.z - playerWidth)).isSolid
         || Map.getBlockAtPos(new Vector3(transform.position.x - playerWidth, transform.position.y + Globals.playerHeight + upSpeed, transform.position.z + playerWidth)).isSolid
         || Map.getBlockAtPos(new Vector3(transform.position.x + playerWidth, transform.position.y + Globals.playerHeight + upSpeed, transform.position.z + playerWidth)).isSolid)
            return 0f;
        else
            return upSpeed;
    }

    public bool front
    {
        get
        {
            if (Map.getBlockAtPos(new Vector3(transform.position.x, transform.position.y, transform.position.z + playerWidth)).isSolid
                    || Map.getBlockAtPos(new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z + playerWidth)).isSolid)
                return true;
            return false;
        }
    }

    public bool back
    {
        get
        {
            if (Map.getBlockAtPos(new Vector3(transform.position.x, transform.position.y, transform.position.z - playerWidth)).isSolid
                    || Map.getBlockAtPos(new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z - playerWidth)).isSolid)
                return true;
            return false;
        }
    }

    public bool left
    {
        get
        {
            if (Map.getBlockAtPos(new Vector3(transform.position.x - playerWidth, transform.position.y, transform.position.z)).isSolid
                    || Map.getBlockAtPos(new Vector3(transform.position.x - playerWidth, transform.position.y + 1f, transform.position.z)).isSolid)
                return true;
            return false;
        }
    }

    public bool right
    {
        get
        {
            if (Map.getBlockAtPos(new Vector3(transform.position.x + playerWidth, transform.position.y, transform.position.z)).isSolid
                    || Map.getBlockAtPos(new Vector3(transform.position.x + playerWidth, transform.position.y + 1f, transform.position.z)).isSolid)
                return true;
            return false;
        }
    }
}
