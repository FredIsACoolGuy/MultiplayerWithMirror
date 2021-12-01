using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeSceneWhenClicked : MonoBehaviour
{
   public int sceneNum;
    //changes to the scene number stored in sceneNum
   public void clicked()
    {
        SceneManager.LoadScene(sceneNum);
    }
}
