using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BackToMainMenuButton : MonoBehaviour
{
   public void clicked()
    {
        SceneManager.LoadScene(0);
    }
}
