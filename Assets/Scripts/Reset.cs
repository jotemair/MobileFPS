using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reset : MonoBehaviour
{
    public void OnResetButtonClicked()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }
}
