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

//**************************************************************************
//	�� �� ����  
//	��	  ;��  TextBox��չ���ռ�����
//	�� �� �ˣ�  �����
//  �������ڣ�  2006.03.09
//	�� �� �ţ�	V1.1
//	�޸ļ�¼��  ��̫�� 2006.04.25 ��Ӷ����ַ�����������֤
//              ��̫�� 20060608 �޸ģ�����ֻ��״̬�²�����ʽ����������ñ������֣�
//              �涨���е�ֻ����ʽ��Ϊ�� CssReadOnly
//              ȡ����ʽ���ƹ��ܣ����û���ʽ���壻
//              ��̫�� 2008.2.15 ���ӡ����������ԣ������Զ����ݸ��µ�����
//                     2009.12.29 ������֤����
//**************************************************************************

using System;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Web;
using System.Web.UI.WebControls;
using PWMIS.Common;
using PWMIS.DataMap;
using PWMIS.Web.Validate;

namespace PWMIS.Web.Controls
{
    /// <summary>
    ///     �����ı���ؼ�.
    /// </summary>
    [ToolboxBitmap(typeof (ControlIcon), "DataTextBox.bmp")]
    public class DataTextBox : TextBox, IDataTextBox, IQueryControl
    {
        //private string _BaseText=null ;

        /// <summary>
        ///     Ĭ�Ϲ��캯��
        /// </summary>
        public DataTextBox()
        {
            EnsureChildControls();
        }

        #region �������

        /// <summary>
        ///     ��֤ʧ�ܳ��ֵ���Ϣ
        /// </summary>
        [Category("�ؼ���֤"), Description("��֤ʧ�ܳ��ֵ���Ϣ")]
        public string ErrorMessage
        {
            get
            {
                if (ViewState["ErrorMessage"] != null)
                    return (string) ViewState["ErrorMessage"];
                return "";
            }
            set { ViewState["ErrorMessage"] = value; }
        }

        #endregion

        #region �ؼ���֤

        /// <summary>
        ///     ִ�з�������֤ʱ����������
        /// </summary>
        [Category("�ؼ���֤"), Description("ִ�з�������֤ʱ����������")]
        public ValidationDataType Type
        {
            get
            {
                if (ViewState["ValidationDataType"] != null)
                    return (ValidationDataType) ViewState["ValidationDataType"];
                return ValidationDataType.String;
            }
            set
            {
                ViewState["ValidationDataType"] = value;
                //ȡ����ʽ���ƹ��ܣ����û���ʽ����
                //				switch(value)
                //				{
                //					case ValidationDataType.Currency:
                //					case ValidationDataType.Double:
                //					case ValidationDataType.Integer:
                //						this.Style.Add("TEXT-ALIGN","right");
                //						break;
                //					default:
                //						this.Style.Remove("TEXT-ALIGN");
                //						break;
                //				}
            }
        }

        /// <summary>
        ///     �Ƿ�ͨ����������֤Ĭ��Ϊtrue
        /// </summary>
        [Category("�ؼ���֤"), Description("�Ƿ�ͨ����������֤Ĭ��Ϊtrue")]
        public bool IsValid
        {
            get
            {
                if (!isClientValidation)
                {
                    return Validate();
                }
                return true;
            }
        }


        /// <summary>
        ///     ��ʾ��Ϣ������
        /// </summary>
        [Category("�ؼ���֤"), Description("��ʾ��Ϣ������")]
        [TypeConverter(typeof (EnumConverter))]
        public EnumMessageType MessageType { get; set; }

        /// <summary>
        ///     ��Ҫ��֤�ĳ����������ͣ�����趨Ϊ���ޡ�����ֹͣ�ؼ���֤��
        /// </summary>
        [Category("�ؼ���֤"), Description("��Ҫ��֤�ĳ����������ͣ�����趨Ϊ���ޡ�����ֹͣ�ؼ���֤��")]
        [TypeConverter(typeof (StandardRegexListConvertor))]
        public string OftenType
        {
            get
            {
                if (ViewState["OftenType"] != null)
                    return ViewState["OftenType"].ToString();
                return "��";
            }
            set
            {
                ViewState["OftenType"] = value;
                if (value == "��")
                {
                    RegexString = "";
                    isClientValidation = false;
                }
                else
                    RegexString = "^" + RegexStatic.GetGenerateRegex()[value] + "$";
            }
        }

        /// <summary>
        ///     �趨�ؼ���֤��������ʽ
        /// </summary>
        [Category("�ؼ���֤"), Description("�趨�ؼ���֤��������ʽ")]
        public string RegexString
        {
            get
            {
                if (ViewState["RegexString"] != null)
                    return (string) ViewState["RegexString"];
                return "";
            }
            set { ViewState["RegexString"] = value; }
        }

