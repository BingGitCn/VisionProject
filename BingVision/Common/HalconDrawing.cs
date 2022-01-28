﻿/*************************************************************************************
 *
 * 文 件 名:   HalconDrawing
 * 描    述:
 *
 * 版    本：  V1.0.0.0
 * 创 建 者：  Bing
 * 创建时间：  2022/1/27 13:26:13
 * ======================================
 * 历史更新记录
 * 版本：V          修改时间：         修改人：
 * 修改内容：
 * ======================================
*************************************************************************************/

namespace BingLibrary.Vision
{
    public enum HalconDrawing
    {
        margin, fill
    }

    /// <summary>
    /// 鼠标绘制结束方式，松开结束绘制，或者点击右键结束绘制
    /// </summary>
    public enum HalconDrawMode
    { 
        directly,rightButton
    }
}