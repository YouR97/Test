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
        /// 文件保存路径
        /// </summary>
        private static readonly string savePath = $"{Application.dataPath}/Res/JsonDatas";
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
            string path = Path.Combine(savePath, "GiftPackDatas.json");
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

            listGiftPackData ??= new List<GiftPackData>();

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
                GUILayout.Label("ID", GUILayout.Width(50f));
                EditorGUILayout.LabelField(giftPackData.ID, GUILayout.Width(300f));

                GUILayout.Label("礼包名", GUILayout.Width(50f));
                giftPackData.Name = EditorGUILayout.TextField(giftPackData.Name, GUILayout.Width(300f));

                GUILayout.Label("礼包图标", GUILayout.Width(70f));
                giftPackData.IconName = EditorGUILayout.TextField(giftPackData.IconName, GUILayout.Width(300f));

                GUILayout.Label("礼包描述", GUILayout.Width(80f));
                giftPackData.Description = EditorGUILayout.TextField(giftPackData.Description, GUILayout.Width(300f));

                GUILayout.Label("价格", GUILayout.Width(50f));
                giftPackData.Price = EditorGUILayout.IntField(giftPackData.Price, GUILayout.Width(300f));

                GUILayout.Label("货币类型", GUILayout.Width(80f));
                giftPackData.MoneyType = (E_MoneyType)EditorGUILayout.EnumPopup(giftPackData.MoneyType, GUILayout.Width(300f));
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
                    ID = System.Guid.NewGuid().ToString(),
                    Name = "新礼包",
                    IconName = "礼包",
                    Description = "描述",
                    Price = 0,
                    MoneyType = E_MoneyType.MoneyA,
                });

            EditorGUILayout.EndVertical();
        }

        private void DrawBottom()
        {
            if (GUILayout.Button("保存数据"))
            {
                foreach (GiftPackData data in listGiftPackData)
                {
                    if (string.IsNullOrWhiteSpace(data.ID))
                        data.ID = System.Guid.NewGuid().ToString();
                }

                string path = Path.Combine(savePath, "GiftPackDatas.json");
                FileStream fs = new(path, FileMode.OpenOrCreate);
                byte[] b = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(listGiftPackData));
                fs.Write(b, 0, b.Length);
                fs.Close();

                AssetDatabase.Refresh();
            }
        }
    }
}