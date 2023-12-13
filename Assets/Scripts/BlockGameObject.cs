using UnityEngine;

public class BlockGameObject : MonoBehaviour
{
    #region Private Members 
    [SerializeField] private MeshRenderer _meshRenderer;
    [SerializeField] private GameObject _selected;
    [SerializeField] private TMPro.TextMeshProUGUI _textMesh;
    [SerializeField] private TMPro.TextMeshProUGUI _textMesh2;
    [SerializeField] private Rigidbody _rigidbody;

    [SerializeField] private BlockData _blockData;

    #endregion

    #region Unity CallBacks 
    private void OnMouseOver()
    {
        if(Input.GetMouseButtonDown(1)){

            if (GameManager.Instance.PreviousSelectedBlock)
                GameManager.Instance.PreviousSelectedBlock.SetActive(false);
            if (GameManager.Instance.PreviousSelectedBlock == _selected)
                return;
            _selected.SetActive(true);
            UIManager.Instance.ShowData(_blockData);
            GameManager.Instance.PreviousSelectedBlock = _selected;
        }
    }

    #endregion
    #region Private Methods
    private void SetMaterial(Material material)
    {
        _meshRenderer.material = material;
    }
    private void SetText(string txt)
    {
        _textMesh.text = txt;
        _textMesh2.text = txt;
    }

    #endregion
    #region public Methods
    public void SetData(BlockData blockData)
    {
        this._blockData = blockData;

        if (blockData.mastery == 0)
        {
            SetMaterial(GameManager.Instance.GameData.GlassMaterial);
            if (blockData.grade == "6th Grade")
                GameManager.Instance.GlassBlocks6Grade.Add(gameObject);
            if (blockData.grade == "7th Grade")
                GameManager.Instance.GlassBlocks7Grade.Add(gameObject);
            if (blockData.grade == "8th Grade")
                GameManager.Instance.GlassBlocks8Grade.Add(gameObject);
        }
        else if (blockData.mastery == 1)
        {
            SetMaterial(GameManager.Instance.GameData.WoodMaterial);
            SetText("LEARNED");
        }
        else
        {
            SetMaterial(GameManager.Instance.GameData.StoneMaterial);
            SetText("Mastered");
        }
    }

    public void IsKinematic(bool isKinematic)
    {
        _rigidbody.isKinematic = isKinematic;
    }

    #endregion

}
