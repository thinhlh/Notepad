using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notepad.Classes
{
    /// <summary>
    /// Use to serialize details of current window to JSON for reopening later
    /// </summary>
    public class TemporaryDetail
    {
        /// <summary>
        /// store path of TabItem
        /// </summary>
        public string path { get; set; }

        /// <summary>
        /// store TabItem Name include * for saved or unsaved
        /// can be descript later to set isSaved
        /// </summary>
        public string header { get; set; }

        /// <summary>
        /// store value of richtextbox
        /// </summary>
        public string text { get; set; }

        /// <summary>
        /// store language of current RTB
        /// </summary>
        public string language { get; set; }
    }
}
