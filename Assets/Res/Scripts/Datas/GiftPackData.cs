using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Test
{
    /// <summary>
    /// 礼包数据
    /// </summary>
    public class GiftPackData
    {
        public string ID;
        /// <summary>
        /// 礼包名
        /// </summary>
        public string Name;
        /// <summary>
        /// 图标名
        /// </summary>
        public string IconName;
        /// <summary>
        /// 描述
        /// </summary>
        public string Description;
        /// <summary>
        /// 价格
        /// </summary>
        public int Price;
        /// <summary>
        /// 货币类型
        /// </summary>
        public E_MoneyType MoneyType;
    }
}