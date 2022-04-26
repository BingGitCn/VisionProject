using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Folding;
using System.Collections.Generic;

/*************************************************************************************
 *
 * 文 件 名:   HalconFold
 * 描    述:
 *
 * 版    本：  V1.0.0.0
 * 创 建 者：  Bing
 * 创建时间：  2022/4/26 8:47:24
 * ======================================
 * 历史更新记录
 * 版本：V          修改时间：         修改人：
 * 修改内容：
 * ======================================
*************************************************************************************/

namespace BingLibrary.Vision
{
    public class IniFoldingStrategy
    {
        public void UpdateFoldings(FoldingManager manager, TextDocument document)
        {
            var foldings = CreateNewFoldings(document, out var firstErrorOffset);
            manager.UpdateFoldings(foldings, firstErrorOffset);
        }

        private IEnumerable<NewFolding> CreateNewFoldings(TextDocument document, out int firstErrorOffset)
        {
            firstErrorOffset = -1;
            return CreateNewFoldings(document);
        }

        private IEnumerable<NewFolding> CreateNewFoldings(TextDocument document)
        {
            var newFoldings = new List<NewFolding>();
            var startOffset = 0;
            var isFolding = false;

            foreach (var line in document.Lines)
            {
                var text = document.GetText(line.Offset, line.Length).Trim();

                if (text.Replace(" ", "").StartsWith("*region"))
                {
                    if (isFolding)
                    {
                        newFoldings.Add(new NewFolding(startOffset, line.Offset - 2)); // -2 = \r\n
                    }

                    startOffset = line.Offset + line.Length;
                    isFolding = true;
                }

                //if (text.StartsWith(";"))
                //{
                //    if (isFolding)
                //    {
                //        var next = line.NextLine;
                //        var nextText = next == null ? "" : document.GetText(next.Offset, next.Length).Trim();
                //        if (nextText.StartsWith("["))
                //        {
                //            newFoldings.Add(new NewFolding(startOffset, line.Offset - 2)); // -2 = \r\n
                //            startOffset = line.Offset + line.Length;
                //        }
                //    }
                //}
            }

            if (isFolding)
            {
                newFoldings.Add(new NewFolding(startOffset, document.TextLength));
            }

            return newFoldings;
        }
    }
}