/*
 * ========================================================================
 * Copyright(c) 2006-2010 PWMIS, All Rights Reserved.
 * Welcom use the PDF.NET (PWMIS Data Process Framework).
 * See more information,Please goto http://www.pwmis.com/sqlmap 
 * ========================================================================
 * ���������
 * 
 * ���ߣ���̫��     ʱ�䣺2008-10-12
 * �汾��V3.0
 * 
 * �޸��ߣ�         ʱ�䣺                
 * �޸�˵����
 * ========================================================================
*/

using System;
using System.ComponentModel;
using System.Drawing;
using System.Web.UI.WebControls;
using PWMIS.Common;

namespace PWMIS.Web.Controls
{
    /// <summary>
    ///     �����б�ؼ�
    /// </summary>
    [ToolboxBitmap(typeof (ControlIcon), "DataListBox.bmp")]
    public class DataListBox : ListBox, IDataControl, IQueryControl
    {
        #region IBrainControl ��Ա

        #region ��������

        [Category("Data"), Description("�趨��Ӧ������Դ����ʽ��FullClassName,AssemblyName �������Ҫ��ʵ���࣬�������ø����ԡ�")]
        public string DataProvider { get; set; }

        [Category("Data"), Description("�趨��Ӧ�����ݿ��ֶ��Ƿ��������������Զ����ݲ�ѯ�͸��µ�����")]
        public bool PrimaryKey
        {
            get
            {
                if (ViewState["PrimaryKey"] != null)
                    return (bool) ViewState["PrimaryKey"];
                return false;
            }
            set { ViewState["PrimaryKey"] = value; }
        }

        [Category("Data"), Description("�趨��Ӧ�������ֶ�����")]
        public TypeCode SysTypeCode
        {
            get
            {
                if (ViewState["SysTypeCode"] != null)
                    return (TypeCode) ViewState["SysTypeCode"];
                return new TypeCode();
            }
            set { ViewState["SysTypeCode"] = value; }
        }

        [Category("Data"), Description("�趨�����ݿ��ֶζ�Ӧ��������")]
        public string LinkProperty
        {
            get
            {
                if (ViewState["LinkProperty"] != null)
                    return (string) ViewState["LinkProperty"];
                return "";
            }
            set { ViewState["LinkProperty"] = value; }
        }

        [Category("Data"), Description("�趨�����ݱ��Ӧ�����ݱ���")]
        public string LinkObject
        {
            get
            {
                if (ViewState["LinkObject"] != null)
                    return (string) ViewState["LinkObject"];
                return "";
            }
            set { ViewState["LinkObject"] = value; }
        }

        #endregion

        #region Ĭ������

        public bool ReadOnly
        {
            get
            {
                //δѡ������Ϊֻ���������ԣ������������ݿ⡣dth,2008.7.27
                if (SelectedIndex == -1)
                    return true;
                return !Enabled;
            }
            set { Enabled = !value; }
        }

        #endregion

        #region �ӿڷ���

        public void SetValue(object obj)
        {
            if (obj == null) return;
            foreach (ListItem Item in Items)
            {
                Item.Selected = false;
            }
            var SelItemValues = "";
            //if(obj!=null)
            SelItemValues = obj.ToString().Trim();
            //string delimStr = ",";
            //char [] delimiter = delimStr.ToCharArray();

            var SelItemobj = SelItemValues.Split(','); // SelItemValues.Split(delimiter);

            foreach (var s in SelItemobj)
            {
                var item = Items.FindByValue(s);
                item.Selected = true;
            }
        }

        public object GetValue()
        {
            var SelItemValues = "";

            foreach (ListItem Item in Items)
            {
                if (Item.Selected)
                {
                    if (SelItemValues == string.Empty)
                    {
                        SelItemValues += Item.Value.Trim();
                    }
                    else
                    {
                        SelItemValues += "," + Item.Value.Trim();
                    }
                }
            }
            return SelItemValues;
        }

        public virtual bool Validate()
        {
            if (!IsNull)
            {
                if (SelectedValue.Trim() == string.Empty)
                {
                    return false;
                }
            }
            return true;
        }

        #endregion

        #region ��������

        public bool IsNull
        {
            get
            {
                if (ViewState["isNull"] != null)
                    return (bool) ViewState["isNull"];
                return true;
            }
            set { ViewState["isNull"] = value; }
        }

        public bool IsValid
        {
            get { return Validate(); }
        }

        #endregion

        #region IQueryControl ��Ա

        public string CompareSymbol
        {
            get
            {
                if (ViewState["CompareSymbol"] != null)
                    return (string) ViewState["CompareSymbol"];
                return "";
            }
            set { ViewState["CompareSymbol"] = value; }
        }

        public string QueryFormatString
        {
            get
            {
                if (ViewState["QueryFormatString"] != null)
                    return (string) ViewState["QueryFormatString"];
                return "";
            }
            set { ViewState["QueryFormatString"] = value; }
        }

        #endregion

        #endregion
    }
}