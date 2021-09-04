using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NPOI.SS.UserModel;
using System.Xml;

namespace MyExtension
{
    /// <summary>
    /// 擴充
    /// </summary>
    static public class Extension
    {
        /// <summary>
        /// NPOI - 取得指定型別的CellValue
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cell"></param>
        /// <returns>T CellValue</returns>
        static public T GetValue<T>(this ICell cell)
        {
            var type = typeof(T);
            dynamic temp;
            try
            {
                switch (cell.CellType)
                {
                    case CellType.Numeric:  // 數值格式
                        if (DateUtil.IsCellDateFormatted(cell))
                        {   // 日期格式=> 大於等於1：1900/1/1以後的日期；小於0：表示時間(乘上24)
                            if (cell.NumericCellValue >= 1)
                            {
                                temp = cell.DateCellValue;
                            }
                            else
                            {
                                temp = DateTime.MinValue.AddDays(cell.NumericCellValue);
                            }
                            break;
                        }
                        else
                        {   // 數值格式
                            temp = cell.NumericCellValue;
                            break;
                        }
                    case CellType.String:   // 字串格式
                        temp = cell.StringCellValue;
                        break;
                    case CellType.Formula:
                        IFormulaEvaluator iFormula = WorkbookFactory.CreateFormulaEvaluator(cell.Sheet.Workbook);
                        var formulaType = iFormula.Evaluate(cell).CellType;
                        if (formulaType == CellType.Numeric)
                        {
                            if (DateUtil.IsCellDateFormatted(cell))
                            {   // 日期格式=> 大於等於1：1900/1/1以後的日期；小於0：表示時間(乘上24)
                                if (cell.NumericCellValue >= 1)
                                {
                                    temp = cell.DateCellValue;
                                }
                                else
                                {
                                    temp = DateTime.MinValue.AddDays(cell.NumericCellValue);
                                }
                                break;
                            }
                            else
                            {   // 數值格式
                                temp = cell.NumericCellValue;
                                break;
                            }
                        }
                        else
                        {
                            temp = cell.StringCellValue;
                            break;
                        }
                    default:
                        temp = cell.ToString();
                        break;
                }

                if (type == typeof(string))
                {
                    temp = temp.ToString();
                }
                else if (type == typeof(DateTime))
                {
                    DateTime.TryParse(temp.ToString(), out DateTime dateTime);
                    temp = dateTime;
                }
                else if (type == typeof(double))
                {
                    double.TryParse(temp.ToString(), out double d);
                    temp = d;
                }
                else if (type == typeof(int) || type == typeof(long))
                {
                    if (temp.GetType() == typeof(double) || temp.ToString().Contains("."))
                    {
                        double.TryParse(temp.ToString(), out double d);
                        temp = Math.Round(d, 0, MidpointRounding.AwayFromZero);
                    }

                    if (type == typeof(int))
                    {
                        int.TryParse(temp.ToString(), out int i);
                        temp = i;
                    }
                    else if (type == typeof(long))
                    {
                        long.TryParse(temp.ToString(), out long i);
                        temp = i;
                    }
                }
            }
            catch(NullReferenceException)
            {
                // cell 為 null 的處理方式，傳回指定型態的預設值
                if (type == typeof(string))
                {
                    temp = (string)default;
                }
                else if (type == typeof(DateTime))
                {
                    temp = (DateTime)default;
                }
                else if (type == typeof(double))
                {
                    temp = (double)default;
                }
                else if (type == typeof(int))
                {
                    temp = (int)default;
                }
                else if(type == typeof(long))
                {
                    temp = (long)default;
                }
                else
                {
                    temp = default;
                }
            }

            return temp;
        }

        /// <summary>
        /// 將指定dataset轉為XML輸出在指定的路徑。預設路徑為程式根目錄
        /// </summary>
        /// <param name="dataset"></param>
        /// <param name="FilePath"></param>
        /// <param name="FileName"></param>
        static public void ToXML(this DataSet dataset, string FilePath = null, string FileName = null)
        {
            FilePath = FilePath ?? Environment.CurrentDirectory;
            FileName = FileName ?? "default.xml";

            StringWriter sw = new StringWriter();
            dataset.WriteXml(sw, XmlWriteMode.IgnoreSchema);
            string xml = sw.ToString();
            File.WriteAllText(Path.Combine(FilePath, FileName), xml);
        }

        /// <summary>
        /// 取得Full Stack Traces的訊息字串
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        static public string GetFullStackTracesString(this Exception e)
        {
            List<string> m = new List<string> { $"\"{e.Message}\"" };
            foreach (var item in (new StackTrace(e, true)).GetFrames())
            {
                if (item.GetMethod().Name != "Throw" && item.GetMethod().DeclaringType.Name != "ExceptionDispatchInfo")
                {
                    int line = item.GetFileLineNumber();
                    string method = $"{item.GetMethod().Name}({string.Join(",", item.GetMethod().GetParameters().Select(x => x.ParameterType.Name))})";
                    string namespace_ = item.GetMethod().DeclaringType.FullName;
                    string fileName = item.GetFileName()?.Split('\\').LastOrDefault() ?? "";
                    m.Add($" at {namespace_}.{method} in {fileName}:Line {line}");
                }
            }
            return string.Join("\n", m);
        }
    }
}
