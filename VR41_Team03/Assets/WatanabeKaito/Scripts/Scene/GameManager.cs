using UnityEngine;

public class GameManager : MonoBehaviour
{
    public void SceneChangeResult()
    {
        SceneController.Instance.ChangeScene(SceneController.SCENE_TYPE.RESULT, SceneTransition.TRANSITION_TYPE.FADE);
    }
}
