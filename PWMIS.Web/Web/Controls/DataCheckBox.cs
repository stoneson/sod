/*
 * ========================================================================
 * Copyright(c) 2006-2010 PWMIS, All Rights Reserved.
 * Welcom use the PDF.NET (PWMIS Data Process Framework).
 * See more information,Please goto http://www.pwmis.com/sqlmap 
 * ========================================================================
 * ���������
 * 
 * ���ߣ���̫��     ʱ�䣺2008-10-12
 * �汾��V4.5
 * 
 * �޸��ߣ�         ʱ�䣺2013-3-1                
 * �޸�˵���������˿ؼ�
 * ========================================================================
*/
using System;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using PWMIS.Common;


namespace PWMIS.Web.Controls
{
	/// <summary>
	/// BrainCheckBox ��ժҪ˵����2008.7.6
	/// </summary>
    [System.Drawing.ToolboxBitmap(typeof(ControlIcon), "DataCheckBox.bmp")]
    public class DataCheckBox : CheckBox, IDataControl, IQueryControl
	{
		public DataCheckBox()
		{
			//
			// TODO: �ڴ˴���ӹ��캯���߼�
			//
		}

		

		#region IBrainControl ��Ա

		#region ��������
		/// <summary>
		/// ָ���Ƿ�������
		/// </summary>
		[Category("Data"),Description("�趨��Ӧ�����ݿ��ֶ��Ƿ��������������Զ����ݲ�ѯ�͸��µ�����")]
		public bool PrimaryKey
		{
			get
			{
				if(ViewState["PrimaryKey"]!=null)
					return (bool)ViewState["PrimaryKey"];
				return false;
			}
			set
			{
				ViewState["PrimaryKey"]=value;
			}
		}

		[Category("Data"),Description("�趨�����ݿ��ֶζ�Ӧ������ֵ")]
		public string Value
		{
			get
			{
				if(ViewState["CheckBoxvalue"]!=null)
					return (string)ViewState["CheckBoxvalue"];
				return "";
			}
			set
			{
				ViewState["CheckBoxvalue"]=value;
			}
		}

		[Category("Data"),Description("�趨�����ݿ��ֶζ�Ӧ��������")]
		public string LinkProperty
		{
			get
			{
				if(ViewState["LinkProperty"]!=null)
					return (string)ViewState["LinkProperty"];
				return "";
			}
			set
			{
				ViewState["LinkProperty"]=value;
			}
		}

		[Category("Data"),Description("�趨�����ݱ��Ӧ�����ݱ���")]
		public string LinkObject
		{
			get
			{
				if(ViewState["LinkObject"]!=null)
					return (string)ViewState["LinkObject"];
				return "";
				// TODO:  ��� BrainDropDownList.LinkObject getter ʵ��
			}
			set
			{
				ViewState["LinkObject"]=value;
				// TODO:  ��� BrainDropDownList.LinkObject setter ʵ��
			}
		}

		#endregion

		#region Ĭ������

		public bool IsValid
		{
			get
			{
				// TODO:  ��� BrainListBox.IsValid getter ʵ��
				return true;
			}
		}
		public System.TypeCode SysTypeCode
		{
			get
			{
				// TODO:  ��� BrainCheckBox.SysTypeCode getter ʵ��
				return System.TypeCode.String;
			}
			set
			{
				// TODO:  ��� BrainCheckBox.SysTypeCode setter ʵ��
			}
		}

		public virtual bool Validate()
		{
			// TODO:  ��� BrainListBox.Validate ʵ��
			return true;
		}
		#endregion

		#region ��������

		public bool ReadOnly
		{
			get
			{
                if (!this.Checked)//���δѡ����ô�趨Ϊֻ���������ԡ�
                    return true;
				return !base.Enabled;
			}
			set
			{
				base.Enabled=!value;
			}
		}

		[Category("Data"),Description("�趨�������Ƿ����")]
		public bool isNull
		{

			get
			{
				// TODO:  ��� BrainTextBox.isClientValidation getter ʵ��
				if(ViewState["isNull"]!=null)
					return (bool)ViewState["isNull"];
				return true;
			}
			set
			{
				// TODO:  ��� BrainTextBox.isClientValidation setter ʵ��
				ViewState["isNull"] = value;

			}
		}
		#endregion

		#region �ӿڷ���

		public void SetValue(object obj)
		{
			this.Checked = false;
            if (obj==null || obj == DBNull.Value)
            {
                //this.isNull = true;
                //this.Value = "";
                return;
            }
			string SelItemValues = "";
            SelItemValues = obj.ToString().Trim();
            //string delimStr = ",";
            //char [] delimiter = delimStr.ToCharArray();

			string [] SelItemobj =SelItemValues.Split(',');// SelItemValues.Split(delimiter);
			
			foreach(string s in SelItemobj)
			{
				if(this.Value.Trim() == s.Trim())
				{
					this.Checked=true;
                    break;//add 2008.7.26
				}
			}
		}

        public object GetValue()
        {
            switch (this.SysTypeCode)
            {
                case TypeCode.String:
                    {
                        return this.Value.Trim();
                    }
                case TypeCode.Int32:
                    {
                        if (this.Value.Trim() != "")
                        {
                            return Convert.ToInt32(this.Value.Trim());
                        }
                        //return 0;
                        return DBNull.Value;
                    }
                case TypeCode.Decimal:
                    {
                        if (this.Value.Trim() != "")
                        {
                            return Convert.ToDecimal(this.Value.Trim());
                        }
                        //return 0;
                        return DBNull.Value;
                    }
                case TypeCode.DateTime:
                    if (this.Value.Trim() != "")
                    {
                        try
                        {
                            return Convert.ToDateTime(this.Value.Trim());
                        }
                        catch
                        {
                            return DBNull.Value; //"1900-1-1";
                        }
                    }
                    return DBNull.Value;//"1900-1-1";

                case TypeCode.Double:
                    {
                        if (this.Value.Trim() != "")
                        {
                            return Convert.ToDouble(this.Value.Trim());
                        }
                        //return 0;
                        return DBNull.Value;
                    }
                case TypeCode.Boolean:
                    {
                        if (this.Value.Trim() != "")
                        {
                            try
                            {
                                return Convert.ToBoolean(this.Value.Trim());
                            }
                            catch
                            {
                                return DBNull.Value; //"1900-1-1";
                            }
                        }
                        return DBNull.Value;//"1900-1-1";
                    }
                default:
                    if (this.Value.Trim() == "")
                    {
                        return DBNull.Value;
                    }
                    else
                    {
                        return this.Value.Trim();
                    }
            }
        }
		#endregion

        #region IQueryControl ��Ա

        public string CompareSymbol
        {
            get
            {
                if (ViewState["CompareSymbol"] != null)
                    return (string)ViewState["CompareSymbol"];
                return "";
            }
            set
            {
                ViewState["CompareSymbol"] = value;
            }
        }

        public string QueryFormatString
        {
            get
            {
                if (ViewState["QueryFormatString"] != null)
                    return (string)ViewState["QueryFormatString"];
                return "";
            }
            set
            {
                ViewState["QueryFormatString"] = value;
            }
        }

        #endregion


		#endregion
	}
}
