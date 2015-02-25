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
 * �޸��ߣ�         ʱ�䣺2013-3-1                
 * �޸�˵���������˿ؼ�
 * ========================================================================
*/

using System;
using System.ComponentModel;
using System.Drawing;
using System.Web.UI.WebControls;
using PWMIS.Common;
using PWMIS.DataMap;

namespace PWMIS.Web.Controls
{
    /// <summary>
    ///     Summary description for BrainLabel.
    /// </summary>
    [ToolboxBitmap(typeof (ControlIcon), "DataLable.bmp")]
    public class DataLabel : Label, IDataTextBox
    {
        #region �������

        /// <summary>
        ///     ���ݳ��ָ�ʽ
        /// </summary>
        [Category("���"), Description("���ݳ��ָ�ʽ")]
        public string DataFormatString
        {
            get
            {
                if (ViewState["DataFormatString"] != null)
                    return (string) ViewState["DataFormatString"];
                return "";
            }
            set { ViewState["DataFormatString"] = value; }
        }

        #endregion

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

        [Category("Data"), Description("�趨�����ݿ��ֶζ�Ӧ�����ݱ���")]
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

        #region �ӿڷ���

        public void SetValue(object value)
        {
            var dtbv = new DataTextBoxValue(this);
            dtbv.SetValue(value);
        }

        public object GetValue()
        {
            var dtbv = new DataTextBoxValue(this);
            return dtbv.GetValue();
        }


        public virtual bool Validate()
        {
            return true;
        }

        #endregion

        #region Ĭ������

        public bool isClientValidation
        {
            get { return false; }
        }

        public bool IsNull
        {
            get { return true; }
            set { }
        }

        public bool IsValid
        {
            get { return Validate(); }
        }

        public bool ReadOnly
        {
            get { return true; }
            set { }
        }

        public int MaxLength { get; set; }

        #endregion

        #endregion
    }
}