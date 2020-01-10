using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.OleDb;
using System.Reflection;
using System.Text.RegularExpressions;
using System.IO;
using GlImportTool;

namespace GlImportTool
{
    public partial class YXtoYonyou : Form
    {
        AppMain MainForm;


        string DBPath;
        string DBPWD;
        ReadData.OleDBHelper ODHAccess = new ReadData.OleDBHelper();

        OleDbConnection conn;   //数据库连接
        DataTable SouAccDT = new DataTable();
        DataTable TarAccDT = new DataTable();
        int MaxAccLeveal = 0;
        int[,,] GLAccLevelInfo;

        //------------------------------------------  0   ------  1  ------   2   -------------  3  --------------  4   ------  5   ------  6   -----  7   ------  8   ---------  9   ----  10   ---  11   -----  12   -----  13   -----  14   --------  15   --------  16   ---------  17   ----------  18   --------  19   -----  20  -----  21   -------  22   --------  23  ----  24  ---  25  -----------  26  -----------  27  -----  28  ----  29  ----  30  -----  31  ---  32 ----
        string[] SouAccDTColName = new string[] { "级次    ", "科目编码", "科目名称", "                      ", "        ", "        ", "        ", "简码  ", "            ", "        ", "    ", "        ", "余额方向", "        ", "        ", "              ", "        ", "                ", "            ", "数量核算", "计量单位", "外币核算", "币种        ", "        ", "    ", "        ", "                  ", "部门核算", "    ", "        ", "    ", "项目核算","id    " };
        string[] TarAccDTColName = new string[] { "科目级次", "科目旧码", "科目旧称", "填制凭证时录入结算信息", "期末调汇", "科目编码", "科目名称", "助记码", "科目类型编码", "科目类型", "停用", "账页格式", "余额方向", "现金科目", "银行科目", "现金等价物科目", "汇总打印", "汇总打印科目编码", "汇总打印科目", "数量核算", "计量单位", "外币核算", "默认币种编码", "默认币种", "受控", "业务系统", "受控科目可手工制单", "部门    ", "个人", "往来单位", "存货", "项目    ","旧id  " };
        string[] TarAccDTColType = new string[] { "Int32   ", "String  ", "String  ", "Char                  ", "Char    ", "String  ", "String  ", "String", "String      ", "String  ", "Char", "String  ", "String  ", "Char    ", "Char    ", "Char          ", "Char    ", "String          ", "String      ", "Char    ", "String  ", "Char    ", "String      ", "String  ", "Char", "String  ", "Char              ", "Char    ", "Char", "Char    ", "Char", "Char    ","String" };
//        string[] TarDefaultValue = new string[] { "        ", "        ", "        ", "0                     ", "0       ", "        ", "        ", "      ", "            ", "        ", "0   ", "金额式  ", "        ", "0       ", "0       ", "0             ", "0       ", "                ", "            ", "0       ", "        ", "0       ", "            ", "        ", "0   ", "        ", "                  ", "0       ", "0   ", "0       ", "0   ", "0       " };

        int[] SpecialHanding = new int[] { 0, 1, 2,  5, 6, 7, 8, 9,11,12,20,22 };
        int[] WaittingHanding = new int[] { 11 };
        //填制凭证时录入结算信息	期末调汇 科目编码 科目名称	助记码	科目类型编码	科目类型	停用	账页格式	余额方向	现金科目	银行科目	现金等价物科目	汇总打印	汇总打印科目编码	汇总打印科目	数量核算	计量单位	外币核算	默认币种编码	默认币种 	受控	业务系统	受控科目可手工制单	部门	个人	往来单位	存货	项目
        string[] ExcelSkepName = new string[] { "科目级次", "科目旧码", "科目旧称", "旧id" };

        public YXtoYonyou(AppMain ParentForm)
        {
            InitializeComponent();
            MainForm = ParentForm;
            //BTNLoadGLAcc.Enabled = false;
            //BTNRebuildAcc.Enabled = false;
            //BTNSaveAcc.Enabled = true;
            BTNImportVouch.Enabled = true;
        }

        //表格信息

