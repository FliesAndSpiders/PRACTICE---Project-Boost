using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using System;

public class Rocket : MonoBehaviour
{

    Rigidbody rocketBody;
    AudioSource rocketSound;
    [SerializeField] bool collisionsEnabled = true;
    [SerializeField] float rcsThrust = 150f;
    [SerializeField] float mainThrust = 150f;
    [SerializeField] float levelLoadDelay = 2f;
    [SerializeField] AudioClip mainEngine;
    [SerializeField] AudioClip explosionSound;
    [SerializeField] AudioClip victorySound;
    [SerializeField] ParticleSystem engineParticles;
    [SerializeField] ParticleSystem successParticles;
    [SerializeField] ParticleSystem defeatParticles;
    
    enum State { Alive, Dying, Transcending}
    State currentState = State.Alive;
    // Start is called before the first frame update
    void Start()
    {
        rocketBody = GetComponent<Rigidbody>();
        rocketSound = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (currentState == State.Alive)
        {
            Thrust();
            Rotate();
        }
        if (Debug.isDebugBuild)
        {
            RespondToDebugKeys();
        }
    }

    private void RespondToDebugKeys()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            LoadNextScene();
        }
        else if (Input.GetKeyDown(KeyCode.C))
        {
            collisionsEnabled = !collisionsEnabled;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (currentState != State.Alive || !collisionsEnabled)
        {
            return;
        }

        switch (collision.gameObject.tag)
        {
            case "Friendly":
                //do nothing
                break;
            case "Finish":
                //load next scene
                currentState = State.Transcending;
                successParticles.Play();
                rocketSound.Stop();
                rocketSound.PlayOneShot(victorySound);
                Invoke("LoadNextScene", levelLoadDelay);
                break;
            default:
                //die
                currentState = State.Dying;
                defeatParticles.Play();
                rocketSound.Stop();
                rocketSound.PlayOneShot(explosionSound);
                Invoke("LoadFirstScene", levelLoadDelay);
                break;
        }
    }

    private void LoadFirstScene()
    {
        currentState = State.Alive;
        SceneManager.LoadScene(0);
    }

    private void LoadNextScene()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        currentState = State.Alive;
        if (currentSceneIndex == SceneManager.sceneCountInBuildSettings - 1)
        {
            LoadFirstScene();
        }
        else
        {
            SceneManager.LoadScene(currentSceneIndex + 1);
        }
    }

    private void Rotate()
    {
        float rotationSpeed = rcsThrust * Time.deltaTime;
        rocketBody.freezeRotation = true; // this makes it so that it only rotates through player input and not by hitting objects
        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(Vector3.forward * rotationSpeed);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(-Vector3.forward * rotationSpeed);
        }
        rocketBody.freezeRotation = false; //hitting things now makes the rocket turn again
    }

    private void Thrust()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            if (!rocketSound.isPlaying)
            {
                rocketSound.PlayOneShot(mainEngine);
                engineParticles.Play();
            }
            rocketBody.AddRelativeForce(Vector3.up * mainThrust * Time.deltaTime);
        }
        else
        {
            rocketSound.Stop();
            engineParticles.Stop();
        }
    }
}
