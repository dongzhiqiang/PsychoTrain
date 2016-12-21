using System;
using System.Collections.Generic;
using System.IO;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Linq;

public class Util{
     static string GetCopyFile(string sExcelFile)
    {
        int index = sExcelFile.LastIndexOf('.');
        if (index <= -1)
            return sExcelFile + "__copy__";

        return sExcelFile.Insert(index, "__copy__");
    }

    public static byte[] ReadFileByte(string file)
    {
        if (!File.Exists(file))
        {
            Console.WriteLine(file + " not find!");
            return null;
        }

        try
        {
            FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read);
            byte[] bytes = new byte[fs.Length];
            fs.Read(bytes, 0, (int)fs.Length);
            fs.Close();
            return bytes;
        }
        catch (Exception /*ex*/)
        {

        }

        string copy_file = GetCopyFile(file);
        while (File.Exists(copy_file))
            copy_file = GetCopyFile(copy_file);

        File.Copy(file, copy_file);
        byte[] data = ReadFileByte(copy_file);
        File.Delete(copy_file);
        return data;
    }

    
}
public class ExportCfg{
    public string clientPath;
    public string serverPath;
    public bool endPause;
}

public class XlsTableCfg{
    public ExportCfg cfg;
    public XlsCfg parent;
    public string name;
    public string clientPath;
    public string serverPath;

    // ��excel��ȡ������ԭʼ����
    public List<string> descs = new List<string>();//��һ������
    public List<string> fields = new List<string>();//�ڶ����ֶ�
    public List<List<string>> data = new List<List<string>>();
    public bool isRead = false;

    public XlsTableCfg(ExportCfg cfg,XlsCfg parent){
        this.parent = parent;
        this.cfg =cfg;
    }

    public void Read(ISheet sheet)
    {
        int fieldsCnt = 0;  //ͳһ������
        int emptyRowCnt = 0;//�����հ�����
        for (int j = 0; j <= sheet.LastRowNum; j++)
        {
            IRow row = sheet.GetRow(j);  //��ȡ��ǰ������
            if (row != null)
            {
                List<string> rowData;
                if (j == 0)
                    continue;
                else if (j == 1)
                    rowData = descs;//����
                else if (j == 2)
                    rowData = fields;//�ֶ���
                else
                {
                    rowData = new List<string>();
                    data.Add(rowData);
                }

                int lastColNum = fieldsCnt > 0 ? fieldsCnt + 1 : row.LastCellNum;
                bool allCellEmpty = true;
                for (int k = 0; k < lastColNum; k++)
                {
                    if(k == 0)//��һ�к���
                        continue;
                    ICell cell = row.LastCellNum < k ? null : row.GetCell(k);  //��ǰ���
                    string cellText = cell == null ? "" : cell.ToString();
                    if (string.IsNullOrEmpty(cellText))
                    {
                        rowData.Add("");                        
                    }
                    else
                    {
                        allCellEmpty = false;
                        rowData.Add(cellText);
                    }

                    //��ȡ�ڶ��У������У��������У��ֶ����У�ʱûȷ���������ж���������ȫ�հ׾Ͳ��ٶ�ȡ��
                    if (j == 1 || j == 2)
                    {
                        int curColCnt = rowData.Count;
                        if (curColCnt >= 3)
                        {
                            if (rowData[curColCnt - 1] == "" && rowData[curColCnt - 2] == "" && rowData[curColCnt - 3] == "")
                            {
                                //������ֻ�ο��ֶ�����
                                if (j == 2)
                                    fieldsCnt = rowData.Count - 3;
                                break;
                            }
                        }
                    }
                }

                //�����ȡ���ǵ����У��ֶ����У������û�����������пհף��Ǿ�ֱ��������ֶ����е�����
                if (j == 2 && fieldsCnt <= 0)
                    fieldsCnt = rowData.Count;

                //������������ǿ��У��Ǿ�ֱ�Ӳ�����������
                if (j > 2 && allCellEmpty)
                {
                    ++emptyRowCnt;
                    if (emptyRowCnt >= 3)
                    {
                        data.RemoveRange(data.Count - 3, 3);
                        break;
                    }                        
                }
                else
                {
                    emptyRowCnt = 0;
                }
            }
        }
        //���������С��ֶ����е�����Ϊͳһ������
        if (descs.Count > fieldsCnt)
            descs.RemoveRange(fieldsCnt, descs.Count - fieldsCnt);
        else if (descs.Count < fieldsCnt)
            descs.AddRange(Enumerable.Repeat("", fieldsCnt - descs.Count));
        if (fields.Count > fieldsCnt)
            fields.RemoveRange(fieldsCnt, fields.Count - fieldsCnt);
        else if (fields.Count < fieldsCnt)
            fields.AddRange(Enumerable.Repeat("", fieldsCnt - fields.Count));
        isRead = true;
    }