        string TBInfo(int D, int ID)
        {
            switch (D)
            {
                case 1:
                    return SouAccDTColName[ID].ToString().Replace(" ", "");
                    break;
                case 2:
                    return TarAccDTColName[ID].ToString().Replace(" ", "");
                    break;
                case 3:
                    return TarAccDTColType[ID].ToString().Replace(" ", "");
                    break;
                default:
                    return "";
                    break;
            }
            /*
            if (D == 1)
            {
                return SouAccDTColName[ID].ToString().Replace(" ", "");
            }
            else if (D == 2)
            {
                return TarAccDTColName[ID].ToString().Replace(" ", "");
            }
            else if (D == 3)
            {
                return TarAccDTColType[ID].ToString().Replace(" ", "");
            }
            
            else if (D == 4)
            {
                return TarDefaultValue[ID].ToString().Replace(" ", "");
            }
           
            else
            {
                return "";
            }
            */
        }


        //数据库连接
        void InitializeConn(string MDBPath)
        {
            
            ODHAccess.dbPath = MDBPath;
            ODHAccess.PWD = "222555";
            ODHAccess.InitialConn("Access");

            //conn = new OleDbConnection(ConnStr);
            conn = ODHAccess.conn;
            //conn.Open();
            if (conn.State == ConnectionState.Open)
            {
                this.LMessage.Text = "Access数据库的连接成功! "+ MDBPath;
                DBPath = MDBPath;
                DBPWD = "222555";
            }
            else
            {
                this.LMessage.Text = "Access数据库的连接失败! " + MDBPath;
            }
            //conn.Close();
        }

        //打开并连接数据库
        private void BTNOpenMDB_Click(object sender, EventArgs e)
        {
            try
            {

                OpenFileDialog GetXlsPath = new OpenFileDialog();
                GetXlsPath.Multiselect = false;
                GetXlsPath.Filter = "Access(*.mdb)|*.mdb";
                GetXlsPath.Title = "选择源友信数据库文件";

                if (GetXlsPath.ShowDialog() == DialogResult.OK)
                {
                    this.TBMDBPath.Text = GetXlsPath.FileName;
                    InitializeConn(this.TBMDBPath.Text);
                }
                BTNLoadGLAcc.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "FileDialogError", MessageBoxButtons.OK);
            }
        }

        //加载科目档案表
        private void BTNLoadGLAcc_Click(object sender, EventArgs e)
        {
            LoadAccInfoTB();
        }

        void LoadAccInfoTB()
        {
            //获取科目档案表
            string Sql1 = "select * from [科目] order by [科目编码]";
            OleDbDataAdapter ODDA = new OleDbDataAdapter(Sql1, conn);
            ODDA.Fill(SouAccDT);

            this.DGVShow.DataSource = SouAccDT;

            //获取科目最大级数
            MaxAccLeveal = SouAccDT.AsEnumerable().Select(t => int.Parse(t["级次"].ToString())).Max();

            string GLAccLevelMsg = "科目级次：" + MaxAccLeveal + " ;级次规则:";
            string GLAccLevelMsg2 = "  ";

            //获取级次规则
            GLAccLevelInfo = new int[2, 2, MaxAccLeveal + 1];
            //00i总长，最极，级次
            GLAccLevelInfo[0, 0, 0] = 0;
            GLAccLevelInfo[0, 1, 0] = 0;

            for (int i = 1; i <= MaxAccLeveal; i++)
            {
                //获取科目编码长度

                //获取本级科目编码的最大值
                var MaxAccLevelNo = SouAccDT.Select("级次=" + i.ToString()).AsEnumerable().Select(t => long.Parse(Regex.Replace(t["科目编码"].ToString(), @"[^\d]*", ""))).Max().ToString();
                var MinAccLevelNo = SouAccDT.Select("级次=" + i.ToString()).AsEnumerable().Select(t => long.Parse(Regex.Replace(t["科目编码"].ToString(), @"[^\d]*", ""))).Min().ToString();
                //Regex.Replace( key,@"[^\d]*", "");  //剔除字符串中非数字的部分

                //编码总长
                GLAccLevelInfo[0, 0, i] = MinAccLevelNo.ToString().Length; //最小总长
                GLAccLevelInfo[0, 1, i] = MaxAccLevelNo.ToString().Length; //最大总长

                //编码级长
                //应修改为 本级最大长度-上级最小长度总和
                GLAccLevelInfo[1, 1, i] = GLAccLevelInfo[0, 1, i] - GLAccLevelInfo[0, 0, i - 1];   //最大级长=本级最大长度-上级最小长度

                GLAccLevelMsg += GLAccLevelInfo[1, 1, i];
                if (i != MaxAccLeveal)
                {
                    GLAccLevelMsg += "-";
                }
                GLAccLevelMsg2 += "(" + MaxAccLevelNo + ")";
            }
            GLAccLevelMsg += "   " + GLAccLevelMsg2;
            this.LMessage.Text = GLAccLevelMsg;
            BTNRebuildAcc.Enabled = true;
        }

