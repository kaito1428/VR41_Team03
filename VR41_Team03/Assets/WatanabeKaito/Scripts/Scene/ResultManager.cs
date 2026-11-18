using UnityEngine;

public class ResultManager : MonoBehaviour
{
    public void SceneChangeTitle()
    {
       SceneController.Instance.ChangeScene(SceneController.SCENE_TYPE.TITLE, SceneTransition.TRANSITION_TYPE.FADE);
    }

    public void SceneChangeGame()
    {
        SceneController.Instance.ChangeScene(SceneController.SCENE_TYPE.GAME, SceneTransition.TRANSITION_TYPE.FADE);

        ScoreManager.Instance.StartGame();
    }
}
