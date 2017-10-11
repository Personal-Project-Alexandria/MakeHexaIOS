using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;

[System.Serializable]
public enum CellType
{
    NONE,
    UP,
    DOWN = -1
}
[System.Serializable]
public enum ColorType
{
    NONE = -1,
    COLOR_1,
    COLOR_2,
    COLOR_3,
    COLOR_4,
    COLOR_5,
    COLOR_6,
    SPECIAL_COLOR
}
[System.Serializable]
public struct PosMap
{
    public int Row;

    public int Col;

    public PosMap(int nRow, int nCol)
    {
        this.Row = nRow;
        this.Col = nCol;
    }
}

public class Hexagon : MonoSingleton<Hexagon>
{

    public int _Row;

    public int _Col;

    public List<Color> _listColorUI;

    private CellModel[,] _mapData;//map data ban dau, la mang 2 chieu

    private TriangelItem[,] _mapUI;//các item triangle trên màn hình

    private List<CellModel> _listMapping = new List<CellModel>();//list map

    private List<CellModel> _listHint = new List<CellModel>();//list kiem tra dung chua

    private List<ColorType> _listColorUse = new List<ColorType>(6);

    private List<ColorType> _listSumColor = new List<ColorType>(6);

    public Transform panelPattern;

    public List<Transform> listPosPattern;

    public Text _getScoreEffect;

    public Text panelOutOfMove;

    private int _nScore;
    private void Awake()
    {

        this.InitMapData();
        this.InitMapUI();
        this.FillMapData();
        this.DrawMap();
        this.InitLogicListColor();

        this.CheckTransferColor();

        this.callbackScore = new List<CallbackAddScore>();
    }

    private void Start()
    {
        AlgorithmCheckHexa.Instance.UpdateRowCol(this._Row, this._Col);

    }

    #region Init
    //khoi tao map ban dau
    private void InitMapData()
    {
        this._mapData = new CellModel[this._Row, this._Col];
        for (int i = 0; i < this._Row; i++)
        {
            for (int j = 0; j < this._Col; j++)
            {
                //chưa cập nhật map
                this._mapData[i, j] = new CellModel(CellType.NONE, ColorType.NONE, new PosMap(i, j));
            }
        }
    }
    private void InitMapUI()
    {
        this._mapUI = new TriangelItem[this._Row, this._Col];
        for (int i = 0; i < this._Row; i++)
        {
            for (int j = 0; j < this._Col; j++)
            {
                //khởi tạo thì chưa có gì
                this._mapUI[i, j] = null;
            }
        }
    }
    //đổ data vào map
    //cập nhật vị trí Map[i,j] là top hay down
    private void FillMapData()
    {
        int num = this._Row / 2 - 1;
        for (int i = this._Row / 2 - 1; i >= 0; i--)
        {
            int num2 = num - i;
            int num3 = this._Col - 1 - num2;
            CellType cellType = CellType.DOWN;
            for (int j = num2; j <= num3; j++)
            {
                this._mapData[i, j].Type = cellType;
                //cellType *= CellType.DOWN;
                cellType = (CellType)((int)cellType * (int)CellType.DOWN);
            }
        }
        int num4 = 0;
        for (int k = this._Row / 2; k < this._Row; k++)
        {
            int num5 = k - 1 - num4 * 2;
            num4++;
            for (int l = 0; l < this._Col; l++)
            {
                this._mapData[k, l].Type = (CellType)((int)(this._mapData[num5, l].Type) * (int)CellType.DOWN);
            }
        }
    }
    private void InitLogicListColor()
    {
        this._listSumColor.Clear();
        this._listColorUse.Clear();
        for (int i = 0; i < 6; i++)
        {
            ColorType item = (ColorType)i;
            this._listSumColor.Add(item);
        }
    }
    //vẽ ra màn hình
    public void DrawMap()
    {
        int num = 0;
        for (int i = 0; i < this._Row; i++)
        {
            for (int j = 0; j < this._Col; j++)
            {
                CellType type = this._mapData[i, j].Type;
                if (type != CellType.NONE)
                {
                    GameObject gameObject = Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/TriangeItem"));
                    gameObject.transform.SetParent(this.transform);// this._layerTriangle);
                    gameObject.transform.localScale = Vector3.one;
                    gameObject.name = num.ToString();
                    this.SetPositionTriangle(gameObject.transform, i, j, type);
                    TriangelItem item = gameObject.GetComponent<TriangelItem>();
                    if (item != null)
                    {
                        item.SetData(i, j, type);
                        this._mapUI[i, j] = item;
                    }
                    num++;
                }
            }
        }
    }
    private void SetPositionTriangle(Transform triangel, int nRow, int nCol, CellType _cellType)
    {
        float num = (float)((_cellType != CellType.UP) ? 0 : (-(float)Constant.DELTA));
        int num2 = (nCol != 0) ? (Constant.RANGE_X * nCol) : 0;
        int num3 = (nRow != 0) ? (Constant.RANGE_Y * nRow) : 0;
        float startX = (-(this._Col - 2) / 2 * Constant.CELL_SIZE_WIDTH);
        float startY = (-this._Row / 2 * Constant.CELL_SIZE_HEIGHT);
        triangel.localPosition = new Vector2((float)(startX + Constant.CELL_SIZE_WIDTH / 2 + num2 + Constant.CELL_SIZE_WIDTH / 2 * nCol), (float)(startY + Constant.CELL_SIZE_HEIGHT / 2 + num3 + Constant.CELL_SIZE_HEIGHT * nRow) + num);
    }
    private void TransferColor(int nCount)
    {
        if (this._listSumColor.Count == 0)
        {
            return;
        }
        while (nCount != 0)
        {
            int index = UnityEngine.Random.Range(0, this._listSumColor.Count);
            this._listColorUse.Add(this._listSumColor[index]);
            this._listSumColor.RemoveAt(index);
            nCount--;
        }
    }

