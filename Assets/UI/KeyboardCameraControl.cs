//
//Filename: KeyboardCameraControl.cs
//
using UnityEngine;

[AddComponentMenu("Camera-Control/Keyboard")]
public class KeyboardCameraControl : MonoBehaviour
{
    // Keyboard axes buttons in the same order as Unity
    public enum KeyboardAxis { Horizontal = 0, Vertical = 1, None = 3 }

    [System.Serializable]
    // Handles left modifiers keys (Alt, Ctrl, Shift)
    public class Modifiers
    {
        public bool leftAlt;
        public bool leftControl;
        public bool leftShift;

        public bool checkModifiers()
        {
            return (!leftAlt ^ Input.GetKey(KeyCode.LeftAlt)) &&
                (!leftControl ^ Input.GetKey(KeyCode.LeftControl)) &&
                (!leftShift ^ Input.GetKey(KeyCode.LeftShift));
        }
    }

    [System.Serializable]
    // Handles common parameters for translations and rotations
    public class KeyboardControlConfiguration
    {

        public bool activate;
        public KeyboardAxis keyboardAxis;
        public Modifiers modifiers;
        public float sensitivity;

        public bool isActivated()
        {
            return activate && keyboardAxis != KeyboardAxis.None && modifiers.checkModifiers();
        }
    }

    // Yaw default configuration
    public KeyboardControlConfiguration yaw = new KeyboardControlConfiguration { keyboardAxis = KeyboardAxis.Horizontal, modifiers = new Modifiers { leftAlt = true }, sensitivity = 1F };

    // Pitch default configuration
    public KeyboardControlConfiguration pitch = new KeyboardControlConfiguration { keyboardAxis = KeyboardAxis.Vertical, modifiers = new Modifiers { leftAlt = true }, sensitivity = 1F };

    // Roll default configuration
    public KeyboardControlConfiguration roll = new KeyboardControlConfiguration { keyboardAxis = KeyboardAxis.Horizontal, modifiers = new Modifiers { leftAlt = true, leftControl = true }, sensitivity = 1F };

    // Vertical translation default configuration
    public KeyboardControlConfiguration verticalTranslation = new KeyboardControlConfiguration { keyboardAxis = KeyboardAxis.Vertical, sensitivity = 0.5F };

    // Horizontal translation default configuration
    public KeyboardControlConfiguration horizontalTranslation = new KeyboardControlConfiguration { keyboardAxis = KeyboardAxis.Horizontal, sensitivity = 0.5F };

    // Depth (forward/backward) translation default configuration
    public KeyboardControlConfiguration depthTranslation = new KeyboardControlConfiguration { keyboardAxis = KeyboardAxis.Vertical, modifiers = new Modifiers { leftControl = true }, sensitivity = 0.5F };

    // Default unity names for keyboard axes
    public string keyboardHorizontalAxisName = "Horizontal";
    public string keyboardVerticalAxisName = "Vertical";


    private string[] keyboardAxesNames;

    void Start()
    {
        keyboardAxesNames = new string[] { keyboardHorizontalAxisName, keyboardVerticalAxisName };
    }


    // LateUpdate  is called once per frame after all Update are done
    void LateUpdate()
    {
        if (yaw.isActivated())
        {
            var rotationX = Input.GetAxis(keyboardAxesNames[(int)yaw.keyboardAxis]) * yaw.sensitivity;
            transform.Rotate(0, rotationX, 0);
        }
        if (pitch.isActivated())
        {
            var rotationY = Input.GetAxis(keyboardAxesNames[(int)pitch.keyboardAxis]) * pitch.sensitivity;
            transform.Rotate(-rotationY, 0, 0);
        }
        if (roll.isActivated())
        {
            var rotationZ = Input.GetAxis(keyboardAxesNames[(int)roll.keyboardAxis]) * roll.sensitivity;
            transform.Rotate(0, 0, rotationZ);
        }
        if (verticalTranslation.isActivated())
        {
            var translateY = Input.GetAxis(keyboardAxesNames[(int)verticalTranslation.keyboardAxis]) * verticalTranslation.sensitivity;
            var tempRot = transform.rotation;
            transform.rotation = new Quaternion(0, 0, 0, 0);
            transform.Translate(0, translateY, 0);
            transform.rotation = tempRot;
        }
        if (horizontalTranslation.isActivated())
        {
            var translateX = Input.GetAxis(keyboardAxesNames[(int)horizontalTranslation.keyboardAxis]) * horizontalTranslation.sensitivity;

            var tempRot = transform.rotation;
            transform.rotation = new Quaternion(0, 0, 0, 0);
            transform.Translate(translateX, 0, 0);
            transform.rotation = tempRot;
        }
        if (depthTranslation.isActivated())
        {
            var translateZ = Input.GetAxis(keyboardAxesNames[(int)depthTranslation.keyboardAxis]) * depthTranslation.sensitivity;
            transform.Translate(0, 0, translateZ);
        }
    }
}