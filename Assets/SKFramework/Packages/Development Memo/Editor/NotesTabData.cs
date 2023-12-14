using System;
using System.Collections.Generic;

namespace SK.Framework
{
    /// <summary>
    /// 笔记Tab页数据类
    /// </summary>
    [Serializable]
    public class NotesTabData
    {
        /// <summary>
        /// 笔记列表
        /// </summary>
        public List<NoteItem> notes = new List<NoteItem>(0);
    }
}