    private void CheckTransferColor()
    {
        if (this._nScore >= 0 && this._nScore < 850)
        {
            if (this._listColorUse.Count == 0)
            {
                this.TransferColor(4);
            }
        }
        else if (this._nScore >= 850 && this._nScore < 2000)
        {
            if (this._listColorUse.Count == 4)
            {
                this.TransferColor(1);
            }
        }
        else
        {
            this.TransferColor(1);
        }
    }
    #endregion

    #region Create Pattern
    private GameObject InitDataPattern(int nIndex, bool bIsSpecial)
    {
        ColorType colorType = (!bIsSpecial) ? this.GetRandomColor() : ColorType.SPECIAL_COLOR;
        HexaPattern.Type eType = (HexaPattern.Type)((!bIsSpecial) ? UnityEngine.Random.Range(0, 6) : 6);
        Color colorUI = (!bIsSpecial) ? this._listColorUI[(int)colorType] : Color.white;
        GameObject gameObject = this.CreatePattern(eType, nIndex, colorType, colorUI, bIsSpecial);
        gameObject.transform.localScale = Vector3.zero;
        gameObject.transform.DOScale(Vector3.one, 0.25f);
        gameObject.transform.position = listPosPattern[nIndex].position;
        gameObject.name = "Pattern " + nIndex;// + this._nCountPattern;
		gameObject.SetActive(true);
        //this._nCountPattern++;
        return gameObject;
    }
    [ContextMenu("create one")]
    void testCreateOnePattern()
    {
        this.CreatePattern(HexaPattern.Type.ONE_TRIANGLE, 1, ColorType.COLOR_1, this._listColorUI[(int)ColorType.COLOR_1], false);
    }
    [ContextMenu("create list")]
    private void CreateThreePattern()
    {
        for (int i = 0; i < 3; i++)
        {
            GameObject gameObject = this.InitDataPattern(i, false);
			//this._listPattern[i] = gameObject.GetComponent<HexaPattern>();
			//this.listPosPattern.Add(gameObject.transform);
		}
    }

