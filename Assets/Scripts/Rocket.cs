using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class Rocket : MonoBehaviour
{

    Rigidbody rocketBody;
    AudioSource rocketSound;
    [SerializeField] float rcsThrust = 150f;
    [SerializeField] float mainThrust = 150f;
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
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (currentState != State.Alive)
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
                Invoke("LoadNextScene", 1f);
                break;
            default:
                //die
                currentState = State.Dying;
                defeatParticles.Play();
                rocketSound.Stop();
                rocketSound.PlayOneShot(explosionSound);
                Invoke("LoadFirstScene", 1.0f);
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
        currentState = State.Alive;
        SceneManager.LoadScene(1);
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
