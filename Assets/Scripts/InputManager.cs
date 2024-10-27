using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager instance;

    public bool menuOpenCloseInput { get;private set; }

    private PlayerInput playerInput;

    private InputAction menuOpenCloseAction;

    // Start is called before the first frame update
    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        playerInput = GetComponent<PlayerInput>();
        menuOpenCloseAction = playerInput.actions["MenuOpenClose"];
    }

    // Update is called once per frame
    void Update()
    {
        menuOpenCloseInput = menuOpenCloseAction.WasPressedThisFrame();
    }
}
