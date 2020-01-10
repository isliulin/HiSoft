using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace GlImportTool
{
    public class TPlusImportProject
    {
        public DataTable DTAccountSource = new DataTable();
        public DataTable DTAccountTarget = new DataTable();
        public DataTable DTAccountCompared = new DataTable();

        //T+会计科目模板信息
        public string[] TarAccDTColName = new string[] { "填制凭证时录入结算信息", "期末调汇", "科目编码", "科目名称", "助记码", "科目类型编码", "科目类型", "停用", "账页格式", "余额方向", "现金科目", "银行科目", "现金等价物科目", "汇总打印", "汇总打印科目编码", "汇总打印科目", "数量核算", "计量单位", "外币核算", "默认币种编码", "默认币种", "受控", "业务系统", "受控科目可手工制单", "部门", "个人", "往来单位", "存货", "项目" };
        public string[] TarAccDTColType = new string[] { "Char", "Char", "String  ", "String", "String", "String", "String", "Char", "String", "String", "Char", "Char", "Char", "Char", "String", "String", "Char", "String", "Char", "String", "String", "Char", "String", "Char", "Char", "Char", "Char", "Char", "Char" };

        public string[] TargetAccountCompared = new string[] { "科目编码", "科目名称" };


        //T+会计科目编码规则
        public string[] SAccountCodingGuidel;

        //T+会计制度
        public string AccountingSystem = "2013年小企业会计准则";

        //计量单位信息
        public DataTable JLInfoO = new DataTable();
        public DataTable JLInfoN = new DataTable();



        //-----------------------------------------------------------------------------
        
        //-------------------------------------------------------------------------------


        //初始化会计科目模板表
        public void InitialAccountTarget()
        {
            //创建T+会计科目模板
            for (int i = 0; i < TarAccDTColName.Count(); i++)
            {
                string CType = "System." + TarAccDTColType[i];
                //Console.WriteLine(TarAccDTColName[i] + " | " + CType);
                DTAccountTarget.Columns.Add(TarAccDTColName[i], System.Type.GetType(CType));
            }

            //获取会计科目数据
            foreach (DataRow ASR in DTAccountSource.Rows)
            {
                DataRow NewAccTarRow = DTAccountTarget.NewRow();
                //历遍列
                for (int i = 0; i < DTAccountSource.Columns.Count; i++)
                {
                    //获取当前列名称
                    string rowname = ASR.Table.Columns[i].ColumnName.ToString();
                    //如果列在模板中且列值不为空
                    if ((Array.IndexOf(TarAccDTColName, rowname) > -1) && (!String.IsNullOrEmpty(ASR[rowname].ToString())))
                    {
                        //根据模板列数据类型处理数据
                        switch (NewAccTarRow.Table.Columns[rowname].DataType.ToString())
                        {
                            case "System.Char":
                                //布尔型
                                bool check = bool.Parse(ASR[rowname].ToString());
                                if (check)
                                {
                                    NewAccTarRow[rowname] = '1';
                                }
                                break;
                            case "System.String":
                                //字符串
                                NewAccTarRow[rowname] = ASR[rowname].ToString();
                                break;
                            default:
                                NewAccTarRow[rowname] = ASR[rowname];
                                break;
                        }
                    }
                }
                DTAccountTarget.Rows.Add(NewAccTarRow);
            }
        }

        //会计科目模板表信息校验
        public void CheckAccountTarget()
        {
            for (int i = 0; i < DTAccountTarget.Rows.Count; i++)
            {
                //获取科目类型
                string[] AccType = GetAccountType(DTAccountTarget.Rows[i]["科目编码"]);
                DTAccountTarget.Rows[i]["科目类型编码"] = AccType[0];
                DTAccountTarget.Rows[i]["科目类型"] = AccType[1];

                //获取账页格式

                //余额方向
                //需判断上级科目余额方向信息及同级其他科目余额方向信息
                //DTAccountTarget.Rows[i]["余额方向"] = AccType[0];

                //计量单位


                //默认币种
                if (String.IsNullOrEmpty(DTAccountTarget.Rows[i]["外币核算"].ToString()))
                {
                    DTAccountTarget.Rows[i]["默认币种编码"] = "";
                    DTAccountTarget.Rows[i]["默认币种"] = "";
                }
                else
                {
                    DTAccountTarget.Rows[i]["默认币种编码"] = "RMB";
                    DTAccountTarget.Rows[i]["默认币种"] = "人民币";
                }
            }
        }

        //对照表变更


        //获取科目类型
        string[] GetAccountType(object AccIDs)
        {
            string AccID = AccIDs.ToString();
            string[] AccType = new string[2];
            char IDT = Char.Parse(AccID.Substring(0, 1));
            switch (AccountingSystem)
            {
                case "2013年小企业会计准则":
                    switch (IDT)
                    {
                        case '1':
                            //资产
                            AccType[0] = "1";
                            AccType[1] = "资产";
                            return AccType;
                            break;
                        case '2':
                            //负债
                            AccType[0] = "2";
                            AccType[1] = "负债";
                            return AccType;
                            break;
                        case '3':
                            //权益
                            AccType[0] = "3";
                            AccType[1] = "权益";
                            return AccType;
                            break;
                        case '4':
                            //成本
                            AccType[0] = "4";
                            AccType[1] = "成本";
                            return AccType;
                            break;
                        case '5':
                            //损益
                            AccType[0] = "5";
                            AccType[1] = "损益";
                            return AccType;
                            break;
                        default:
                            return AccType;
                            break;
                    }
                    break;
                //
                case "07新准则":
                    return AccType;
                    break;
                default:
                    return AccType;
                    break;
            }
        }

        //   
    }
}
