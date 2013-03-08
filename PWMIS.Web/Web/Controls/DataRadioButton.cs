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
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using PWMIS.Common;


namespace PWMIS.Web.Controls
{
	/// <summary>
    /// ���ݸ�ѡ��ؼ�
	/// </summary>
    [System.Drawing.ToolboxBitmap(typeof(ControlIcon), "DataRadioButton.bmp")]
    public class DataRadioButton : RadioButton, IDataControl, IQueryControl
	{
		public DataRadioButton()
		{
			//
			// TODO: �ڴ˴���ӹ��캯���߼�
			//
		}

		#region IBrainControl ��Ա

		#region ��������
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

		[Category("���"),Description("�趨�����ݿ��ֶζ�Ӧ������ֵ")]
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
				// TODO:  ��� BrainDropDownList.LinkProperty getter ʵ��
				if(ViewState["LinkProperty"]!=null)
					return (string)ViewState["LinkProperty"];
				return "";
			}
			set
			{
				ViewState["LinkProperty"]=value;
				// TODO:  ��� BrainDropDownList.LinkProperty setter ʵ��
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
				return true;
			}
		}
		public System.TypeCode SysTypeCode
		{
			get
			{
				if(ViewState["SysTypeCode"]!=null)
					return (TypeCode)ViewState["SysTypeCode"];
				return System.TypeCode.String;
			}
			set
			{
				ViewState["SysTypeCode"]=value;
			}
		}

		public bool ReadOnly
		{
			get
			{
                if (this.Checked)
                    return false ;
				return !base.Enabled;
			}
			set
			{
				base.Enabled=!value;
			}
		}

		#endregion

		#region ��ڷ���

		public bool isNull
		{

			get
			{
				if(ViewState["isNull"]!=null)
					return (bool)ViewState["isNull"];
				return true;
			}
			set
			{
				ViewState["isNull"] = value;
			}
		}

		public void SetValue(object obj)
		{
			this.Checked = false;
			if(obj!=null)
			{
				string SelItemValues = obj.ToString().Trim();
                string strValue = this.Value.Trim();
                if (strValue == SelItemValues.Trim())
				{
					this.Checked=true;
                    return;
				}
                //����ֵ���⴦�����ݿ��п��ܴ洢��ֵΪ0����1
                if (this.SysTypeCode == TypeCode.Boolean)
                {
                    if (strValue.ToLower() == "true" && SelItemValues == "1")
                        this.Checked = true;
                    else if (strValue.ToLower() == "false" && SelItemValues == "0")
                        this.Checked = true;
                }

			}

		}

		public object GetValue()
		{
            return this.Checked ? GetValueInner() : DBNull.Value;
		}

        private object GetValueInner()
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

		public virtual bool Validate()
		{
			return true;
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