    public List<HexaPattern> listPattern;
    private GameObject CreatePattern(HexaPattern.Type eType, int nIndex, ColorType eColor, Color colorUI, bool bIsSpecial)
    {
        GameObject gameObject = Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/HexaPattern"));
        gameObject.transform.SetParent(this.panelPattern); // this._panelPattern);
        gameObject.transform.localScale = Vector3.zero;
        gameObject.transform.position = listPosPattern[nIndex].position;

        if (bIsSpecial)
        {
            //gameObject.transform.position = this._specialPatternManager.transform.position;
        }
        else
        {
            gameObject.transform.localPosition = new Vector2(0f, 0f);
        }

        HexaPattern pattern = gameObject.GetComponent<HexaPattern>();
        if (pattern != null)
        {
			pattern.Init();
            pattern.CreatePattern(nIndex, eType, eColor, colorUI);
            this.listPattern.Add(pattern);
        }
        return gameObject;
    }

    private ColorType GetRandomColor()
    {
        int index = UnityEngine.Random.Range(0, this._listColorUse.Count);
        return this._listColorUse[index];
    }
    #endregion

    #region Move Pattern 

    public void OnPatternBeginDrag(PointerEventData eventData, HexaPattern pattern)
    {

    }
    public void OnPatternDrag(PointerEventData eventData, HexaPattern pattern)
    {

        Vector3 pos = Camera.main.ScreenToWorldPoint(new Vector3(eventData.position.x, eventData.position.y));

        Vector2 position2 = pattern.transform.parent.InverseTransformPoint(pos);
        pattern.UpdatePosDrag(position2);

        List<RaycastResult> results = new List<RaycastResult>();
        Vector2 posCenter = pattern.GetPositionTriangleCenter();
        Vector2 posInverse = Camera.main.WorldToScreenPoint(posCenter);

        eventData.position = posInverse;
        EventSystem.current.RaycastAll(eventData, results);

        if (results.Count != 0)
        {
            TriangelItem triangel = getTriangelItemRayCat(results);
            if (triangel != null)
            {

                int row = triangel.Row;
                int col = triangel.Col;

                this._listMapping = pattern.GetListMapping(row, col);

                bool isAllow = this.CheckAllowAddPattern(this._listMapping, pattern.Color);
                if (isAllow)
                {
                    this.ClearHint();
                    this.GetListHint(this._listMapping);
                    this.ShowHint(pattern.Color);
                }
                else
                {
                    this.ClearHint();
                }
            }
            else
            {
                this.ClearHint();
            }
        }
        else
        {
            this.ClearHint();
        }
    }
    public void OnPatternEndDrag(PointerEventData eventData, HexaPattern pattern)
    {

        List<RaycastResult> results = new List<RaycastResult>();
        Vector2 posCenter = pattern.GetPositionTriangleCenter();
        Vector2 posInverse = Camera.main.WorldToScreenPoint(posCenter);
        eventData.position = posInverse;
        EventSystem.current.RaycastAll(eventData, results);
        
        this.ClearHint();
        if (results.Count != 0)
        {

            TriangelItem triangel = getTriangelItemRayCat(results);
            if (triangel != null)
            {
                int row = triangel.Row;
                int col = triangel.Col;

                this._listMapping = pattern.GetListMapping(row, col);
                //DebugLisMapping(this._listMapping);
                bool isAllow = this.CheckAllowAddPattern(this._listMapping, pattern.Color);
                if (isAllow)
                {
                    SoundManager.Instance.PlaySfx(SFX.Take);
                    this.GetListHint(this._listMapping);
                    this.ShowPatternTriangle(pattern.Color, this._listHint);
                    List<PosMap> lists = AlgorithmCheckHexa.Instance.CheckHexa(this._listHint, this._mapData, pattern.TypePattern);
                    bool getDouble = false;
                    int num = this.CaculateScore(pattern.NumberTriangle, lists.Count, pattern.Color, ref getDouble);
                    if (lists.Count > 0)
                    {
                        Debug.Log("Pos map " + lists.Count);
                        StartCoroutine(ClearPatternTriangle(lists));
                        //CreateExplosionEffect(pattern.transform.position);
                    }
                    this.PlayEffectGetScore(num, pattern.transform.position, pattern.Color);
                    this.CheckTransferColor();
                    this.btHammer.interactable = true;
                    //tao mơi thằng Pattern mới
                    GameObject gameObject = this.InitDataPattern(pattern.IndexInPanel, false);
                    gameObject.transform.position = this.listPosPattern[pattern.IndexInPanel].position;
                    pattern.DestroyPattern();
                    this.listPattern.Remove(pattern);

                    StartCoroutine(CoutingCheckOutOfMove());
                    //this.AddNewPattern(pattern.IndexInPanel, false);
                    //tinh diem o day
                }
                else
                {
                    this.ResetPosPattern(pattern);
                }

            }
            else
            {
                this.ResetPosPattern(pattern);
            }
        }
        else
        {
            this.ResetPosPattern(pattern);
        }
    }
    private void CreateExplosionEffect(Vector2 position)
    {
        GameObject gameObject = Instantiate<GameObject>(Resources.Load<GameObject>("Effect/Star"));
       
        gameObject.transform.localScale = new Vector3(0.2f, 0.2f, 0);
        gameObject.transform.position = position;
        Destroy(gameObject, 2f);
    }
    IEnumerator ClearPatternTriangle(List<PosMap> lists)
    {
        yield return new WaitForEndOfFrame();
        SoundManager.Instance.PlaySfx(SFX.Clear);
        for (int i = 0; i < lists.Count; i++)
        {
            PosMap map = lists[i];
            this._mapData[map.Row, map.Col].Color = ColorType.NONE;
            if (this._mapData[map.Row, map.Col].Type != CellType.NONE)
            {
                this._mapUI[map.Row, map.Col].ClearPatternTriangle();
                yield return new WaitForSeconds(0.1f);
                this.CreateExplosionEffect(this._mapUI[map.Row, map.Col].transform.position);
            }
        }
    }
    private void DebugLisMapping(List<CellModel> listMapping)
    {
        int count = listMapping.Count;
        string text = string.Empty;
        string text2 = string.Empty;
        for (int i = 0; i < count / 2; i++)
        {
            text = text + listMapping[i].Type + " - ";
        }
        text += "\n";
        for (int j = count - 1; j >= count / 2; j--)
        {
            text = text + listMapping[j].Type + " - ";
        }
        for (int k = 0; k < count / 2; k++)
        {
            PosMap posInMap = listMapping[k].PosInMap;
            string text3 = text2;
            text2 = string.Concat(new object[]
            {
                text3,
                "( ",
                posInMap.Row,
                " , ",
                posInMap.Col,
                " )  - "
            });
        }
        text2 += "\n";
        for (int l = count - 1; l >= count / 2; l--)
        {
            PosMap posInMap2 = listMapping[l].PosInMap;
            string text3 = text2;
            text2 = string.Concat(new object[]
            {
                text3,
                "( ",
                posInMap2.Row,
                " , ",
                posInMap2.Col,
                " )  - "
            });
        }
        Debug.Log(text);
        Debug.Log(text2);
    }
    private void ShowPatternTriangle(ColorType eColorType, List<CellModel> listHint)
    {
        int count = listHint.Count;
        if (count == 0)
        {
            return;
        }
        for (int i = 0; i < count; i++)
        {
            PosMap posInMap = listHint[i].PosInMap;
            int row = posInMap.Row;
            int col = posInMap.Col;
            if (eColorType != ColorType.SPECIAL_COLOR)
            {
                this._mapUI[row, col].DisplayPatternTriangle(this._listColorUI[(int)eColorType]);
            }
            else
            {
                this._mapUI[row, col].ClearPatternTriangle();
            }
            this._mapData[row, col].Color = ((eColorType != ColorType.SPECIAL_COLOR) ? eColorType : ColorType.NONE);
        }
    }
    TriangelItem getTriangelItemRayCat(List<RaycastResult> results)
    {
        foreach (RaycastResult result in results)
        {
            TriangelItem item = result.gameObject.GetComponent<TriangelItem>();
            if (item != null)
            {
                return item;
            }
        }
        return null;
    }