        /// <summary>
        ///     ��֤������
        /// </summary>
        [Category("�ؼ���֤"), Description("��֤������")]
        public string RegexName
        {
            get
            {
                if (ViewState["RegexName"] != null)
                    return (string) ViewState["RegexName"];
                return "";
            }
            set { ViewState["RegexName"] = value; }
        }

        /// <summary>
        ///     �趨�Ƿ����ؼ���ʾ��Ϣ
        /// </summary>
        [Category("�ؼ���֤"), Description("�趨�Ƿ����ؼ���ʾ��Ϣ"), DefaultValue(false)]
        public bool OnClickShowInfo
        {
            get
            {
                if (ViewState["OnClickShowInfo"] != null)
                    return (bool) ViewState["OnClickShowInfo"];
                return false;
            }
            set { ViewState["OnClickShowInfo"] = value; }
        }

        /// <summary>
        ///     �趨�ű�·��
        /// </summary>
        [Category("Data"), Description("�趨�ű�·��")]
        public string ScriptPath
        {
            get
            {
                if (ViewState["ScriptPath"] != null)
                    return (string) ViewState["ScriptPath"];
                return Root + "System/WebControls/script.js";
            }
            set { ViewState["ScriptPath"] = value; }
        }

        private string Root
        {
            get
            {
                if (!DesignMode && HttpContext.Current.Request.ApplicationPath != "/")
                {
                    return HttpContext.Current.Request.ApplicationPath + "/";
                }
                return "/";
            }
        }

        #endregion

        #region ��������

        [Category("Data"), Description("�趨��Ӧ������Դ����ʽ��FullClassName,AssemblyName �������Ҫ��ʵ���࣬�������ø����ԡ�")]
        public string DataProvider { get; set; }

        /// <summary>
        ///     �趨��Ӧ�����ݿ��ֶ��Ƿ��������������Զ����ݲ�ѯ�͸��µ�����
        /// </summary>
        [Category("Data"), Description("�趨��Ӧ�����ݿ��ֶ��Ƿ��������������Զ����ݲ�ѯ�͸��µ�����"), DefaultValue(false)]
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

        /// <summary>
        ///     �趨��Ӧ�������ֶ�����
        /// </summary>
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

        #region �ű����

        protected override void OnLoad(EventArgs e)
        {
            var rootScript = "\r\n<script  type=\"text/javascript\" language=\"javascript\">var RootSitePath='" + Root +
                             "';</" + "script>\r\n";
            Page.ClientScript.RegisterStartupScript(GetType(), "JS",
                rootScript + "\r\n<script src=\"" + ScriptPath + "\"></script>\r\n");
            base.OnLoad(e);
        }

