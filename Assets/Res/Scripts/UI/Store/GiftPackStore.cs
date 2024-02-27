using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

namespace Test
{
    /// <summary>
    /// 礼包商店UI
    /// </summary>
    public class GiftPackStore : MonoBehaviour
    {
        /// <summary>
        /// 购买状态
        /// </summary>
        public enum E_BuyState
        {
            Success,
            Failure
        }

        /// <summary>
        /// 礼包类
        /// </summary>
        private class Gift
        {
            #region UI
            /// <summary>
            /// 根节点
            /// </summary>
            private GameObject root;
            /// <summary>
            /// 图标
            /// </summary>
            private Image imgIcon;
            /// <summary>
            /// 购买按钮
            /// </summary>
            private Button btnBuy;
            /// <summary>
            /// 礼包名
            /// </summary>
            private Text textName;
            /// <summary>
            /// 货币类型和价格
            /// </summary>
            private Text textPrice;
            /// <summary>
            /// 描述
            /// </summary>
            private Text textDescription;
            /// <summary>
            /// 已经被购买的状态
            /// </summary>
            private GameObject goBuy;
            #endregion

            private GiftPackData data;
            /// <summary>
            /// 购买回调
            /// </summary>
            private readonly Action<GiftPackData> buyAction;

            public Gift(GameObject root, Action<GiftPackData> buy)
            {
                this.root = root;
                buyAction = buy;

                btnBuy = root.GetComponent<Button>();
                goBuy = root.transform.Find("BuyState").gameObject;
                imgIcon = root.transform.Find("ImageGiftIcon").GetComponent<Image>();
                textName = root.transform.Find("TextName").GetComponent<Text>();
                textPrice = root.transform.Find("TextPrice").GetComponent<Text>();
                textDescription = root.transform.Find("TextDescription").GetComponent<Text>();

                btnBuy.onClick.AddListener(OnBuy);
            }

            public void SetInfo(GiftPackData giftPackData)
            {
                data = giftPackData;

                if (GameInfoManager.Ins.IsGiftBuy(data.ID))
                    goBuy.transform.localScale = Vector3.one;
                else
                    goBuy.transform.localScale = Vector3.zero;

                textName.text = data.Name;
                textDescription.text = data.Description;
                textPrice.text = $"价格：{data.Price}{data.MoneyType}";

                SpriteAtlas spriteAtlas = (SpriteAtlas)AssetDatabase.LoadMainAssetAtPath("Assets/Res/Arts/Icon.spriteatlas");

                if (spriteAtlas != null)
                    imgIcon.sprite = spriteAtlas.GetSprite(data.IconName);
            }

            /// <summary>
            /// 点击购买
            /// </summary>
            private void OnBuy()
            {
                buyAction?.Invoke(data);
            }
        }

        /// <summary>
        /// 货币类
        /// </summary>
        private class Money
        {
            /// <summary>
            /// 根节点
            /// </summary>
            private GameObject root;
            /// <summary>
            /// 货币
            /// </summary>
            private Text textMoney;
            private Animator animator;

            public Money(GameObject root)
            {
                this.root = root;

                textMoney = root.GetComponent<Text>();
                animator = root.GetComponent<Animator>();
            }

            public void SetInfo(E_MoneyType moneyType)
            {
                textMoney.text = $"{moneyType}: {GameInfoManager.Ins.GetMoneyByType(moneyType)}";
            }

            public void PlayAnimator()
            {
                animator.SetTrigger("Play");
            }
        }

        /// <summary>
        /// 提示
        /// </summary>
        private class Tip
        {
            public GameObject root;
            /// <summary>
            /// 内容
            /// </summary>
            private Text textContent;
            private Animator animator;

            public Tip(GameObject root)
            {
                this.root = root;

                textContent = root.GetComponent<Text>();
                animator = root.GetComponent<Animator>();
            }

            public void SetInfo(string content, E_BuyState buyState)
            {
                textContent.text = content;

                switch (buyState)
                {
                    case E_BuyState.Success:
                        {
                            textContent.color = Color.green;
                        }
                        break;
                    case E_BuyState.Failure:
                        {
                            textContent.color = Color.red;
                        }
                        break;
                    default:
                        {
                            Debug.LogWarning("未处理类型");
                        }
                        break;
                }

                animator.SetTrigger("Play");
            }
        }

