using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Linq;

public class GameManager : MonoBehaviour
{
    #region Public Members 
    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<GameManager>();
            }

            return _instance;
        }
        private set { _instance = value; }
    }

    [HideInInspector] public GameObject PreviousSelectedBlock;
    [HideInInspector] public int CurrentStackNum;

    [HideInInspector] public List<GameObject> GlassBlocks6Grade = new List<GameObject>();
    [HideInInspector] public List<GameObject> GlassBlocks7Grade = new List<GameObject>();
    [HideInInspector] public List<GameObject> GlassBlocks8Grade = new List<GameObject>();
    public GameData GameData;
    #endregion

    #region Private Members 
    private static GameManager _instance;


    [Header("Stack's Blocks SpawnPoint ")]
    [SerializeField] private Transform[] _initialBlocksSpawnPointsGrade6;
    [SerializeField] private Transform[] _initialBlocksSpawnPointsGrade7;
    [SerializeField] private Transform[] _initialBlocksSpawnPointsGrade8;

    [Space(5)]
    [Header("Stack's Blocks ")]
    [SerializeField] private List<BlockGameObject> _blocksGameObjectsForGrade6 = new List<BlockGameObject>();
    [SerializeField] private List<BlockGameObject> _blocksGameObjectsForGrade7 = new List<BlockGameObject>();
    [SerializeField] private List<BlockGameObject> _blocksGameObjectsForGrade8 = new List<BlockGameObject>();

    [Space(5)]
    [Header("Stack's Parents ")]
    [SerializeField] private Transform _grade6StackParent;
    [SerializeField] private Transform _grade7StackParent;
    [SerializeField] private Transform _grade8StackParent;

    private readonly List<BlockData> _6GradeStack = new List<BlockData>();
    private readonly List<BlockData> _7GradeStack = new List<BlockData>();
    private readonly List<BlockData> _8GradeStack = new List<BlockData>();


    private const string _uRL = "https://ga1vqcu3o1.execute-api.us-east-1.amazonaws.com/Assessment/stack"; // the link that the data will be fetched from
    private string _json;
    private Vector3 _blockPos;
    private List<BlockData> _allBlocks;
    private Transform[] _stackInitialSpawnPoints;
    private Transform _stackParent;
    #endregion

    #region Unity CallBacks 
    private void Awake()
    {
        CurrentStackNum = 7;
        GenerateRequest();
    }

    #endregion

    #region Private Methods

    private void GenerateRequest()
    {
        StartCoroutine(ProcessRequest(_uRL));
    }
    private IEnumerator ProcessRequest(string uri)
    {
        using (UnityWebRequest request = UnityWebRequest.Get(uri))
        {
            yield return request.SendWebRequest();

            _json = request.downloadHandler.text;
            _allBlocks = JsonConvert.DeserializeObject<List<BlockData>>(_json);
        }
        SortData();
        BuildAllStacks();
    }
    private void SortData()
    {
        foreach (var block in _allBlocks)
        {
            if (block.grade == "6th Grade")
                _6GradeStack.Add(block);
            if (block.grade == "7th Grade")
                _7GradeStack.Add(block);
            if (block.grade == "8th Grade")
                _8GradeStack.Add(block);
        }
    }
    private void BuildAllStacks()
    {
        BuildAndSetStack(_6GradeStack, _blocksGameObjectsForGrade6, 6);
        BuildAndSetStack(_7GradeStack, _blocksGameObjectsForGrade7, 7);
        BuildAndSetStack(_8GradeStack, _blocksGameObjectsForGrade8, 8);
        //allBlocksHolder.SetActive(true);
    }
    private void BuildAndSetStack(List<BlockData> gradeStack, List<BlockGameObject> blocksGameObjects, int grade)
    {
        var sortedList = gradeStack.OrderBy(go => go.domain).ThenBy(go => go.cluster).ThenBy(go => go.standardid).ToArray();

        BuildStackManually(gradeStack.Count, blocksGameObjects, grade);
        int i = 0;
        foreach (var blockData in sortedList)
        {
            BlockGameObject blockGameObject = blocksGameObjects[i];
            blockGameObject.SetData(blockData);
            i++;
        }
    }
    private void BuildStackManually(int gradeCount  , List<BlockGameObject> blocksGameObjects, int grade)
    {
        int i = 0, j = 0;

        if (grade == 6)
        {
            _stackInitialSpawnPoints = _initialBlocksSpawnPointsGrade6;
            _stackParent = _grade6StackParent;
        }
        else if (grade == 7)
        {
            _stackInitialSpawnPoints = _initialBlocksSpawnPointsGrade7;
            _stackParent = _grade7StackParent;
        }
        else
        {
            _stackInitialSpawnPoints = _initialBlocksSpawnPointsGrade8;
            _stackParent = _grade8StackParent;
        }

        while (i < gradeCount)
        {
            foreach (var spawnPoint in _stackInitialSpawnPoints)
            {
                _blockPos = new Vector3(spawnPoint.position.x, spawnPoint.position.y + j * 2, spawnPoint.position.z);
                blocksGameObjects.Add(Instantiate(GameData.BlockPrefab, _blockPos, spawnPoint.rotation, _stackParent));
                i++;
                if (i >= gradeCount)
                    break;
            }
            j++;
        }
    }
    #endregion

    #region public Methods
    public void RemoveGlassBlocks()
    {
        List<BlockGameObject> focusedStackblocksGameObjects = new();
        List<GameObject> glassBlocks = new();
        if (CurrentStackNum == 6)
        {
            focusedStackblocksGameObjects = _blocksGameObjectsForGrade6;
            glassBlocks = GlassBlocks6Grade;
        }
        else if (CurrentStackNum == 7)
        {
            focusedStackblocksGameObjects = _blocksGameObjectsForGrade7;
            glassBlocks = GlassBlocks7Grade;
        }
        else if (CurrentStackNum == 8)
        {
            focusedStackblocksGameObjects = _blocksGameObjectsForGrade8;
            glassBlocks = GlassBlocks8Grade;
        }
        else
            Debug.Log("Current stack number is wrong");
        foreach (var block in focusedStackblocksGameObjects)
        {
            block.IsKinematic(false);
        }
        foreach (var item in glassBlocks)
        {
            item.SetActive(false);
        }
    }

    #endregion
}
