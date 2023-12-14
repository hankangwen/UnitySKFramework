using System;

namespace SK.Framework
{
    /// <summary>
    /// 待办项
    /// </summary>
    [Serializable]
    public class TodoItem 
    {
        /// <summary>
        /// 标题
        /// </summary>
        public string title;

        /// <summary>
        /// 创建人
        /// </summary>
        public string creator;

        /// <summary>
        /// 待办描述
        /// </summary>
        public string description;

        /// <summary>
        /// 创建时间
        /// </summary>
        public string createdTime;

        /// <summary>
        /// 预计完成时间
        /// </summary>
        public DateTime estimatedCompletedTime;

        /// <summary>
        /// 是否已经完成
        /// </summary>
        public bool isCompleted;

        /// <summary>
        /// 是否逾期
        /// </summary>
        public bool isOverdue;

        public TodoItem()
        {
            //默认的预计完成日期为明天
            estimatedCompletedTime = DateTime.Now.Date.AddDays(1);
        }

        public void OverdueCal()
        {
            //计算是否已经逾期
            isOverdue = !isCompleted && (DateTime.Now - estimatedCompletedTime).Days > 0;
        }
    }
}