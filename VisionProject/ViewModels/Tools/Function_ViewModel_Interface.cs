using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*************************************************************************************
 *
 * 文 件 名:   Function_ViewModel_Interface
 * 描    述: 
 * 
 * 版    本：  V1.0.0.0
 * 创 建 者：  Bing
 * 创建时间：  2022/4/7 9:06:08
 * ======================================
 * 历史更新记录
 * 版本：V          修改时间：         修改人：
 * 修改内容：
 * ======================================
*************************************************************************************/
namespace VisionProject.ViewModels
{
    //规定好接口
    public interface IFunction_ViewModel_Interface
    {
        /// <summary>
        /// 读取参数
        /// </summary>
        /// <returns></returns>
       Task<bool> Init();

        /// <summary>
        /// 保存参数
        /// </summary>
        /// <returns></returns>
      bool  Update();
    }
}
