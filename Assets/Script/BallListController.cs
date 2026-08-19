using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Popup;
using UnityEngine.UIElements;

public class BallListController : MonoBehaviour
{

    VisualTreeAsset _ListEntryTemplate;

    ListView _listview;

    List<List<int>> dataList;

    SQLiteCon dbconn;

    VisualElement _parentroot;

    List<int> winningNumbers;

    public void InitializeList(VisualElement root, VisualTreeAsset listElementTemplate)
    {

        dataList = new List<List<int>>();

        //dbconn = SQLiteCon.Instance;
        ////dbconn.DBRead("Select * From NumList");

        //dataList = dbconn.DBReadAll();

        ////dataList.Add(new List<int>() { 0, 1, 2, 3, 4, 5, 6 });
        ////dataList.Add(new List<int>() { 1, 1, 2, 3, 4, 5, 6 });

        _ListEntryTemplate = listElementTemplate;
        _listview = root.Q<ListView>("ListView");
        // 스크롤바 안보이게
        _listview.Q<ScrollView>().verticalScrollerVisibility = ScrollerVisibility.Hidden;

        _parentroot = root;

        //FillList();

        StartCoroutine(WaitForDataBaseAndInitialize());
    }

    private IEnumerator WaitForDataBaseAndInitialize()
    {
        dbconn = SQLiteCon.Instance;

        //db가 초기화 및 연결완료되면
        while (!dbconn.IsReady())
        {
            //Debug.Log("dbconn.IsReady() is false");
            yield return new WaitForSeconds(0.1f);
        }

        dataList = dbconn.DBReadAll();
        FillList();
    }

    void FillList()
    {
        _listview.makeItem = () =>
        {
            var newListEntry = _ListEntryTemplate.Instantiate();

            var newListEntryLogic = new BallItemController();

            newListEntry.userData = newListEntryLogic;

            //newListEntry.RegisterCallback<ClickEvent>(OnItemClicked);

            newListEntryLogic.SetVisualElement(newListEntry);


            return newListEntry;
        };

        _listview.bindItem = (item, index) =>
        {
            (item.userData as BallItemController).SetNumber(dataList[index][0], dataList[index][1], dataList[index][2], dataList[index][3], dataList[index][4], dataList[index][5], dataList[index][6]);
            (item.userData as BallItemController).SetColorMatchNumber(winningNumbers);

            // 기존 콜백 제거후 새로 등록

            item.RemoveLongClickCallback();

            int capturedIndex = index;

            item.AddLongClickCallback(() =>
            {
                int capturedIndex = index;
                // item.AddLongClickCallback(() =>
                // {




                //Debug.Log($"Item {index} long Click");

                // Popup.Dialog dialog = new Popup.Dialog();
                // _parentroot.Add(dialog);
                // dialog.setNumMsg(string.Join(",", dataList[index][0].ToString(), dataList[index][1].ToString(), dataList[index][2].ToString(), dataList[index][3].ToString(), dataList[index][4].ToString(), dataList[index][5].ToString(), dataList[index][6].ToString()));
                // dialog.setMsg("해당 번호를 삭제하시겠습니까?");

                // dialog.confirmed += () => {
                //     //db상 삭제
                //     dbconn.DBDeleteItem(dataList[index]);

                //     //리스트 상 삭제
                //     var items = _listview.itemsSource as List<List<int>>;
                //     items.RemoveAt(index);
                //     _listview.RefreshItems();

                //     //다이어로그 닫기
                //     _parentroot.Remove(dialog);
                // };
                // dialog.canceled += () => { _parentroot.Remove(dialog); };

                // 롱클릭 다이어로그 띄우기
                UIMain.showLongClickDialog(string.Join(",", dataList[capturedIndex][0].ToString(), dataList[capturedIndex][1].ToString(), dataList[capturedIndex][2].ToString(), dataList[capturedIndex][3].ToString(), dataList[capturedIndex][4].ToString(), dataList[capturedIndex][5].ToString(), dataList[capturedIndex][6].ToString()), () =>
                {
                    //db상 삭제
                    dbconn.DBDeleteItem(dataList[index]);

                    //리스트 상 삭제
                    var items = _listview.itemsSource as List<List<int>>;
                    items.RemoveAt(index);
                    // _listview.RefreshItems();
                    _listview.Rebuild();
                });
            });

        };
        

        //unbindItem 추가 - 메모리 누수 방지
        _listview.unbindItem = (item, index) =>
        {
            // 롱클릭 콜백 제거
            item.RemoveLongClickCallback();
        };



        _listview.style.overflow = Overflow.Hidden;

        _listview.fixedItemHeight = 20;
        _listview.virtualizationMethod = CollectionVirtualizationMethod.FixedHeight;

        _listview.itemsSource = dataList;
    }

    public bool AddBallList()
    {


        // 저장된 길이가 10개 이하면
        if (dataList.Count < GlobalVal.getBallListCount)
        {
            //정렬
            GlobalVal.getBallNumber.Sort();
            //보너스 숫자 합해서 리스트화
            GlobalVal.getBallNumber.Add(GlobalVal.getBallBonusNumber);

            dataList.Insert(0,GlobalVal.getBallNumber);
            dbconn.DBAddList(GlobalVal.getBallNumber);
            _listview.Rebuild();
            return true;
        }
        else // 10개 이상이면
        {
            UIMain.showOverListDialog();
            return false;
        }
        

        
    }

    public void ColorMatchNumber(List<int> winningNumbers)
    {
        this.winningNumbers = winningNumbers;
        _listview.Rebuild();
    }

    /*void OnDestroy()
    {
        // 메모리 누수 방지
        _listview?.RemoveItemLongClickCallback();
    }*/

        // *** 중요: OnDestroy 활성화로 메모리 누수 방지 ***
    void OnDestroy()
    {
        // ListView 정리
        if (_listview != null)
        {
            // itemsSource를 null로 설정하여 바인딩 해제
            _listview.itemsSource = null;
            
            // makeItem, bindItem 콜백 제거
            _listview.makeItem = null;
            _listview.bindItem = null;
            _listview.unbindItem = null;
        }

        // 리스트 정리
        if (dataList != null)
        {
            dataList.Clear();
            dataList = null;
        }

        // 가비지 컬렉션 강제 실행
        System.GC.Collect();
    }

    // OnDisable 추가 - 씬 전환 시에도 정리
    void OnDisable()
    {
        if (_listview != null)
        {
            // itemsSource를 임시로 null로 설정
            _listview.itemsSource = null;
        }
    }
    
    // OnEnable 추가 - 다시 활성화될 때 복구
    void OnEnable()
    {
        if (_listview != null && dataList != null && dataList.Count > 0)
        {
            _listview.itemsSource = dataList;
        }
    }
}
