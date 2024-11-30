using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Debug : MonoBehaviour
{
    public DebugControls controls;

    void Awake() {
        controls = new DebugControls();
        controls.Debug.Restart.performed += _ => Restart();
    }

    void OnEnable() {
        controls.Enable();
    }

    void OnDisable() {
        controls.Disable();
    }

    void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
