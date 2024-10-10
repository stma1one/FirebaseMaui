using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVVMSample.Helpers
{
    public static class TaskHelpers
    {
        #region טיפול בהפעלת פעולות אסינכרוניות מפעולות סינכרוניות
        /// <summary>
        /// Extention 
        /// המאפשר להפעיל פעולה א-סינכרונית מתוך פעולה סינכרונית- (לשימוש בפעולות בונות)
        /// </summary>
        /// <param name="task">המשימה שיש להפעיל</param>
        /// <param name="completedCallback">הפעולה שיש להפעיל במקרה של הצלחה</param>
        /// <param name="failedCallBack">פעולה שיש להפעיל במקרה של כשלון</param>
        /// 
        public static async void Awaiter(this Task task, Action? completedCallback = null, Action<Exception>? failedCallBack = null)
        {
            try
            {
                await task;
                completedCallback?.Invoke();
            }
            catch (Exception ex)
            {
                failedCallBack?.Invoke(ex);
            }
        }
        #endregion
    }
}
