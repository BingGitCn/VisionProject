using HalconDotNet;

namespace BingLibrary.Vision.Units
{
    public class DetectBarcode
    {
        private BarcodeType barcodeType;

        /// <summary>
        /// 选择扫码类型
        /// </summary>
        /// <param name="brdType"></param>
        public DetectBarcode(BarcodeType brdType)
        {
            barcodeType = brdType;
        }

        private HTuple dataCodeHandle = new HTuple();

        /// <summary>
        /// 创建条码模型
        /// </summary>
        /// <param name="path"></param>
        public void CreateBarcodeFile(string path)
        {
            if (barcodeType == BarcodeType.DM码)
            {
                HOperatorSet.CreateDataCode2dModel("Data Matrix ECC 200", new HTuple(), new HTuple(), out dataCodeHandle);
            }
            HOperatorSet.WriteDataCode2dModel(dataCodeHandle, path);
        }

        /// <summary>
        /// 读取条码模型
        /// </summary>
        /// <param name="path"></param>
        public void ReadBarcodeFile(string path)
        {
            HOperatorSet.ReadDataCode2dModel(path, out dataCodeHandle);
        }

        /// <summary>
        /// 保存条码模型
        /// </summary>
        /// <param name="path"></param>
        public void SaveBarcodeFile(string path)
        {
            HOperatorSet.WriteDataCode2dModel(dataCodeHandle, path);
        }

        /// <summary>
        /// 扫码
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public string GetBarcode(HImage image)
        {
            HTuple ResultHandles = new HTuple();
            HTuple DecodedDataStrings = new HTuple();
            HObject symbolXlds = new HObject();
            try
            {
                HOperatorSet.FindDataCode2d(image, out symbolXlds, dataCodeHandle, new HTuple(), new HTuple(), out ResultHandles, out DecodedDataStrings);
                return new HTuple((new HTuple(DecodedDataStrings.TupleLength())).TupleEqual(1)) == 1 ? DecodedDataStrings.TupleSelect(0).ToString() : "error";
            }
            catch
            {
                return "error";
            }
        }

        /// <summary>
        /// 扫码带训练
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public string GetBarcodeWithTrain(HImage image)
        {
            HTuple ResultHandles = new HTuple();
            HTuple DecodedDataStrings = new HTuple();
            HObject symbolXlds = new HObject();
            try
            {
                HOperatorSet.FindDataCode2d(image, out symbolXlds, dataCodeHandle, "train", "all", out ResultHandles, out DecodedDataStrings);
                return new HTuple((new HTuple(DecodedDataStrings.TupleLength())).TupleEqual(1)) == 1 ? DecodedDataStrings.TupleSelect(0).ToString() : "error";
            }
            catch
            {
                return "error";
            }
        }
    }
}