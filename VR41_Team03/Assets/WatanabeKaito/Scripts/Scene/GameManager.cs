using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    [SerializeField] private TroublesomeCustomerSpawner m_TroubleCustomerSpawner;

    private void Start()
    {
        m_TroubleCustomerSpawner.BeginGame();
    }

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
