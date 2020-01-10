using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlImportTool
{
    class ProjectSetting
    {
        string[] SourceSoftList = new string[] {"友信","自定义"};
        string[] TargetSoftList = new string[] {"TPlus标准版(12.2)"};

        string SourceSoft="";
        string TargetSoft="";

        //数据源信息
        string DateBaseType;

        //会计科目对照表
        string[] AccountComparedDCS;
        DataTable DTAccountCompared = new DataTable();

        //主表信息
        /* 1、会计科目表
         * 2、凭证主表
         * 3、凭证分录表
         * 
         * 
         * 
         */

        //附属档案信息
        /* 1、计量单位
         * 2、存货档案
         * 3、往来单位
         * 4、客户档案
         * 5、供应商档案
         * 6、部门档案
         * 7、职员档案
         * 
         * 
         * 
         * 
         */
        

    }
}
