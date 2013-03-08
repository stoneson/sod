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
	/// Summary description for BrainLabel.
	/// </summary>
    [System.Drawing.ToolboxBitmap(typeof(ControlIcon), "DataLable.bmp")]
	public class DataLabel:Label,IDataControl
	{
		public DataLabel()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		#region �������

		/// <summary>
		/// ���ݳ��ָ�ʽ
		/// </summary>
		[Category("���"),Description("���ݳ��ָ�ʽ")]
		public string DataFormatString
		{
			get
			{
				if(ViewState["DataFormatString"]!=null)
					return (string)ViewState["DataFormatString"];
				return "";
			}
			set
			{
				ViewState["DataFormatString"]=value;
			}
		}

		#endregion

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

		[Category("Data"),Description("�趨��Ӧ�������ֶ�����")]
		public System.TypeCode SysTypeCode
		{
			get
			{
				if(ViewState["SysTypeCode"]!=null)
					return (System.TypeCode)ViewState["SysTypeCode"];
				return new System.TypeCode ();
			}
			set
			{
				ViewState["SysTypeCode"] = value;
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

		[Category("Data"),Description("�趨�����ݿ��ֶζ�Ӧ�����ݱ���")]
		public string LinkObject
		{
			get
			{
				if(ViewState["LinkObject"]!=null)
					return (string)ViewState["LinkObject"];
				return "";
			}
			set
			{
				ViewState["LinkObject"]=value;
			}
		}

		#endregion

		#region �ӿڷ���

        public void SetValue(object obj)
		{
            //if(value!=null)
            //    if(DataFormatString != "")
            //    {
            //        this.Text=String.Format(DataFormatString,value);
            //    }
            //    else
            //    {
            //        this.Text=value.ToString ();
            //    }
			    
            //else
            //    this.Text="";
            if (obj == null || obj.ToString() == "")
            {
                this.Text = "";
                return;
            }

            // ��̫�� 2006.8.11 ��ӵ������ͺ�Ĭ�����͵�ʵ��
            switch (this.SysTypeCode)
            {
                case TypeCode.String:
                    if (obj != DBNull.Value)
                    {
                        if (DataFormatString != "")
                            this.Text = String.Format(DataFormatString, obj.ToString());
                        else
                            this.Text = obj.ToString().Trim();
                    }
                    else
                    {
                        this.Text = "";
                    }
                    break;
                case TypeCode.Int32:
                    if (obj != DBNull.Value && obj.GetType() == typeof(int))
                    {
                        this.Text = obj.ToString().Trim();
                    }
                    else
                    {
                        this.Text = "";
                    }
                    break;
                case TypeCode.Decimal:
                    if (obj != DBNull.Value && obj.GetType() == typeof(decimal))
                    {
                        if (DataFormatString != "")
                            this.Text = String.Format(DataFormatString, obj);
                        else
                            this.Text = obj.ToString().Trim();
                    }
                    else
                    {
                        this.Text = "";
                    }
                    break;
                case TypeCode.DateTime:
                    if (obj != DBNull.Value && obj.GetType() == typeof(DateTime))
                    {
                        if (DataFormatString != "")
                        {
                            this.Text = String.Format(DataFormatString, obj);
                        }
                        else
                        {
                            //this.Text=((DateTime)obj).ToShortDateString().Trim();
                            //û�и�ʽ����Ϣ������ԭ�����ݸ�ʽ dth,2008.4.4
                            this.Text = ((DateTime)obj).ToString();
                        }
                    }
                    else
                    {
                        this.Text = "";
                    }
                    break;
                case TypeCode.Double:
                case TypeCode.Single:
                    if (obj != DBNull.Value && (obj.GetType() == typeof(double) || obj.GetType() == typeof(float)))
                    {
                        if (DataFormatString != "")
                            this.Text = String.Format(DataFormatString, obj);
                        else
                            this.Text = obj.ToString().Trim();
                    }
                    else
                    {
                        this.Text = "";
                    }
                    break;
                default:
                    this.Text = obj.ToString().Trim();
                    break;
            }

		}

		public object GetValue()
		{

            switch (this.SysTypeCode)
            {
                case TypeCode.String:
                    {
                        return this.Text.Trim();
                    }
                case TypeCode.Int32:
                    {
                        if (this.Text.Trim() != "")
                        {
                            return Convert.ToInt32(this.Text.Trim());
                        }
                        //return 0;
                        return DBNull.Value;
                    }
                case TypeCode.Decimal:
                    {
                        if (this.Text.Trim() != "")
                        {
                            return Convert.ToDecimal(this.Text.Trim());
                        }
                        //return 0;
                        return DBNull.Value;
                    }
                case TypeCode.DateTime:
                    if (this.Text.Trim() != "")
                    {
                        try
                        {
                            return Convert.ToDateTime(this.Text.Trim());
                        }
                        catch
                        {
                            return DBNull.Value; //"1900-1-1";
                        }
                    }
                    return DBNull.Value;//"1900-1-1";

                case TypeCode.Double:
                    {
                        if (this.Text.Trim() != "")
                        {
                            return Convert.ToDouble(this.Text.Trim());
                        }
                        //return 0;
                        return DBNull.Value;
                    }
                case TypeCode.Boolean:
                    {
                        if (this.Text.Trim() != "")
                        {
                            try
                            {
                                return Convert.ToBoolean(this.Text.Trim());
                            }
                            catch
                            {
                                return DBNull.Value; //"1900-1-1";
                            }
                        }
                        return DBNull.Value;//"1900-1-1";
                    }
                default:
                    if (this.Text.Trim() == "")
                    {
                        return DBNull.Value;
                    }
                    else
                    {
                        return this.Text.Trim();
                    }
            }
		}

		public virtual bool Validate()
		{

			return true;
		}

		#endregion

		#region Ĭ������

		public bool isClientValidation
		{
			get
			{
				
				return false;
			}
		}

		public bool isNull
		{

			get
			{
				return true;
			}
			set
			{

			}
		}

		public bool IsValid
		{
			get
			{
				return Validate();
			}
		}

		public bool ReadOnly
		{
			get
			{
				return true;
			}
			set
			{
			}
		}
		#endregion

		#endregion
	}
}