    private bool CheckAllowAddPattern(List<CellModel> listMapping, ColorType eColorType)
    {
        int count = listMapping.Count;
        for (int i = 0; i < count; i++)
        {
            PosMap posInMap = listMapping[i].PosInMap;
            CellType type = listMapping[i].Type;
            int row = posInMap.Row;
            int col = posInMap.Col;
            if (type != CellType.NONE)
            {
                if (row < 0 || col < 0 || row >= this._Row || col >= this._Col)
                {
                    return false;
                }
                if (eColorType != ColorType.SPECIAL_COLOR)
                {
                    if (this._mapData[row, col].Color != ColorType.NONE)
                    {
                        return false;
                    }
                    if (type != this._mapData[row, col].Type)
                    {
                        return false;
                    }
                }
            }
        }
        return true;
    }
    private void ClearHint()
    {
        int count = this._listHint.Count;
        for (int i = 0; i < count; i++)
        {
            PosMap posInMap = this._listHint[i].PosInMap;
            int row = posInMap.Row;
            int col = posInMap.Col;
            this._mapUI[row, col].ClearHint();
        }
        this._listHint.Clear();
    }
    private void GetListHint(List<CellModel> listMapping)
    {
        this._listHint.Clear();
        int count = listMapping.Count;
        if (count == 0)
        {
            return;
        }
        for (int i = 0; i < count; i++)
        {
            PosMap posInMap = listMapping[i].PosInMap;
            int row = posInMap.Row;
            int col = posInMap.Col;
            CellType type = listMapping[i].Type;
            if (row >= 0 && col >= 0)
            {
                if (type != CellType.NONE)
                {
                    if (type == this._mapData[row, col].Type)
                    {
                        this._listHint.Add(listMapping[i]);
                    }
                }
            }
        }
    }

