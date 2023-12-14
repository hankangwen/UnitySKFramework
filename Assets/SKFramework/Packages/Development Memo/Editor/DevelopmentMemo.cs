using UnityEngine;
using UnityEditor;

namespace SK.Framework
{
    /// <summary>
    /// 开发备忘录
    /// </summary>
    [Package("Development Memo", "1.0.0")]
    public class DevelopmentMemo : EditorWindow
    {
        [MenuItem("SKFramework/Development Memo")]
        public static void Open()
        {
            GetWindow<DevelopmentMemo>("Development Memo").Show();
        }

        private enum Menu
        {
            Notes,
            Todos,
        }

        private Menu menu = Menu.Notes;
        private NotesTab notesTab;
        private TodosTab todosTab;

        private void OnEnable()
        {
            if (notesTab == null) 
                notesTab = new NotesTab(this);
            if (todosTab == null) 
                todosTab = new TodosTab(this);
        }

        private void OnGUI()
        {
            OnTopGUI();
            OnBodyGUI();
        }

        private void OnDisable()
        {
            notesTab?.OnDisable();
            todosTab?.OnDisable();
        }

        private void OnTopGUI()
        {
            Color cacheColor = GUI.color;
            Color selectedColor = Color.white;
            selectedColor.a = .5f;
            GUILayout.BeginHorizontal();
            GUI.color = menu == Menu.Notes ? Color.white : selectedColor;
            if (GUILayout.Button("Notes", EditorStyles.miniButtonLeft)) menu = Menu.Notes;
            GUI.color = menu == Menu.Todos ? Color.white : selectedColor;
            if (GUILayout.Button("Todos", EditorStyles.miniButtonRight)) menu = Menu.Todos;
            GUILayout.EndHorizontal();
            GUI.color = cacheColor;
        }

        private void OnBodyGUI()
        {
            switch (menu)
            {
                case Menu.Notes: notesTab.OnGUI(); break;
                case Menu.Todos: todosTab.OnGUI(); break;
                default: break;
            }
        }
    }
}