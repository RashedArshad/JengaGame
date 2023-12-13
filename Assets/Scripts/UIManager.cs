using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    #region Public Members 
    public static UIManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<UIManager>();
            }

            return _instance;
        }
        private set { _instance = value; }
    }

    #endregion

    #region Private Members 
    [SerializeField] private TMPro.TextMeshProUGUI _blockInfoTextMesh; // The text that will represent the data on the selected stack
    [SerializeField] private Button _testMyStackBt;
    [SerializeField] private Button _restartBt;
    private static UIManager _instance;

    #endregion

    #region Unity CallBacks 
    private void Awake()
    {
        _testMyStackBt.onClick.RemoveAllListeners();
        _testMyStackBt.onClick.AddListener(TestMyStack);
        _restartBt.onClick.RemoveAllListeners();
        _restartBt.onClick.AddListener(Restart);
    }

    #endregion

    #region Private Methods
    private void TestMyStack()
    {
        GameManager.Instance.RemoveGlassBlocks();
    }
    private void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    private void DisableTxt()
    {
        _blockInfoTextMesh.transform.root.gameObject.SetActive(false);
    }

    #endregion

    #region public Methods
    public void ShowData(BlockData stackData)
    {
        CancelInvoke(nameof(DisableTxt));
        _blockInfoTextMesh.text = $"<b><color=red>{stackData.grade}: </b>" + $"<color=white>{stackData.domain}" + "<br>"
            + stackData.cluster + "<br>" + $"<b><color=red>{stackData.standardid}: </b>" + $"<color=white>{stackData.standarddescription}";
        _blockInfoTextMesh.transform.root.gameObject.SetActive(true);
        Invoke(nameof(DisableTxt),10);
    }

    #endregion
}