    private void ShowHint(ColorType eColorType)
    {
        int count = this._listHint.Count;
        if (count == 0)
        {
            return;
        }
        Color colorUI = (eColorType != ColorType.SPECIAL_COLOR) ? this._listColorUI[(int)eColorType] : Color.white;
        for (int i = 0; i < count; i++)
        {
            PosMap posInMap = this._listHint[i].PosInMap;
            int row = posInMap.Row;
            int col = posInMap.Col;
            this._mapUI[row, col].DisplayHint(colorUI);
        }
    }

    public void ResetPosPattern(HexaPattern pattern)
    {
        SoundManager.Instance.PlaySfx(SFX.Drop);
        int indexInPanel = pattern.IndexInPanel;
        pattern.transform.DOMove(this.listPosPattern[indexInPanel].position, 0.25f, false);
    }

    IEnumerator CoutingCheckOutOfMove()
    {
        List<HexaPattern> temp = new List<HexaPattern>(this.listPattern);
        yield return new WaitForSeconds(1);
        bool isCheckOutOfMove = false;
        foreach (HexaPattern pattern in temp)
        {
            if (!CheckOutOfMovePattern(pattern))
            {
                isCheckOutOfMove = true;
                
                break;
            }
            else
            {
                Debug.LogError("Het duong di: " + pattern.name);
            }
        }
        if (isCheckOutOfMove == false)
        {
            EffectOutOfMove();
        }
    }
    private void EffectOutOfMove()
    {
        SoundManager.Instance.PlaySfx(SFX.Over);
        this.panelOutOfMove.gameObject.SetActive(true);
        this.panelOutOfMove.transform.position = new Vector3(this.panelOutOfMove.transform.position.x, this.panelOutOfMove.transform.position.y + 3);
        this.panelOutOfMove.color = new Color(1f, 1f, 1f, 0f);
        this.panelOutOfMove.DOFade(1f, 0.75f);
        this.panelOutOfMove.transform.DOLocalMoveY(0, 1.5f, false);
        this.panelOutOfMove.DOFade(0f, 0.75f).SetDelay(1.5f).OnComplete(delegate
        {
            this.panelOutOfMove.gameObject.SetActive(false);
            GameOverDialog dialog = GameManager.Instance.OnShowDialog<GameOverDialog>("Over", _nScore);
            if (dialog != null)
                dialog.setCallbackReplay(OnReplay);
        });
    }
    private bool CheckOutOfMovePattern(HexaPattern pattern)
    {
        if (pattern.TypePattern == HexaPattern.Type.SPECIAL)
        {
            return false;
        }
        pattern.SaveMap();
        for (int i = 0; i < this._Row; i++)
        {
            for (int j = 0; j < this._Col; j++)
            {
                int k = 0;
                while (k < 6)
                {
                    List<CellModel> listMapping = pattern.GetListMapping(i, j);
                    if (this.CheckAllowAddPattern(listMapping, pattern.Color))
                    {
                        pattern.RestoreMap();
                        return false;
                    }
                    k++;
                    pattern.ProcessRotate();
                }
            }
        }
        pattern.RestoreMap();
        return true;
    }

