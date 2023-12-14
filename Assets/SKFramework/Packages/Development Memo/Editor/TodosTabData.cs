using System;
using System.Collections.Generic;

namespace SK.Framework
{
    /// <summary>
    /// 待办Tab页数据类
    /// </summary>
    [Serializable]
    public class TodosTabData
    {
        public List<TodoItem> todos = new List<TodoItem>(0);
    }
}