    public void WriteServer()
    {
        if (string.IsNullOrEmpty(serverPath) || !isRead)
            return;

        //����·��
        string path ;
        if (!cfg.serverPath.Contains(":"))//���·��
        {
            path = Directory.GetCurrentDirectory() + "/" + cfg.serverPath+"/" + serverPath;
        }
        else
        {
            path = cfg.serverPath + "/" + serverPath;
        }

        //�����ļ�
        if (File.Exists(path))
            File.Delete(path);
        else
            Directory.CreateDirectory(path.Substring(0, path.LastIndexOf('/')));//ȷ�������Ŀ¼

        //����
        File.WriteAllText(path, new CsvWriter().WriteAll(descs, fields, data));
    }
    public void WriteClient()
    {
        if (string.IsNullOrEmpty(clientPath) || !isRead)
            return;

        //����·��
        string path;
        if (!cfg.clientPath.Contains(":"))//���·��
        {
            path = Directory.GetCurrentDirectory() + "/" + cfg.clientPath + "/" + clientPath;
        }
        else
        {
            path = cfg.clientPath + "/" + clientPath;
        }

        //�����ļ�
        if (File.Exists(path))
            File.Delete(path);
        else
            Directory.CreateDirectory(path.Substring(0, path.LastIndexOf('/')));//ȷ�������Ŀ¼

        //����
        File.WriteAllText(path, new CsvWriter().WriteAll(descs, fields, data));
    }
}

public class XlsCfg{
    public ExportCfg cfg;
    public string file;
    public Dictionary<string,XlsTableCfg> tables = new Dictionary<string,XlsTableCfg>();

    public XlsCfg(ExportCfg cfg)
    {
        this.cfg = cfg;
    }
    public void Read()
    {
        string currentDirectory = Directory.GetCurrentDirectory();
        IWorkbook wk = null;
        string filePath = currentDirectory + "/���ñ�/" + this.file;
        string fileExt = Path.GetExtension(filePath).ToLower();
        MemoryStream stream = new MemoryStream(Util.ReadFileByte(filePath));
        if (fileExt == ".xls")
        {
            wk = new HSSFWorkbook(stream);
        }
        else if (fileExt == ".xlsx")
        {
            wk = new XSSFWorkbook(stream);
        }
        else
        {
            Console.WriteLine(filePath + " Error!");
            return;
        }

        for (int i = 0; i < wk.NumberOfSheets; i++)  //NumberOfSheets��myxls.xls���ܹ��ı���
        {
            ISheet sheet = wk.GetSheetAt(i);   //��ȡ��ǰ������

            XlsTableCfg tableCfg;
            if (!this.tables.TryGetValue(sheet.SheetName, out tableCfg))
                continue;
            tableCfg.Read(sheet);
        }
    }

    public void Write()
    {
        foreach (XlsTableCfg t in tables.Values)
        {
            if (!t.isRead)
            {
                Console.WriteLine("{0}��{1}û�б���ȡ", file,t.name);
                continue;
            }
            t.WriteServer();
            t.WriteClient();
        }

    }
}

public class ExportHandler: XMLHandlerReg
{
    public ExportCfg cfg = new ExportCfg();
    public List<XlsCfg> xlss = new List<XlsCfg>();

    XlsCfg Current;

    public ExportHandler()
    {
        RegElementStart("Config", OnElementStart_Config);
        RegElementEnd("Config", OnElementEnd_Config);

        RegElementStart("XlsFile", OnElementStart_XlsFile);
        RegElementEnd("XlsFile", OnElementEnd_XlsFile);

        RegElementStart("Table", OnElementStart_Table);
        RegElementEnd("Table", OnElementEnd_Table);        
    }

    public void OnElementStart_Config(string element, XMLAttributes attributes)
    {
        cfg.clientPath = attributes.getValue("clientPath");
        cfg.serverPath = attributes.getValue("serverPath");
        cfg.endPause = attributes.getValueAsBool("endPause", false);
    }

    public void OnElementEnd_Config(string element)
    {

    }

    public void OnElementStart_XlsFile(string element, XMLAttributes attributes)
    {
        Current = new XlsCfg(cfg);
        Current.file = attributes.getValue("file");
        xlss.Add(Current);
    }

    public void OnElementEnd_XlsFile(string element)
    {
        
    }

    public void OnElementStart_Table(string element, XMLAttributes attributes)
    {
        XlsTableCfg d = new XlsTableCfg(cfg,Current);
        d.name = attributes.getValue("name");
        d.clientPath = attributes.getValue("client");
        d.serverPath = attributes.getValue("server");

        Current.tables.Add(d.name, d);
    }

    public void OnElementEnd_Table(string element)
    {

    }
    
}