        /// <summary>
        /// 无限列表
        /// </summary>
        public RecycleView recycleView;
        /// <summary>
        /// 礼包数据资源
        /// </summary>
        public TextAsset GiftDataAsset;

        /// <summary>
        /// 礼包数据
        /// </summary>
        private List<GiftPackData> listGiftPackData;

        /// <summary>
        /// 实例id对于礼包类字典
        /// </summary>
        private readonly Dictionary<int, Gift> dicGift = new();
        /// <summary>
        /// 货币栏字典
        /// </summary>
        private readonly Dictionary<E_MoneyType, Money> dicMoney = new();
        private Tip tip;

        private void Awake()
        {
            listGiftPackData = null;

            try
            {
                listGiftPackData = JsonConvert.DeserializeObject<List<GiftPackData>>(GiftDataAsset.text);
            }
            catch (Exception)
            {
                Debug.LogWarning("反序列化失败");
            }

            dicMoney[E_MoneyType.MoneyA] = new Money(transform.Find("Money/TextMoneyA").gameObject);
            dicMoney[E_MoneyType.MoneyB] = new Money(transform.Find("Money/TextMoneyB").gameObject);
            dicMoney[E_MoneyType.MoneyC] = new Money(transform.Find("Money/TextMoneyC").gameObject);
            foreach (var item in dicMoney)
            {
                item.Value.SetInfo(item.Key);
            }

            tip = new Tip(transform.Find("TextTip").gameObject);

            listGiftPackData ??= new List<GiftPackData>();

            GameInfoManager.Ins.BuyGiftAction += RefreshGiftList;
            GameInfoManager.Ins.MoneyChangeAction += RefreshMoney;
        }

        private void Start()
        {
            StartScrollView();
        }

        private void OnDestroy()
        {
            GameInfoManager.Ins.BuyGiftAction -= RefreshGiftList;
            GameInfoManager.Ins.MoneyChangeAction -= RefreshMoney;
        }

        /// <summary>
        /// 初始化无限列表
        /// </summary>
        public void StartScrollView()
        {
            recycleView.Init(ScrollCallBack);
            recycleView.ShowList(listGiftPackData.Count);
        }

        /// <summary>
        /// 无限列表回调
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="index"></param>
        private void ScrollCallBack(GameObject cell, int index)
        {
            if (!dicGift.TryGetValue(cell.GetInstanceID(), out Gift gift))
            {
                gift = new(cell, BuyGift);
                dicGift.Add(cell.GetInstanceID(), gift);
            }

            if (index < 0 || index >= listGiftPackData.Count)
                throw new Exception("数据异常");

            gift.SetInfo(listGiftPackData[index]);
        }

        /// <summary>
        /// 购买礼包
        /// </summary>
        private void BuyGift(GiftPackData data)
        {
            if (GameInfoManager.Ins.IsGiftBuy(data.ID))
            {
                tip.SetInfo("卖光了卖光了", E_BuyState.Failure);
                return;
            }

            if (GameInfoManager.Ins.GetMoneyByType(data.MoneyType) < data.Price)
            {
                tip.SetInfo($"{data.MoneyType}货币不足", E_BuyState.Failure);
                if (dicMoney.TryGetValue(data.MoneyType, out Money money))
                    money.PlayAnimator();

                return;
            }

            GameInfoManager.Ins.AddMoney(data.MoneyType, -data.Price);
            GameInfoManager.Ins.AddGiftBuyState(data);

            tip.SetInfo("购买成功", E_BuyState.Success);
        }

        /// <summary>
        /// 刷新礼包列表显示状态
        /// </summary>
        private void RefreshGiftList()
        {
            recycleView.UpdateList();
        }

        /// <summary>
        /// 刷新货币
        /// </summary>
        private void RefreshMoney(E_MoneyType moneyType)
        {
            if (dicMoney.TryGetValue(moneyType, out Money money))
            {
                money.SetInfo(moneyType);
            }
        }
    }
}