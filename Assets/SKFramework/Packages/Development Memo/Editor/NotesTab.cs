using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;

using UnityEngine;
using UnityEditor;

namespace SK.Framework
{
    /// <summary>
    /// 开发笔记Tab页
    /// </summary>
    public class NotesTab : IDevelopmentMemoTab
    {
        private readonly DevelopmentMemo window;
        //数据文件路径
        private readonly string dataPath;
        //数据类
        [SerializeField] private NotesTabData data;
        //笔记列表的宽度
        private float listRectWidth = 280f;
        //左右分割线区域
        private Rect splitterRect;
        //是否正在拖拽分割线
        private bool isDragging;
        //列表滚动值
        private Vector2 listScroll;
        //笔记详情滚动值
        private Vector2 detailScroll;
        //当前选中的笔记
        private NoteItem currentNote;
        //标题的最大长度
        private const int titleLengthLimit = 20;
        //检索内容
        private string searchContent;

        public NotesTab(DevelopmentMemo window)
        {
            this.window = window;
            //拼接数据文件路径
            dataPath = Path.GetFullPath(".").Replace("\\", "/") + "/Library/DevelopmentMemo_Notes.dat";
            //判断文件是否存在
            if (File.Exists(dataPath))
            {
                using (FileStream fs = File.Open(dataPath, FileMode.Open))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    //反序列化
                    var deserialize = bf.Deserialize(fs);
                    if (deserialize != null)
                        data = deserialize as NotesTabData;
                    //反序列化失败 数据为空
                    if (data == null)
                    {
                        //删除无效数据文件 并初始化一个新的数据类
                        File.Delete(dataPath);
                        data = new NotesTabData();
                    }
                    else
                    {
                        //默认按照创建时间排序
                        data.notes = data.notes.OrderBy(m => m.createdTime).ToList();
                    }
                }
            }
            else data = new NotesTabData();
        }

        public void OnGUI()
        {
            OnTopGUI();
            OnBodyGUI();
        }

        private void OnTopGUI()
        {
            GUILayout.BeginHorizontal();
            //排序按钮
            GUI.enabled = data != null && data.notes.Count > 0;
            if (GUILayout.Button("Sort", EditorStyles.toolbarDropDown, GUILayout.Width(50f)))
            {
                GenericMenu gm = new GenericMenu();
                gm.AddItem(new GUIContent("Name ↓"), false, () => data.notes = data.notes.OrderBy(m => m.title).ToList());
                gm.AddItem(new GUIContent("Name ↑"), false, () => data.notes = data.notes.OrderByDescending(m => m.title).ToList());
                gm.AddItem(new GUIContent("Created Time ↓"), false, () => data.notes = data.notes.OrderBy(m => m.createdTime).ToList());
                gm.AddItem(new GUIContent("Created Time ↑"), false, () => data.notes = data.notes.OrderByDescending(m => m.createdTime).ToList());
                gm.ShowAsContext();
            }
            GUI.enabled = true;
            //检索输入框
            searchContent = GUILayout.TextField(searchContent, EditorStyles.toolbarSearchField);
            //当点击鼠标且鼠标位置不在输入框中时 取消控件的聚焦
            //if (Event.current.type == EventType.MouseDown && !GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
            //{
            //    GUI.FocusControl(null);
            //    window.Repaint();
            //}
            GUILayout.EndHorizontal();
        }

        private void OnBodyGUI()
        {
            GUILayout.BeginHorizontal();
            {
                //左侧列表
                GUILayout.BeginVertical(GUILayout.Width(listRectWidth));
                OnLeftGUI();
                GUILayout.EndVertical();

                //分割线
                GUILayout.BeginVertical(GUILayout.ExpandHeight(true), GUILayout.MaxWidth(5f));
                GUILayout.Box(string.Empty, "EyeDropperVerticalLine", GUILayout.ExpandHeight(true));
                GUILayout.EndVertical();
                splitterRect = GUILayoutUtility.GetLastRect();

                //右侧详情
                GUILayout.BeginVertical(GUILayout.ExpandWidth(true));
                OnRightGUI();
                GUILayout.EndVertical();
            }
            GUILayout.EndHorizontal();

            if (Event.current != null)
            {
                //光标
                EditorGUIUtility.AddCursorRect(splitterRect, MouseCursor.ResizeHorizontal);
                switch (Event.current.rawType)
                {
                    //开始拖拽分割线
                    case EventType.MouseDown:
                        isDragging = splitterRect.Contains(Event.current.mousePosition);
                        break;
                    case EventType.MouseDrag:
                        if (isDragging)
                        {
                            listRectWidth += Event.current.delta.x;
                            //限制其最大最小值
                            listRectWidth = Mathf.Clamp(listRectWidth, window.position.width * .3f, window.position.width * .8f);
                            window.Repaint();
                        }
                        break;
                    //结束拖拽分割线
                    case EventType.MouseUp:
                        if (isDragging)
                            isDragging = false;
                        break;
                }
            }
        }

