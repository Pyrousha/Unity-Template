using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandler : Singleton<InputHandler>
{
    private enum ButtonIndices
    {
        Interact = 0,
        Ability1 = 1,
        Ability2 = 2,
        Ability3 = 3,
        Jump = 4
    }

    public Vector2 MoveXZ
    {
        get;
        private set;
    }
    public Vector2 Look //Only used in games where the player can move the camera
    {
        get;
        private set;
    }
    public ButtonState Interact => buttons[(int)ButtonIndices.Interact];
    public ButtonState Ability1 => buttons[(int)ButtonIndices.Ability1];
    public ButtonState Ability2 => buttons[(int)ButtonIndices.Ability2];
    public ButtonState Ability3 => buttons[(int)ButtonIndices.Ability3];
    public ButtonState Jump => buttons[(int)ButtonIndices.Jump];

    private int buttonCount = -1; //Size of ButtonIndices enum
    [SerializeField] private short bufferFrames = 5;
    [SerializeField] private bool bufferEnabled = false;
    private short IDSRC = 0;
    private ButtonState[] buttons;
    private Queue<Dictionary<short, short>> inputBuffer = new Queue<Dictionary<short, short>>();
    private Dictionary<short, short> currentFrame;

    public void Start()
    {
        buttonCount = System.Enum.GetValues(typeof(ButtonIndices)).Length;

        buttons = new ButtonState[buttonCount];
        for (int i = 0; i < buttonCount; i++)
            buttons[i].Init(ref IDSRC, this);
    }

    private void FixedUpdate()
    {
        for (int i = 0; i < buttonCount; i++)
            buttons[i].Reset();

        if (bufferEnabled)
        {
            UpdateBuffer();
        }
    }

    //Input functions
    public void CTX_MoveXZ(InputAction.CallbackContext _ctx)
    {
        MoveXZ = _ctx.ReadValue<Vector2>();
    }
    public void CTX_Look(InputAction.CallbackContext _ctx)
    {
        Look = _ctx.ReadValue<Vector2>();
    }

    //Buttons
    public void CTX_Jump(InputAction.CallbackContext _ctx)
    {
        buttons[(int)ButtonIndices.Jump].Set(_ctx);
    }
    public void CTX_Ability1(InputAction.CallbackContext _ctx)
    {
        buttons[(int)ButtonIndices.Ability1].Set(_ctx);
    }
    public void CTX_Ability2(InputAction.CallbackContext _ctx)
    {
        buttons[(int)ButtonIndices.Ability2].Set(_ctx);
    }
    public void CTX_Ability3(InputAction.CallbackContext _ctx)
    {
        buttons[(int)ButtonIndices.Ability3].Set(_ctx);
    }
    public void CTX_Interact(InputAction.CallbackContext _ctx)
    {
        buttons[(int)ButtonIndices.Interact].Set(_ctx);
    }


    //Buffer functions
    public void FlushBuffer()
    {
        inputBuffer.Clear();
    }

    public void UpdateBuffer()
    {
        if (inputBuffer.Count >= bufferFrames)
            inputBuffer.Dequeue();
        currentFrame = new Dictionary<short, short>();
        inputBuffer.Enqueue(currentFrame);
    }

    public void PrintBuffer()
    {
        string bufferData = $"InputBuffer: count-{inputBuffer.Count}";
        foreach (var frame in inputBuffer)
            if (frame.Count > 0)
                bufferData += $"\n{frame.Count}";
        Debug.Log(bufferData);
    }

    public struct ButtonState
    {
        private short id;
        private static short STATE_PRESSED = 0,
                                STATE_RELEASED = 1;
        private InputHandler handler;
        private bool firstFrame;
        public bool Holding
        {
            get;
            private set;
        }
        public readonly bool Down
        {
            get
            {
                if (handler.bufferEnabled && handler.inputBuffer != null)
                {
                    foreach (var frame in handler.inputBuffer)
                    {
                        if (frame.ContainsKey(id) && frame[id] == STATE_PRESSED)
                        {
                            return frame.Remove(id);
                        }
                    }
                    return false;
                }
                return Holding && firstFrame;
            }
        }

        public readonly bool Up
        {
            get
            {
                if (handler.bufferEnabled && handler.inputBuffer != null)
                {
                    foreach (var frame in handler.inputBuffer)
                    {
                        if (frame.ContainsKey(id) && frame[id] == STATE_RELEASED)
                        {
                            return frame.Remove(id);
                        }
                    }
                    return false;
                }
                return !Holding && firstFrame;
            }
        }

        public void Set(InputAction.CallbackContext ctx)
        {
            Holding = !ctx.canceled;
            firstFrame = true;

            if (handler.bufferEnabled && handler.currentFrame != null)
            {
                handler.currentFrame.TryAdd(id, Holding ? STATE_PRESSED : STATE_RELEASED);
            }
        }

        public void Reset()
        {
            firstFrame = false;
        }

        public void Init(ref short IDSRC, InputHandler handler)
        {
            id = IDSRC++;
            this.handler = handler;
        }
    }
}