        //重建科目档案
        private void BTNRebuildAcc_Click(object sender, EventArgs e)
        {
            RebuildAccTB();
        }

        //初始化新科目档案表结构
        void InitialTarAccDT(DataTable DT)
        {
            for (int i = 0; i < TarAccDTColName.Count(); i++)
            {
                string CType = "System." + TBInfo(3, i);
                Console.WriteLine(TBInfo(2, i) + " | " + CType);
                DT.Columns.Add(TBInfo(2, i), System.Type.GetType(CType));
            }
        }

        void RebuildAccTB()
        {
            //初始化新科目档案表结构
            InitialTarAccDT(TarAccDT);

            foreach (DataRow SouRow in SouAccDT.Rows)
            {
                DataRow TarRow = TarAccDT.NewRow();

                CopyAccInfo("友信", SouRow, TarRow, GLAccLevelInfo[1, 1, int.Parse(SouRow[TBInfo(1, 0)].ToString())]);

                TarAccDT.Rows.Add(TarRow);        
            }
            this.DGVShow.DataSource = null;
            this.DGVShow.DataSource = TarAccDT;
            MessageBox.Show("科目档案加载完成");
            BTNSaveAcc.Enabled = true;
            BTNImportVouch.Enabled = true;
        }


        //保存科目档案
        private void BTNSaveAcc_Click(object sender, EventArgs e)
        {
            SaveFileDialog SFDSaveAccInfo = new SaveFileDialog();
            SFDSaveAccInfo.Filter = "TPlus 12.2模板文件(*.xls)|*.xls";
            SFDSaveAccInfo.Title = "保存T+科目档案导入模板 12.2";
            if (SFDSaveAccInfo.ShowDialog() == DialogResult.OK)
            {
                string SaveFileName = @SFDSaveAccInfo.FileName;
                ReadData.FileHelper FH = new ReadData.FileHelper();

                string ResFile = Assembly.GetExecutingAssembly().GetName().Name.ToString() + ".Resources.TPLUSACCINFO.xls";
                Assembly app = Assembly.GetExecutingAssembly();
                Stream SouFS = app.GetManifestResourceStream(ResFile);

                if (FH.SaveResourceFile(@SaveFileName, SouFS).IndexOf("OK") > -1)
                {
                    //SaveAcc(TarAccDT, FH.SRFSavePath, TarAccDTColName, ExcelSkepName);
                    ReadData.ExcelHelper EH = new ReadData.ExcelHelper();
                    EH.Path = SaveFileName;
                    EH.InitialExcelHelper();
                    string OutMSG = EH.DTImportToExcel(TarAccDT, "", TarAccDTColName, ExcelSkepName);
                    MessageBox.Show(OutMSG);
                }
            }

            
        }


        /*************************************************************************************************************/

