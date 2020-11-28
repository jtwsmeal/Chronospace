﻿using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    // Player's rotation when starting at this checkpoint.
    private float playerRotation = 0f;
    private PlayerControl playerControl;
    private SoundManager soundManager;
    private bool activated = false;
    private bool rewindReset = false;

    Transform front;
    Transform back;
    Color original = new Color(255, 255, 255);
    Color neonGreen = new Color(7, 299, 3);

    private void Awake()
    {
        playerControl = FindObjectOfType<PlayerControl>();
        soundManager = FindObjectOfType<SoundManager>();

        front = transform.GetChild(0);
        back = transform.GetChild(1);

        if (IsStartCheckpoint())
        {
            ChangeColor(neonGreen);
        }
    }

    public Vector3 GetCheckPointPosition()
    {
        return transform.position;
    }

    public float GetPlayerRotation()
    {
        return playerRotation;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            transform.parent.GetComponent<CheckPointManager>().AddCheckPoint(this);
            ChangeColor(neonGreen);
            if (!rewindReset)
            {
                playerControl.ResetPositions();
                rewindReset = true;
            }
            // Play the checkpoint sound if this is the first time we hit it and it isn't the first checkpoint.
            if (!activated && !IsStartCheckpoint())
            {
                soundManager.PlayCheckpointSound();
            }
            activated = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            rewindReset = false;
        }
    }  

    private bool IsStartCheckpoint()
    {
        return name.CompareTo("1") == 0;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            /* Change the player's rotation while they're inside the checkpoint, so that if they
             * die before leaving the checkpoint they'll respawn at a reasonable rotation.
             */
            playerRotation = playerControl.GetCameraRotation().x;
        }
    }

    void ChangeColor(Color c)
    {

        front.GetComponent<Renderer>().material.SetColor("_EmissionColor", c * 0.005f);
        back.GetComponent<Renderer>().material.SetColor("_EmissionColor", c * 0.005f);
    }

    public void RevertColor()
    {
        ChangeColor(original);
    }
}
