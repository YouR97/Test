using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using Newtonsoft.Json;
using System.Text;
using Test;

namespace Editor.Test
{
    public class GiftPackEditor : EditorWindow
    {
        /// <summary>
        /// 礼包数据列表
        /// </summary>
        private static List<GiftPackData> listGiftPackData;
        /// <summary>
        /// 当前选中礼包数据
        /// </summary>
        private static GiftPackData giftPackData;

        [MenuItem("Tools/礼包配置", false)]
        private static void ShowWindow()
        {
            listGiftPackData = null;
            string path = Path.Combine(Application.streamingAssetsPath, "GiftPackDatas.json");
            if (File.Exists(path))
            {
                string txt = File.ReadAllText(path);
                try
                {
                    listGiftPackData = JsonConvert.DeserializeObject<List<GiftPackData>>(txt);
                }
                catch (System.Exception)
                {
                    Debug.LogWarning("反序列化失败");
                }
            }

            if (listGiftPackData == null)
                listGiftPackData = new List<GiftPackData>();

            GetWindow(typeof(GiftPackEditor));
        }

        private void OnGUI()
        {
            EditorGUILayout.BeginHorizontal();
            DrawLeft();
            DrawRight();
            EditorGUILayout.EndHorizontal();

            DrawBottom();
        }

        private void DrawLeft()
        {
            EditorGUILayout.BeginVertical();

            EditorGUILayout.LabelField("当前对象", GUILayout.Width(250));

            if (giftPackData == null)
                EditorGUILayout.LabelField("当前未选择对象", GUILayout.Width(100));
            else
            {
                GUILayout.Label("礼包名", GUILayout.Width(50f));
                giftPackData.Name = EditorGUILayout.TextField(giftPackData.Name, GUILayout.Width(200f));

                GUILayout.Label("礼包图标", GUILayout.Width(70f));
                giftPackData.IconName = EditorGUILayout.TextField(giftPackData.IconName, GUILayout.Width(200f));

                GUILayout.Label("礼包描述", GUILayout.Width(80f));
                giftPackData.Description = EditorGUILayout.TextField(giftPackData.Description, GUILayout.Width(200f));

                GUILayout.Label("价格", GUILayout.Width(50f));
                giftPackData.Price = EditorGUILayout.IntField(giftPackData.Price, GUILayout.Width(200f));

                GUILayout.Label("货币类型", GUILayout.Width(80f));
                giftPackData.MoneyType = (E_MoneyType)EditorGUILayout.EnumPopup(giftPackData.MoneyType, GUILayout.Width(200f));
            }

            if (GUILayout.Button("删除数据", GUILayout.Width(100)))
            {
                listGiftPackData.Remove(giftPackData);
                giftPackData = null;
            }

            EditorGUILayout.EndVertical();
        }

        private void DrawRight()
        {
            EditorGUILayout.BeginVertical();

            EditorGUILayout.LabelField("对象列表", GUILayout.Width(350));
            foreach (var item in listGiftPackData)
            {
                if (GUILayout.Button(item.Name, GUILayout.Width(300)))
                    giftPackData = item;
            }

            if (GUILayout.Button("新增数据", GUILayout.Width(100)))
                listGiftPackData.Add(new GiftPackData() 
                {
                    Name = "新礼包",
                    IconName = "礼包",
                });

            EditorGUILayout.EndVertical();
        }

        private void DrawBottom()
        {
            if (GUILayout.Button("保存数据"))
            {
                string path = Path.Combine(Application.streamingAssetsPath, "GiftPackDatas.json");
                FileStream fs = new FileStream(path, FileMode.OpenOrCreate);
                byte[] b = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(listGiftPackData));
                fs.Write(b, 0, b.Length);
                fs.Close();
            }
        }
    }
}