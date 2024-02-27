using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Test
{
    public class GameInfoManager : MonoBehaviour
    {
        public static GameInfoManager Ins;

        /// <summary>
        /// 货币数量字典
        /// </summary>
        private readonly Dictionary<E_MoneyType, int> dicMoney = new()
        {
            { E_MoneyType.MoneyA, 100 },
            { E_MoneyType.MoneyB, 100 },
            { E_MoneyType.MoneyC, 100 }
        };
        /// <summary>
        /// 是否购买过礼包的标记
        /// </summary>
        private readonly HashSet<string> setBuyGiftFlag = new();
        /// <summary>
        /// 购买礼包回调
        /// </summary>
        public Action BuyGiftAction;
        /// <summary>
        /// 货币变化回调
        /// </summary>
        public Action<E_MoneyType> MoneyChangeAction;

        private void Awake()
        {
            if (Ins != null)
                Destroy(gameObject);

            Ins = this;
        }

        /// <summary>
        /// 礼包是否购买过
        /// </summary>
        /// <param name="gift"></param>
        /// <returns></returns>
        public bool IsGiftBuy(string giftId)
        {
            return setBuyGiftFlag.Contains(giftId);
        }

        /// <summary>
        /// 添加礼包购买状态
        /// </summary>
        public void AddGiftBuyState(GiftPackData data)
        {
            if (setBuyGiftFlag.Add(data.ID))
                BuyGiftAction?.Invoke();
        }

        public int GetMoneyByType(E_MoneyType moneyType)
        {
            if (dicMoney.TryGetValue(moneyType, out int count))
                return count;

            return 0;
        }

        /// <summary>
        /// 增量修改货币
        /// </summary>
        /// <param name="moneyType"></param>
        /// <param name="change"></param>
        /// <returns></returns>
        public void AddMoney(E_MoneyType moneyType, int change)
        {
            if (dicMoney.TryGetValue(moneyType, out int count))
            {
                if (count + change < 0) // 根据需求 货币能否为负值
                    throw new Exception("错误");
                else
                    dicMoney[moneyType] = count + change;
            }
            else
            {
                if(change < 0)
                    throw new Exception("错误");
                else
                    dicMoney[moneyType] = change;
            }

            MoneyChangeAction?.Invoke(moneyType);
        }
    }
}