using System.Collections.Generic;
using System.IO;
using System.Text;
using ExcelDataReader;

public class ExcelReader
{
    public static List<ExcelData> ReadExcel(string filePath)    //创建一个名为ReadExcel的函数，并返回一个结构体列表，表中元素都为ExcelData类型
    {
        List<ExcelData> excelData = new List<ExcelData>();     //创建一个新的空ExcelData结构体列表对象，实际就是一个列表，包含了多个参考ExcelData的空结构体。再把这个空对象赋值给excelData，excelData就成了包含多个参数都为空的空结构体列表。
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);      //支持中日文读取
        using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read))    //stream=（数据）流，一个 ​​FileStream对象​​，表示与文件连接的“数据流”。作用为​​逐部分读取或写入数据​​，高效处理大数据或实时数据，避免内存爆炸。流式读取文案，用.NET提供的File.Open()方法，读取地址,打开文件，只读文件
        {
            using (var reader = ExcelReaderFactory.CreateReader(stream))    //创建 阅读器    ExcelReaderFactory为包中的自带函数，用于读取数据流stream
            {
                do
                {
                    while (reader.Read())                         //如果阅读器读取到的内容为真           .Read()内置方法，读取本行
                    {
                        if (reader.IsDBNull(0) && reader.IsDBNull(1))
                            continue;
                        ExcelData data = new ExcelData();        //创建一个新的ExcelData空对象，赋值给临时变量data。    data为ExcelData结构体为参考的结构体对象，与ExcelData结构体只有参数上的不同，这里把空结构体赋值给data，相当于data成为了结构体，并且其中的参数（speaker和content）都为空。
                        data.speakerName = GetCellString(reader, 0);
                        data.speakingContent = GetCellString(reader, 1);
                        data.avatarImageFileName = GetCellString(reader, 2);
                        data.vocalAudioFileName = GetCellString(reader, 3);
                        data.backgroundImageFileName = GetCellString(reader, 4);
                        data.backgroundMusicFileName = GetCellString(reader, 5);
                        data.character1Action = GetCellString(reader, 6);
                        data.character1ImageFileName = GetCellString(reader, 7);
                        data.character2Action = GetCellString(reader, 8);
                        data.character2ImageFileName = GetCellString(reader, 9);
                        data.englishName = GetCellString(reader, 10);
                        data.englishContent = GetCellString(reader, 11);
                        data.japaneseName = GetCellString(reader, 12);
                        data.japaneseContent = GetCellString(reader, 13);
                        excelData.Add(data);                    //逐行载入文案        把读取到的结构体对象data添加到excelData结构体列表中
                    }
                } while (reader.NextResult());                  //若下一行有内容为真，则读取下一行，无内容结束循环。          NextResult() 为内置方法，跳转excel表格文案的下一行
            }
        }                                                       //得到一个结构体列表，内容物为文案行数个结构体
        return excelData;                                        //返回从Excel表格读取的结构体列表,内容即为文案
    }

    private static string GetCellString(IExcelDataReader reader,int index)
    {
        return reader.IsDBNull(index) ? string.Empty : reader.GetValue(index)?.ToString();
    }
}
