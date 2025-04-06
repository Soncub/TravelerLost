using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BookAnimation : MonoBehaviour
{
    public Animator book;
    public float animationLength = 1.5f;

    State animationState = State.Idle;

    public void OpenBook()
    {
        CloseAnimationState(false);
        book.SetBool("ShouldOpenBook", true);
    }

    public void CloseBook()
    {
        book.SetBool("ShouldOpenBook", false);
    }

    public bool IsAnimating()
    {
        if(animationState == State.IsAnimating)
        {
            return true;
        }

        return false;
    }

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