        //附属档案生成
        void CopyAccInfo(string ProPlan, DataRow SouRow, DataRow TarRow, int NLevelLength)
        {
            //历遍科目档案条目并生成信息
            for (int i = 0; i < TarAccDTColName.Count(); i++)
            {
                //判断是否需要特殊处理
                if (Array.IndexOf(SpecialHanding, i) > -1)
                {
                    //特殊处理
                    switch (ProPlan)
                    {
                        case "友信":
                            YXAccSpecialHandle(i, SouRow, TarRow, NLevelLength);
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    //标准处理
                    CopyStandInfo(ProPlan, i, SouRow, TarRow);
                }
            }

            //延时处理条目
            for (int i = 0; i < WaittingHanding.Count(); i++)
            {
                switch (ProPlan)
                {
                    case "友信":
                        YXAccWaittingHandle(WaittingHanding[i], SouRow, TarRow, NLevelLength);
                        break;
                    default:
                        break;
                }
            }
        }

        //特殊处理
        //友信特殊处理
        void YXAccSpecialHandle(int InfoID, DataRow SouRow, DataRow TarRow, int NLevelLength)
        {
            switch (InfoID)
            {
                case 0:
                    //科目级次
                    TarRow[TBInfo(2, InfoID)] = int.Parse(SouRow[TBInfo(1, 0)].ToString());
                    break;
                case 1:
                    //科目旧码
                    TarRow[TBInfo(2, InfoID)] = SouRow[TBInfo(1, 1)].ToString();
                    break;
                case 2:
                    //科目旧名
                    TarRow[TBInfo(2, InfoID)] = SouRow[TBInfo(1, 2)].ToString();
                    break;
                case 5:
                    //
                    GetYXAccIDandName("友信", SouRow, TarRow, NLevelLength);
                    break;
                case 6:
                    //
                    break;
                case 7:
                    //拼音码
                    TarRow[TBInfo(2, InfoID)] = GetSpellCode(TarRow[TBInfo(2, InfoID-1)].ToString());
                    break;
                case 8:
                    //科目类型编码 //T+要求
                    string ID = TarRow[TBInfo(2, InfoID-3)].ToString();
                    char ATIDH = Char.Parse(ID.Substring(0, 1));
                    switch (ATIDH)
                    {
                        case '1':
                            //资产
                            TarRow[TBInfo(2, InfoID)] = "1";
                            TarRow[TBInfo(2, InfoID+1)] = "资产";
                            break;
                        case '2':
                            //负债
                            TarRow[TBInfo(2, InfoID)] = "2";
                            TarRow[TBInfo(2, InfoID+1)] = "负债";
                            break;
                        case '3':
                            //权益
                            TarRow[TBInfo(2, InfoID)] = "3";
                            TarRow[TBInfo(2, InfoID+1)] = "权益";
                            break;
                        case '4':
                            //成本
                            TarRow[TBInfo(2, InfoID)] = "4";
                            TarRow[TBInfo(2, InfoID+1)] = "成本";
                            break;
                        case '5':
                            //损益
                            TarRow[TBInfo(2, InfoID)] = "5";
                            TarRow[TBInfo(2, InfoID+1)] = "损益";
                            break;
                        case '6':
                            break;
                        default:
                            break;
                    }

                    break;
                case 9:
                    //
                    break;
                case 11:
                    //账页格式 延时处理
                    //TarRow[TBInfo(2, 9)] = "金额式";
                    break;
                case 12:
                    //余额方向
                    if (SouRow[TBInfo(1, InfoID)].ToString().IndexOf("借") > -1)
                    {
                        TarRow[TBInfo(2, InfoID)] = "借方";
                    }
                    else if (SouRow[TBInfo(1, InfoID)].ToString().IndexOf("贷") > -1)
                    {
                        TarRow[TBInfo(2, InfoID)] = "贷方";
                    }
                    else
                    {
                        TarRow[TBInfo(2, InfoID)] = "";
                    }
                    break;
                case 20:
                    //T+要求 数量核算，计量单位不能为空
                    if (string.IsNullOrEmpty(SouRow[TBInfo(1, InfoID)].ToString()))
                    {
                        TarRow[TBInfo(2, InfoID)] = "单位";
                    }
                    else
                    {
                        TarRow[TBInfo(2, InfoID)] = SouRow[TBInfo(1, InfoID)].ToString();
                    }
                    break;
                case 22:
                    //T+要求 非外币核算，默认币种应为空
                    if (TarRow[TBInfo(2, InfoID-1)].ToString() == "1")
                    {
                        TarRow[TBInfo(2, InfoID)] = "RMB";
                    }
                    break;
                default:
                    CopyStandInfo("友信", InfoID, SouRow, TarRow);
                    break;
            }
        }

        //友信延时处理
        void YXAccWaittingHandle(int InfoID, DataRow SouRow, DataRow TarRow, int NLevelLength)
        {
            switch (InfoID)
            {
                case 11:
                    string StateInfo = TarRow[TBInfo(2, 19)].ToString() + TarRow[TBInfo(2, 21)].ToString();
                    StateInfo = StateInfo.Replace(" ", "");
                    switch (StateInfo)
                    {
                        case "11":
                            TarRow[TBInfo(2, InfoID)] = "外币数量式";
                            break;
                        case "10":
                            TarRow[TBInfo(2, InfoID)] = "数量金额式";
                            break;
                        case "01":
                            TarRow[TBInfo(2, InfoID)] = "外币金额式";
                            break;
                        default:
                            TarRow[TBInfo(2, InfoID)] = "金额式";
                            break;
                    }                   
                    break;
                default:
                    break;
            }
        }

        //标准复制
        void CopyStandInfo(string ProPlan, int InfoID, DataRow SouRow, DataRow TarRow)
        {
            if (ProPlan == "友信")
            {
                if (!String.IsNullOrEmpty(TBInfo(1, InfoID)))
                {
                    switch (TBInfo(3, InfoID))
                    {
                        case "String":
                            TarRow[TBInfo(2, InfoID)] = SouRow[TBInfo(1, InfoID)].ToString();
                            break;
                        case "Char":
                            bool check = bool.Parse(SouRow[TBInfo(1, InfoID)].ToString());
                            if (check)
                            {
                                TarRow[TBInfo(2, InfoID)] = '1';
                            }
                            else
                            {
                                TarRow[TBInfo(2, InfoID)] = '0';
                            }
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        //获取 新 科目编码
        //获取 新 科目名称
        void GetYXAccIDandName(string ProPlan, DataRow SouRow, DataRow TarRow, int NLevelLength)
        {
            switch (ProPlan)
            {
                case "友信":
                    //友信处理开始
                    if (int.Parse(SouRow[TBInfo(1, 0)].ToString()) == 1)
                    {
                        TarRow[TBInfo(2, 5)] = TarRow[TBInfo(2, 1)];
                        TarRow[TBInfo(2, 6)] = TarRow[TBInfo(2, 2)];
                        //Console.WriteLine("    " + TarRow[TBInfo(2, 4)].ToString().PadRight(30, ' '));
                    }
                    else
                    {
                        //科目名称
                        string AccName = PrintAccName(SouRow[TBInfo(1, 2)].ToString(), int.Parse(SouRow[TBInfo(1, 0)].ToString()));
                        //Console.Write("  " + AccName.PadRight(30, ' ') + "   |    ");

                        //上级科目名称
                        string upAccName = SouRow[TBInfo(1, 2)].ToString().Replace("-" + AccName, "").Replace('－' + AccName, "");
                        //错别字害死人啊
                        upAccName = ReNameAccName(upAccName, int.Parse(SouRow[TBInfo(1, 0)].ToString()), TarRow[TBInfo(2, 1)].ToString());
                        //Console.WriteLine("  " + upAccName.PadRight(20, ' ') + "   |    ");

                        //原始上级科目编码
                        string OldUpAcc = SouAccDT.Select(TBInfo(1, 2) + " like '" + upAccName + "'").AsEnumerable().Select(t => t[TBInfo(1, 1)].ToString()).Max();

                        //当前上级科目编码  
                        string NewUpAcc = TarAccDT.Select(TBInfo(2, 2) + " like '" + upAccName + "'").AsEnumerable().Select(t => t[TBInfo(2, 5)].ToString()).Max();

                        //科目ID
                        string TempID = Regex.Replace(SouRow[TBInfo(1, 1)].ToString().Replace(OldUpAcc, ""), @"[^\d]*", "");
                        TempID = FillID(TempID, NLevelLength);

                        TarRow[TBInfo(2, 5)] = NewUpAcc + TempID;
                        TarRow[TBInfo(2, 6)] = AccName.Replace("应交税费", "应交税金").Replace("帐", "账");
                    }
                    //友信处理结束
                    break;

                default:
                    break;
            }
        }

        //科目名称校验
        string PrintAccName(string OldName, int level)
        {
            string[] FullNameList = OldName.ToString().Split('-', '－');
            string AccName = "";
            if (FullNameList.Count() == level)
            {
                AccName = FullNameList[FullNameList.Count() - 1];
            }
            else
            {
                string TempAccName = "";
                for (int i = level - 1; i < FullNameList.Count(); i++)
                {
                    TempAccName += FullNameList[i];
                    if (i != FullNameList.Count() - 1)
                    {
                        TempAccName += "-";
                    }
                }
                AccName = TempAccName;
            }
            return AccName;
        }

        //科目编码补全
        string FillID(string ID, int CheckLength)
        {
            string k = ID;
            while (k.Length < CheckLength)
            {
                k = "0" + k;
            }
            return k;
        }

        //拼音码生成
        public static string GetSpellCode(string CnStr)
        {
            string strTemp = "";
            int iLen = CnStr.Length;
            int i = 0;
            for (i = 0; i <= iLen - 1; i++)
            {
                strTemp += GetCharSpellCode(CnStr.Substring(i, 1));
            }
            return strTemp;
        }
        private static string GetCharSpellCode(string CnChar)
        {
            long iCnChar;
            byte[] ZW = System.Text.Encoding.Default.GetBytes(CnChar);
            //如果是字母，则直接返回
            if (ZW.Length == 1)
            {
                return CnChar.ToUpper();
            }
            else
            {
                // get the array of byte from the single char
                int i1 = (short)(ZW[0]);
                int i2 = (short)(ZW[1]);
                iCnChar = i1 * 256 + i2;
            }
            // iCnChar match the constant
            if ((iCnChar >= 45217) && (iCnChar <= 45252))
            {
                return "A";
            }
            else if ((iCnChar >= 45253) && (iCnChar <= 45760))
            {
                return "B";
            }
            else if ((iCnChar >= 45761) && (iCnChar <= 46317))
            {
                return "C";
            }
            else if ((iCnChar >= 46318) && (iCnChar <= 46825))
            {
                return "D";
            }
            else if ((iCnChar >= 46826) && (iCnChar <= 47009))
            {
                return "E";
            }
            else if ((iCnChar >= 47010) && (iCnChar <= 47296))
            {
                return "F";
            }
            else if ((iCnChar >= 47297) && (iCnChar <= 47613))
            {
                return "G";
            }
            else if ((iCnChar >= 47614) && (iCnChar <= 48118))
            {
                return "H";
            }
            else if ((iCnChar >= 48119) && (iCnChar <= 49061))
            {
                return "J";
            }
            else if ((iCnChar >= 49062) && (iCnChar <= 49323))
            {
                return "K";
            }
            else if ((iCnChar >= 49324) && (iCnChar <= 49895))
            {
                return "L";
            }
            else if ((iCnChar >= 49896) && (iCnChar <= 50370))
            {
                return "M";
            }
            else if ((iCnChar >= 50371) && (iCnChar <= 50613))
            {
                return "N";
            }
            else if ((iCnChar >= 50614) && (iCnChar <= 50621))
            {
                return "O";
            }
            else if ((iCnChar >= 50622) && (iCnChar <= 50905))
            {
                return "P";
            }
            else if ((iCnChar >= 50906) && (iCnChar <= 51386))
            {
                return "Q";
            }
            else if ((iCnChar >= 51387) && (iCnChar <= 51445))
            {
                return "R";
            }
            else if ((iCnChar >= 51446) && (iCnChar <= 52217))
            {
                return "S";
            }
            else if ((iCnChar >= 52218) && (iCnChar <= 52697))
            {
                return "T";
            }
            else if ((iCnChar >= 52698) && (iCnChar <= 52979))
            {
                return "W";
            }
            else if ((iCnChar >= 52980) && (iCnChar <= 53640))
            {
                return "X";
            }
            else if ((iCnChar >= 53689) && (iCnChar <= 54480))
            {
                return "Y";
            }
            else if ((iCnChar >= 54481) && (iCnChar <= 55289))
            {
                return "Z";
            }
            else
                return ("");
        }

        //错别字处理
        string ReNameAccName(string AccNameIn, int level, string OldID)
        {
            string AccName = AccNameIn;
            AccName = AccName.Replace("应付账", "应付帐");
            if (level == 2)
            {
                AccName = AccName.Replace("应交税金", "应交税费");
            }
            if (OldID.ToString() == "2171011001" || OldID.ToString() == "2171011002")
            {
                AccName = AccName.Replace("应交税费", "应交税金");
            }
            return AccName;
        }

        private void BTNImportVouch_Click(object sender, EventArgs e)
        {
            //MessageBox.Show(@DBPath);
            //conn.Dispose();
            //conn.Close();
            Form YXImp = new YXImportVouch(TarAccDT,ODHAccess);
            YXImp.Show();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
            MainForm.OpenForm(this,new Tplus.TPlusImportHandler(MainForm,this, new Tplus.TPlusImportData()), MainForm);
            
        }

        private void BTNTImpHand_Click(object sender, EventArgs e)
        {
            Tplus.TPlusImportData TPID = new Tplus.TPlusImportData();
            DataTable AccYMDT = ODHAccess.ReadDT("select [凭证年月] as 会计期间 from [凭证] group by [凭证年月]");
            AccYMDT.Columns.Add("期间日期", Type.GetType("System.String"));


            TPID.AccYearMonth = AccYMDT;
          

            MainForm.OpenForm(this, new Tplus.TPlusImportHandler(MainForm, this,TPID), MainForm);
        }
    }
}
