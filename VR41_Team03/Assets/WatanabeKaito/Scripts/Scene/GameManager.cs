using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    private void Update()
    {
        if (Keyboard.current.rKey.wasPressedThisFrame)
        {
            SceneChangeResult();
        }
    }

    public void SceneChangeResult()
    {
        SceneController.Instance.ChangeScene(SceneController.SCENE_TYPE.RESULT, SceneTransition.TRANSITION_TYPE.FADE);
    }
}
