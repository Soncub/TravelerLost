using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BookAnimation : MonoBehaviour
{
    public Animator book;
    public float animationLength = 1.5f;

    [SerializeField] private CameraManager cameraManager;

    State animationState = State.Idle;

    public void OpenBook()
    {
        cameraManager.enabled = false;

        // Allows pause menu to check if the book is animating
        CloseAnimationState(false);

        // Enables the book and plays its animation
        book.gameObject.SetActive(true);
        book.SetBool("ShouldOpenBook", true);
    }

    public void CloseBook()
    {
        // Disables the book
        book.SetBool("ShouldOpenBook", false);
        book.gameObject.SetActive(false);

        cameraManager.enabled = true;
    }

    // Checks if the book is in the middle of its animation
    public bool IsAnimating()
    {
        if(animationState == State.IsAnimating)
        {
            return true;
        }

        return false;
    }

    // Changes the animation state to idle if true is passed; opens the animation state otherwise
    public void CloseAnimationState(bool shouldClose)
    {
        if(shouldClose)
        {
            animationState = State.Idle;
        }
        else
        {
            animationState = State.IsAnimating;
        }
    }

    enum State
    {
        Idle,
        IsAnimating
    }
}