    #endregion

    #region Logic
    public Button btHammer;
    public void OnNewGame()
    {
        AdManager.Instance.ShowInterstitial();
        this.ClearScore();
        this.InitLogicListColor();
        this.CheckTransferColor();
        this.CreateThreePattern();
        this.btHammer.interactable = false;
    }
    public delegate void CallbackAddScore(int score);
    public List<CallbackAddScore> callbackScore;
    public void AddScore(int score)
    {
        this._nScore += score;
        foreach (CallbackAddScore callback in this.callbackScore)
        {
            callback.Invoke(this._nScore);
        }
    }
    public void ClearScore()
    {
        this._nScore = 0;
        foreach (CallbackAddScore callback in this.callbackScore)
        {
            callback.Invoke(this._nScore);
        }
    }
    private int CaculateScore(int nNumberPatternTriangle, int nCountListCheckHexa, ColorType ePatternColor, ref bool bCanGetDouble)
    {
        int num = 0;
        int nScore = this._nScore;
        if (ePatternColor == ColorType.SPECIAL_COLOR)
        {
            //this._nScore += 60;
            this.AddScore(60);
            num += 60;
            bCanGetDouble = false;
        }
        else
        {
            //this._nScore += nNumberPatternTriangle;
            this.AddScore(nNumberPatternTriangle);
            num = nNumberPatternTriangle;
            bCanGetDouble = false;
            if (nCountListCheckHexa == 6)
            {
                //this._nScore += 60;
                this.AddScore(60);
                num = 60;
            }
            if (nCountListCheckHexa >= 10)
            {
                //this._nScore += 240;
                this.AddScore(240);
                num = 240;
                bCanGetDouble = true;
            }
        }

        return num;
    }
    private void PlayEffectGetScore(int nScore, Vector2 posPattern, ColorType eColor)
    {
        string text = string.Empty;
        this._getScoreEffect.gameObject.SetActive(true);
        this._getScoreEffect.transform.position = posPattern;
        Vector2 vector = this._getScoreEffect.transform.localPosition;
        float arg_65_0 = (vector.x >= -270f) ? vector.x : -270f;
        float x = (vector.x <= 270f) ? vector.x : 270f;
        this._getScoreEffect.transform.localPosition = new Vector2(x, vector.y);
        this._getScoreEffect.color = new Color(1f, 1f, 1f, 0f);
        if (eColor == ColorType.SPECIAL_COLOR)
        {
            text = "+60 \n Special";
        }
        else
        {
            text = ((nScore != 240) ? ("+" + nScore.ToString()) : "+240 \n Get Double");
        }
        this._getScoreEffect.text = text;
        this._getScoreEffect.GetComponent<Outline>().effectColor = ((eColor != ColorType.SPECIAL_COLOR) ? this._listColorUI[(int)eColor] : Color.black);
        this._getScoreEffect.DOFade(1f, 0.75f);
        this._getScoreEffect.transform.DOLocalMoveY(this._getScoreEffect.transform.localPosition.y + 150f, 1f, false);
        this._getScoreEffect.DOFade(0f, 0.75f).SetDelay(1f).OnComplete(delegate
        {
            this._getScoreEffect.gameObject.SetActive(false);
        });
    }
    public void GamePause()
    { }
    public void EndGame()
    {

    }
    public void OnReplay()
    {
        this.ResetMap();
        this.ResetListPattern();
        this.OnNewGame();
    }
    #endregion

    #region Clear
    private void ResetMap()
    {
        for (int i = 0; i < this._Row; i++)
        {
            for (int j = 0; j < this._Col; j++)
            {
                this._mapData[i, j].Color = ColorType.NONE;
                if (this._mapData[i, j].Type != CellType.NONE)
                {
                    this._mapUI[i, j].ClearPatternTriangle();
                }
            }
        }
    }