        private void OnLeftGUI()
        {
            //滚动视图
            listScroll = EditorGUILayout.BeginScrollView(listScroll);
            {
                //遍历笔记列表
                for (int i = 0; i < data.notes.Count; i++)
                {
                    var note = data.notes[i];
                    //如果检索输入框不为空 判断是否符合检索内容
                    if (!string.IsNullOrEmpty(searchContent) && !note.title.ToLower().Contains(searchContent.ToLower())) continue;

                    GUILayout.BeginHorizontal(currentNote == note ? "MeTransitionSelectHead" : "ProjectBrowserHeaderBgTop");
                    GUILayout.Label(note.title);
                    GUILayout.EndHorizontal();
                    //鼠标点击选中当前项
                    if (Event.current.type == EventType.MouseDown && GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
                    {
                        if (currentNote != note)
                        {
                            GUI.FocusControl(null);
                            currentNote = note;
                            window.Repaint();
                        }
                    }
                }
            }
            EditorGUILayout.EndScrollView();

            GUILayout.FlexibleSpace();

            //创建新的笔记
            if (GUILayout.Button("Create New", EditorStyles.miniButton))
            {
                var note = new NoteItem()
                {
                    title = "New Note",
                    createdTime = DateTime.Now.ToString(),
                };
                //添加到数据列表
                data.notes.Add(note);
            }
            GUILayout.Space(2.5f);
        }

        private void OnRightGUI() 
        {
            if (currentNote == null) return;

            detailScroll = EditorGUILayout.BeginScrollView(detailScroll);
            //标题
            GUILayout.Label("标题：", EditorStyles.boldLabel);
            GUILayout.BeginHorizontal(EditorStyles.helpBox);
            string newTitle = EditorGUILayout.TextField(currentNote.title, EditorStyles.label);
            if (newTitle != currentNote.title)
            {
                //长度限制
                if (newTitle.Length > 0 && newTitle.Length <= titleLengthLimit)
                    currentNote.title = newTitle;
            }
            GUILayout.FlexibleSpace();
            GUILayout.Label(string.Format("{0}/{1}", currentNote.title.Length, titleLengthLimit), EditorStyles.miniBoldLabel);
            GUILayout.EndHorizontal();

            //日期
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label(currentNote.createdTime, EditorStyles.miniBoldLabel);
            GUILayout.EndHorizontal();

            //作者
            GUILayout.Label("作者：", EditorStyles.boldLabel);
            GUILayout.BeginHorizontal(EditorStyles.helpBox);
            currentNote.author = EditorGUILayout.TextField(currentNote.author, EditorStyles.label);
            GUILayout.EndHorizontal();
            GUILayout.Space(20f);


            //内容
            GUILayout.Label("内容：", EditorStyles.boldLabel);
            currentNote.content = EditorGUILayout.TextArea(currentNote.content, 
                GUILayout.MaxWidth(window.position.width - listRectWidth - 15f), GUILayout.MinHeight(200f));

            //删除
            EditorGUILayout.Space();
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Delete"))
            {
                if (EditorUtility.DisplayDialog("Notice", "Whether to delete the note ?", "Confirm", "Cancle"))
                {
                    data.notes.Remove(currentNote);
                    currentNote = null;
                    window.Repaint();
                }
            }
            GUILayout.EndHorizontal();
            EditorGUILayout.EndScrollView();
        }

        public void OnDisable()
        {
            try
            {
                //写入数据文件进行保存
                using (FileStream fs = File.Create(dataPath))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    //序列化
                    bf.Serialize(fs, data);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
            }
        }
    }
}