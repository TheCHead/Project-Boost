using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour
{
    [SerializeField] float rcsThrust = 100f;
    [SerializeField] float mainThrust = 100f;
    [SerializeField] AudioClip mainEngine;
    [SerializeField] AudioClip winSFX;
    [SerializeField] AudioClip deathSFX;
    Rigidbody myRigidBody;
    AudioSource myAudioSource;
    enum State { Alive, Dying, Transcending };
    State state = State.Alive;
    // Start is called before the first frame update
    void Start()
    {
        myRigidBody = GetComponent<Rigidbody>();
        myAudioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (state == State.Alive)
        {
            Thrust();
            Rotate();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (state != State.Alive) { return; }

        switch (collision.gameObject.tag)
        {
            case "Friendly":
                break;
            case "Finish":
                StartWinSequence();
                break;
            default:
                StartCoroutine(ProcessDeath());
                break;
        }
    }

    private void StartWinSequence()
    {
        state = State.Transcending;
        myAudioSource.Stop();
        myAudioSource.PlayOneShot(winSFX);
        Invoke("LoadNextScene", 1f);
    }

    IEnumerator ProcessDeath()
    {
        state = State.Dying;
        myAudioSource.Stop();
        myAudioSource.PlayOneShot(deathSFX);
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void LoadNextScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    private void Thrust()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            myRigidBody.AddRelativeForce(Vector3.up * mainThrust);
            if (!myAudioSource.isPlaying)
            {
                myAudioSource.PlayOneShot(mainEngine);
            }
        }
        else
        {
            myAudioSource.Stop();
        }
    }

    private void Rotate()
    {
        myRigidBody.freezeRotation = true;
        float rotationThisFrame = rcsThrust * Time.deltaTime;
        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(Vector3.forward * rotationThisFrame);
        }

        if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(-Vector3.forward * rotationThisFrame);
        }

        myRigidBody.freezeRotation = false;
    }
}