    private void ResetListPattern()
    {
        for (int i = 0; i < this.listPattern.Count; i++)
        {
            if (this.listPattern[i] != null)
            {
                this.listPattern[i].DestroyPattern();
            }
        }
        this.listPattern.Clear();
    }
    #endregion

    #region Skill
    public bool isCanHammer;

    public bool isDeletePattern;
    public void ActiveHammer()
    {
        int num = 0;
        this.isCanHammer = true;
        for (int i = 0; i < this._Row; i++)
        {
            for (int j = 0; j < this._Col; j++)
            {
                if (this._mapData[i, j].Type != CellType.NONE)
                {
                    if (this._mapData[i, j].Color != ColorType.NONE)
                    {
                        this._mapUI[i, j].OnUseHammer(true);
                        num++;
                    }
                }
            }
        }
    }

    public void CancelHammer()
    {
        this.isCanHammer = false;
        for (int i = 0; i < this._Row; i++)
        {
            for (int j = 0; j < this._Col; j++)
            {
                if (this._mapData[i, j].Type != CellType.NONE)
                {
                    if (this._mapData[i, j].Color != ColorType.NONE)
                    {
                        this._mapUI[i, j].OnUseHammer(false);
                    }
                }
            }
        }
    }

    public void OnUseHammerClick(PointerEventData eventData, SkillPanel skillPanel, int coins)
    {
        if(this.isCanHammer == false)
        {
            return;
        }
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);
        TriangelItem triangel = getTriangelItemRayCat(results);
        if (triangel != null)
        {
            if (this._mapData[triangel.Row, triangel.Col].Color == ColorType.NONE)
            {
                this.CancelHammer();
            }
            else
            {
                this.CancelHammer();
                //////oogleAnalyticsV4.getInstance().LogEvent("SKILL", "Use Skill Hammer Success", string.Empty, 0L);

                UserProfile.Instance.ReduceDiamond(coins);
                this._mapData[triangel.Row, triangel.Col].Color = ColorType.NONE;
                this._mapUI[triangel.Row, triangel.Col].ClearPatternTriangle();
                SoundManager.Instance.PlaySfx(SFX.Clear);
            }
               
        }
        else
        {
            this.CancelHammer();
        }
        skillPanel.callbackUseSkill = null;
        skillPanel.gameObject.SetActive(false);

    }
   
    public void ActiveSkillDelete()
    {
        foreach(HexaPattern pattern in this.listPattern)
        {
            if(pattern != null)
            {
                pattern.ActiveSkillDeleTe(true);
            }
        }
    }
    public void CancelSkillDelete()
    {
        foreach (HexaPattern pattern in this.listPattern)
        {
            if (pattern != null)
            {
                pattern.ActiveSkillDeleTe(false);
            }
        }
    }
    public void OnUseDeleteClick(PointerEventData eventData, SkillPanel skillPanel, int coins)
    {

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);
        HexaPattern pattern = getHexaPattern(results);
        if (pattern != null)
        {
            //////oogleAnalyticsV4.getInstance().LogEvent("SKILL", "Use Skill Delete Success ", string.Empty, 0L);

            GameObject gameObject = this.InitDataPattern(pattern.IndexInPanel, false);
            gameObject.transform.position = this.listPosPattern[pattern.IndexInPanel].position;
            UserProfile.Instance.ReduceDiamond(coins);
            pattern.DestroyPattern();
            this.listPattern.Remove(pattern);
            SoundManager.Instance.PlaySfx(SFX.Clear);
            this.CancelSkillDelete();
        }
        else
        {
            this.CancelSkillDelete();
        }
        skillPanel.callbackUseSkill = null;
        skillPanel.gameObject.SetActive(false);
    }
    public HexaPattern getHexaPattern(List<RaycastResult> results)
    {
        foreach (RaycastResult result in results)
        {
            HexaPattern item = result.gameObject.GetComponent<HexaPattern>();
            if (item != null)
            {
                return item;
            }
        }
        return null;
    }
    #endregion
}