        protected override void OnPreRender(EventArgs e)
        {
            var messageType = "";
            switch (MessageType)
            {
                case EnumMessageType.��:
                    messageType = "tip";
                    break;
                case EnumMessageType.��ʾ��:
                    messageType = "alert";
                    break;
            }
            //����ؼ���ʾ��Ϣ
            if (OnClickShowInfo)
            {
                //�������ʾ��ʽʼ���Բ�����ʾ
                Attributes.Add("onclick", "DTControl_SetInputBG(this);ShowMessage('����д" + RegexName + "',this,'tip');");
                Attributes.Add("onblur", "DTControl_CleInputBG(this);DTControl_Hide_TIPDIV();");
            }


            if (IsNull && OftenType == "��")
            {
                base.OnPreRender(e);
                return;
            }
            if (!IsNull)
            {
                //����Ϊ��
                Page.ClientScript.RegisterOnSubmitStatement(GetType(), UniqueID,
                    "if(document.all." + ClientID + ".value==''){\r\n ShowMessage('�����Ϊ��!',document.all." + ClientID +
                    ",'" + messageType + "');\r\n document.all." + ClientID + ".focus();return false;\r\n}\r\n");
            }


            //switch (this.OftenType)
            //{
            //    case "����":
            //        string path = Root + "System/JS/My97DatePicker/WdatePicker.js";
            //        this.Page.ClientScript.RegisterStartupScript(this.GetType (),"JS_calendar", "\r\n<script language='javascript' type='text/javascript' src='"+path+"'></script>\r\n");
            //        this.Attributes.Add("onfocus", "new WdatePicker(this)");

            //        break;

            //}
            if (this.RegexString != "" && OnClickShowInfo && !isClientValidation)
            {
                var RegexString = this.RegexString.Replace(@"\", @"\\");
                Attributes.Add("onchange",
                    "return isCustomRegular(this,'" + RegexString + "','" + RegexName + "û����д��ȷ','" + messageType +
                    "');");
            }
            ////
            if (!isClientValidation) //�ؼ���֤�ű�
            {
                var script =
                    @"javascript:var key= (event.keyCode | event.which);if( !(( key>=48 && key<=57)|| key==46 || key==37 || key==39 || key==45 || key==43 || key==8 || key==46  ) ) {try{ event.returnValue = false;event.preventDefault();}catch(ex){} alert('" +
                    ErrorMessage + "');}";
                switch (Type)
                {
                    case ValidationDataType.String:
                        //Convert.ToString(this.Text.Trim());
                        //��̫�� 2006.04.25 ��Ӷ����ַ�����������֤
                        if (MaxLength > 0 && TextMode == TextBoxMode.MultiLine)
                        {
                            var maxlen = MaxLength.ToString();
                            Attributes.Add("onblur",
                                "javascript:if(this.value.length>" + maxlen + "){this.select();alert('��������ֲ��ܳ��� " +
                                maxlen + " ���ַ���');MaxLenError=true;}else{MaxLenError=false;}");
                        }
                        break;
                    case ValidationDataType.Integer:
                        Attributes.Add("onkeypress", script);
                        break;

                    case ValidationDataType.Currency:
                        Attributes.Add("onkeypress", script);
                        break;

                    case ValidationDataType.Date:
                        var path = Root + "System/JS/My97DatePicker/WdatePicker.js";
                        Page.ClientScript.RegisterStartupScript(GetType(), "JS_calendar",
                            "\r\n<script language='javascript' type='text/javascript' src='" + path + "'></script>\r\n");
                        Attributes.Add("onfocus", "new WdatePicker(this)");
                        break;
                    case ValidationDataType.Double:
                        Attributes.Add("onkeypress", script);
                        break;
                }
            }
            else //ִ���Զ�����֤������Զ���ű�
            {
                RegisterClientScript();
                if (ClientValidationFunctionString != "")
                {
                    Attributes.Add("onblur",
                        @"if(strCheck_" + ID + "(this.value)==false) {this.value = '';alert('" + ErrorMessage + "');}");
                }
            }
            base.OnPreRender(e);
        }


        /// <summary>
        ///     ����ű�
        /// </summary>
        protected virtual void RegisterClientScript()
        {
            var versionInfo = Assembly.GetAssembly(GetType()).FullName;
            var start = versionInfo.IndexOf("Version=") + 8;
            var end = versionInfo.IndexOf(",", start);
            versionInfo = versionInfo.Substring(start, end - start);
            var info = @"
<!--
 ********************************************
 * ServerControls " + versionInfo + @"
 ********************************************
-->";

            var ClientFunctionString = @"<SCRIPT language=javascript >
function strCheck_" + ID + @"(str)
{
var pattern =/" + ClientValidationFunctionString + @"/;
if(pattern.test(str)) 
{
return true; 
}
  return false;}
</SCRIPT>";

            if (ClientValidationFunctionString == "")
            {
                ClientFunctionString = "";
            }
            if (ClientFunctionString != string.Empty)
            {
                Page.ClientScript.RegisterClientScriptBlock(GetType(), ID + "_Info", info);
            }


            Page.ClientScript.RegisterClientScriptBlock(GetType(), ID + "_ValidationFunction", ClientFunctionString);
        }

        #endregion

        #region IBrainControl ��Ա

        #region ��������

        /// <summary>
        ///     �趨�����ݿ��ֶζ�Ӧ��������
        /// </summary>
        [Category("Data"), Description("�趨�����ݿ��ֶζ�Ӧ��������")]
        public string LinkProperty
        {
            get
            {
                // TODO:  ��� BrainTextBox.LinkProperty getter ʵ��
                if (ViewState["LinkProperty"] != null)
                    return (string) ViewState["LinkProperty"];
                return "";
            }
            set
            {
                ViewState["LinkProperty"] = value;
                // TODO:  ��� BrainTextBox.LinkProperty setter ʵ��
            }
        }

        /// <summary>
        ///     �趨�����ݿ��ֶζ�Ӧ�����ݱ���
        /// </summary>
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

        #region ��������

        //�Ƿ�ֻ��
        public override bool ReadOnly
        {
            get { return base.ReadOnly; }
            set
            {
                base.ReadOnly = value;
                if (value)
                    //��̫�� 20060608 �޸ģ�����ֻ��״̬�²�����ʽ����������ñ������֣�����һ�б�ע��
                    //this.BackColor=System.Drawing.Color.FromName("#E0E0E0");
                    CssClass = "CssReadOnly";
                else
                    BackColor = Color.Empty;
            }
        }

        //		/// <summary>
        //		/// ��ȡ���������ı�����������˸�ʽ�ַ�������ô��ʾ�ı�Ϊ��ʽ������ı��������ڲ������ʱ����Ȼʹ�ø�ʽ��ǰ���ı�
        //		/// </summary>
        //		public override string Text
        //		{
        //			get
        //			{
        ////				if(_BaseText==null)
        ////				{
        ////					if(ViewState["BaseText"]!=null)
        ////						_BaseText=ViewState["BaseText"].ToString ();
        ////					else
        ////						_BaseText= base.Text;
        ////				}
        ////				return _BaseText;
        //				return base.Text;
        //				
        //			}
        //			set
        //			{
        //				if(DataFormatString!="")
        //					base.Text =String.Format(DataFormatString,value); 
        //				else
        //					base.Text =value;
        ////				_BaseText=value;
        ////				ViewState["BaseText"]=value;
        //			}
        //		}

        #endregion

        #region �ӿڷ���

        //��������
        public void SetValue(object obj)
        {
            var dtbv = new DataTextBoxValue(this);
            dtbv.SetValue(obj);
        }

        //�����ռ� 
        //Ϊ��ʱstring ���� ����
        //��������  һ�ɷ���dbnull.value
        //��̫�� 2006.8.23 �޸ģ���������͵�ֵΪ���ַ�������ô����ֵ�޸�Ϊ DBNull.Value ������Ĭ�ϵ� "0"
        public object GetValue()
        {
            var dtbv = new DataTextBoxValue(this);
            return dtbv.GetValue();
        }

        #endregion

        #region �ؼ���֤����

        public bool Validate()
        {
            // TODO:  ��� BrainTextBox.Validate ʵ��

            //��������ؼ���֤
            if (!isClientValidation)
            {
                if (Text.Trim() != "")
                {
                    try
                    {
                        switch (Type)
                        {
                            case ValidationDataType.String:
                                Convert.ToString(Text.Trim());
                                break;
                            case ValidationDataType.Integer:
                                Convert.ToInt32(Text.Trim());
                                break;

                            case ValidationDataType.Currency:
                                Convert.ToDecimal(Text.Trim());
                                break;

                            case ValidationDataType.Date:
                                Convert.ToDateTime(Text.Trim());
                                break;
                            case ValidationDataType.Double:
                                Convert.ToDouble(Text.Trim());
                                break;
                        }
                        return true;
                        //						if(!this.isNull)//������Ϊ��
                        //						{
                        //							return false;
                        //						}
                        //						else//����Ϊ��
                        //						{
                        //							return true;
                        //						}
                    }
                    catch
                    {
                        return false; //�쳣 �������� ������
                    }
                }
                //��̫�� 2006.05.8 �޸ģ��������ֵΪ���ڽ����жϣ����沿���ѱ�ע��
                //return true;
                if (!IsNull) //������Ϊ��
                {
                    return false;
                }
                return true;
            }
            return true;
        }

        #endregion

        #region �Զ�����֤����

        /// <summary>
        ///     �趨�Զ�����֤������ʽ
        /// </summary>
        [Category("�Զ�����֤"), Description("�趨�Զ�����֤������ʽ"), DefaultValue("")]
        public string ClientValidationFunctionString
        {
            get
            {
                if (ViewState["ClientValidationFunctionString"] != null)
                    return (string) ViewState["ClientValidationFunctionString"];
                return "";
            }
            set { ViewState["ClientValidationFunctionString"] = value; }
        }

        /// <summary>
        ///     �趨�ؼ��Ƿ��ȡ�Զ�����֤(ֹͣ�ؼ���֤)
        /// </summary>
        [Category("�Զ�����֤"), Description("�趨�ؼ��Ƿ��ȡ�Զ�����֤(ֹͣ�ؼ���֤)"), DefaultValue(false)]
        public bool isClientValidation
        {
            get
            {
                if (ViewState["ClientValidation"] != null)
                    return (bool) ViewState["ClientValidation"];
                return false;
            }
            set { ViewState["ClientValidation"] = value; }
        }

        #endregion

        #region �ؼ���֤

        /// <summary>
        ///     �ؼ���֤--�趨�ؼ�ֵ�Ƿ����Ϊ��
        /// </summary>
        [Category("�ؼ���֤"), Description("�趨�ؼ�ֵ�Ƿ����Ϊ��"), DefaultValue(true)]
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

        #endregion

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
    }
}