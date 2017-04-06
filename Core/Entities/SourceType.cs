using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Enums
{
    /// <summary>
    /// пока оставим только "глобальные минус\плюс сллова, 
    /// применяются ко ВСЕМ источникам новостей
    /// </summary>
    public enum SourceType:int
    {
        Rss = 1,
        /// <summary>
        /// maybe with words
        /// </summary>
        Url= 2,
        Push = 3
    }    
}

