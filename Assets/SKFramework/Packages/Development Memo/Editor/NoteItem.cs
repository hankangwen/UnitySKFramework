using System;

namespace SK.Framework
{
    [Serializable]
    public class NoteItem
    {
        /// <summary>
        /// 创建时间
        /// </summary>
        public string createdTime;

        /// <summary>
        /// 标题
        /// </summary>
        public string title;

        /// <summary>
        /// 作者
        /// </summary>
        public string author;

        /// <summary>
        /// 内容
        /// </summary>
        public string content;
    }
}