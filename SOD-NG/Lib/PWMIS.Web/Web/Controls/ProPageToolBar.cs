//ver 4.5 dbmstype auto get;

using System;
using System.Collections;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using PWMIS.Common;
using PWMIS.DataProvider.Adapter;
using PWMIS.DataProvider.Data;
//using System.Drawing.Design;

namespace PWMIS.Web.Controls
{
    /// <summary>
    ///     �����¼�ί�ж���
    /// </summary>
    public delegate void ClickEventHandler(object sender, EventArgs e);

    /// <summary>
    ///     ���ݰﶨί�ж���
    /// </summary>
    public delegate void DataBoundHandler(object sender, EventArgs e);

    /// <summary>
    ///     Web ��ҳ������
    ///     ��̫�� 2007.1.10 Ver 1.0��2008.5.8 Ver 1.0.1.2��2008.7.24 Ver 1.0.1.3
    ///     Ver 1.0.1 �������ݷ��ʹ���
    ///     Ver 1.0.1.1 �Զ��������ļ�����ȫ��Ĭ�����ò����������ҳ��С
    ///     Ver 1.0.1.2 ���˿����Զ����÷�ҳ��С�⣬�����������ض��ķ�ҳ��С��
    ///     Ver 1.0.1.3 ֧��GridView
    /// </summary>
    [ToolboxBitmap(typeof (ControlIcon), "DataPageToolBar.bmp")]
    [DefaultProperty("AllCount"),
     DefaultEvent("PageChangeIndex"),
     ToolboxData("<{0}:ProPageToolBar runat=server></{0}:ProPageToolBar>")]
    public class ProPageToolBar : WebControl, INamingContainer
    {
        /// <summary>
        ///     δ��ʼ������ֵ
        /// </summary>
        private const int UNKNOW_NUM = -999;

        private CommonDB DAO
        {
            get
            {
                if (_DAO == null)
                {
                    CheckAutoConfig();
                    CheckAutoIDB();
                }
                if (_DAO == null)
                    throw new Exception("δʵ�������ݷ������,��ȷ���Ѿ���������ȷ�����ã�");

                //edit dbmstype:
                DBMSType = _DAO.CurrentDBMSType;
                return _DAO;
            }
            set { _DAO = value; }
        }

        #region �ڲ��ؼ�����

        protected Label lblAllCount = new Label();
        protected Label lblCPA = new Label();
        protected LinkButton lnkFirstPage = new LinkButton();
        protected LinkButton lnkPrePage = new LinkButton();
        protected LinkButton lnkNextPage = new LinkButton();
        protected LinkButton lnkLastPage = new LinkButton();
        protected TextBox txtNavePage = new TextBox();
        protected DropDownList dlPageSize = new DropDownList();
        protected LinkButton lnkGo = new LinkButton();

        #endregion

        #region �ֲ���������

        private int PageIndex = UNKNOW_NUM; //
        private int _AllCount;
        private int _PageSize;
        private int _CurrentPage;
        private bool hasSetBgColor;

        private bool ChangePageProperty;
        private bool _UserChangePageSize = true;
        private bool _ShowEmptyData = true;

        private string _SQL;
        private string _Where;
        private string _ConnectionString = string.Empty;
        private string _ErrorMessage = string.Empty;
        private CommonDB _DAO;
        private DBMSType _DBMSType = DBMSType.SqlServer;
        private IDataParameter[] _Parameters;

        #endregion

        #region �������Զ���

        [Bindable(true),
         Category("Appearance"),
         Description("��ҳ˵��"),
         DefaultValue("")]
        public string Text { get; set; }

        private string FontSize
        {
            get
            {
                return Font.Size.Unit.ToString(); //fontsize;
            }
        }

        /// <summary>
        ///     ��ҳ����������ʽ,0-Ĭ�ϣ�1-����ʾ��¼������2-����ʾҳ��ת��3-�Ȳ���ʾ��¼������Ҳ����ʾҳ��ת
        /// </summary>
        [Bindable(true),
         Category("��ҳ����"),
         Description("��ҳ�������ķ�ҳ��ʽ��0-Ĭ�ϣ�1-����ʾ��¼������2-����ʾҳ��ת��3-�Ȳ���ʾ��¼������Ҳ����ʾҳ��ת")
        ]
        public int PageToolBarStyle
        {
            get
            {
                if (ViewState["_PageToolBarStyle"] != null)
                    return (int) ViewState["_PageToolBarStyle"];
                return 0;
            }
            set { ViewState["_PageToolBarStyle"] = value; }
        }

        #endregion

        #region �ڲ��ؼ���ʽ������

        public string css_linkStyle = "";
        public string css_btnStyle = "";
        public string css_txtStyle = "";

        public ProPageToolBar()
        {
            AutoBindData = false;
            AutoConfig = false;
            AutoIDB = false;
        }

        #endregion

        #region ��ҳ���Զ���

        /// <summary>
        ///     ��ǰ����ҳ�룬Ĭ��ֵ1
        /// </summary>
        [Bindable(true),
         Category("��ҳ����"),
         Description("��ǰ����ҳ")
        ]
        public int CurrentPage
        {
            get
            {
                if (ViewState[ID + "_CurrentPage"] != null)
                    _CurrentPage = (int) ViewState[ID + "_CurrentPage"];
                return _CurrentPage <= 0 ? 1 : _CurrentPage;
            }
            set
            {
                if (value < 0) value = 1;
                _CurrentPage = value;
                ViewState[ID + "_CurrentPage"] = value;
                PageIndex = value;
                ChangePageProperty = true;
                txtNavePage.Text = value.ToString();
            }
        }

        /// <summary>
        ///     ��¼������Ĭ��ֵ0
        /// </summary>
        [Bindable(true),
         Category("��ҳ����"),
         Description("��¼����"),
         DefaultValue(0)]
        public int AllCount
        {
            get
            {
                if (ViewState[ID + "_AllCount"] != null)
                    _AllCount = (int) ViewState[ID + "_AllCount"];
                return _AllCount;
            }
            set
            {
                if (value < 0 && value != -1) value = 0;
                _AllCount = value;
                ViewState[ID + "_AllCount"] = value;
                ChangePageProperty = true;
                lblAllCount.Text = value.ToString();
            }
        }

        /// <summary>
        ///     ҳ���С��Ĭ��ֵ10������0��ʾ��ϵͳ�Զ���ȡ����ֵ
        /// </summary>
        [Bindable(true),
         Category("��ҳ����"),
         Description("ÿҳ���ҳ��¼��С��Ĭ��ֵ10,����0��ʾ��ϵͳ�Զ���ȡ����ֵ"),
         DefaultValue(10)]
        public int PageSize
        {
            get
            {
                if (ViewState[ID + "_PageSize"] != null)
                {
                    _PageSize = (int) ViewState[ID + "_PageSize"];
                    return _PageSize <= 0 ? 10 : _PageSize;
                }

                //����Ĭ�Ϸ�ҳ��С
                if (AutoConfig && _PageSize == 0)
                {
                    var defaultPageSize = ConfigurationSettings.AppSettings["PageSize"];
                    if (defaultPageSize != null && defaultPageSize != "")
                    {
                        _PageSize = int.Parse(defaultPageSize);
                        return _PageSize;
                    }
                    _PageSize = 10;
                }
                else
                {
                    _PageSize = _PageSize <= 0 ? 10 : _PageSize;
                }
                return _PageSize;
            }
            set
            {
                if (AutoConfig && value == 0)
                {
                    var defaultPageSize = ConfigurationSettings.AppSettings["PageSize"];
                    if (defaultPageSize != null && defaultPageSize != "")
                    {
                        _PageSize = int.Parse(defaultPageSize);
                    }
                    else
                    {
                        _PageSize = 10;
                    }
                    value = _PageSize;
                }
                if (value < 0) value = 10;
                _PageSize = value;
                ViewState[ID + "_PageSize"] = value;
                ChangePageProperty = true;
            }
        }

        /// <summary>
        ///     ҳ��������ֻ��
        /// </summary>
        [Bindable(true),
         Category("��ҳ����"),
         Description("ҳ��������ֻ��"),
         DefaultValue(1)]
        public int PageCount
        {
            get
            {
                var AllPage = AllCount/PageSize;
                if ((AllPage*PageSize) < AllCount) AllPage++;
                if (AllPage <= 0) AllPage = 1;
                return AllPage;
            }
        }

        [
            Category("��ҳ����"),
            Description("�Ƿ������û������ҳ���ʱ��ı��ҳ��С"),
            DefaultValue(true)]
        public bool UserChangePageSize
        {
            get
            {
                if (ViewState[ID + "_UserChangePageSize"] != null)
                    _UserChangePageSize = (bool) ViewState[ID + "_UserChangePageSize"];
                return _UserChangePageSize;
            }
            set
            {
                _UserChangePageSize = value;
                ViewState[ID + "_UserChangePageSize"] = value;
            }
        }

        #endregion

        #region ��ҳ�¼�

        /// <summary>
        ///     ҳ��ı��¼�
        /// </summary>
        [Category("��ҳ�¼�"),
         Description("ҳ��ı��¼�")]
        public event ClickEventHandler PageChangeIndex;

        /// <summary>
        ///     Ŀ��ؼ�������ݰ�֮ǰ���¼�
        /// </summary>
        [Category("Data"),
         Description("Ŀ��ؼ�������ݰ�֮ǰ���¼�")]
        public event DataBoundHandler DataControlDataBinding;

        /// <summary>
        ///     Ŀ��ؼ�������ݰ�����¼�
        /// </summary>
        [Category("Data"),
         Description("Ŀ��ؼ�������ݰ�����¼�")]
        public event DataBoundHandler DataControlDataBound;

        /// <summary>
        ///     �ı�ҳ������
        /// </summary>
        /// <param name="e">Ŀ��</param>
        protected void changeIndex(EventArgs e)
        {
            if (PageChangeIndex != null)
            {
                PageChangeIndex(this, e);
            }
            //if(this.Site !=null && ! this.Site.DesignMode  )//������ʱ
            //{
            if (AutoBindData)
            {
                if (Page.IsPostBack)
                {
                    BindResultData();
                }
            }
            //}
        }

        #endregion

        #region �����ķ���

        /// <summary>
        ///     ��ȡһ��ʵ����ѯ����
        /// </summary>
        /// <returns></returns>
        public IDataParameter GetParameter()
        {
            return DAO.GetParameter();
        }

        /// <summary>
        ///     ��ȡһ��ʵ����ѯ����
        /// </summary>
        /// <param name="paraName"></param>
        /// <param name="Value"></param>
        /// <returns></returns>
        public IDataParameter GetParameter(string paraName, object Value)
        {
            return DAO.GetParameter(paraName, Value);
        }

        /// <summary>
        ///     �����ṩ�ķ�ҳ��ѯ�Ϳؼ��ṩ�����ݷ�����Ϣ��������Դ��ȡ���ݡ�
        /// </summary>
        /// <returns></returns>
        public object GetDataSource()
        {
            if (AllCount == 0)
                AllCount = -1; //���⴦����ȡ��¼����Ϊ0ʱ�ļܹ�
            DAO.ConnectionString = ConnectionString;
            object result = DAO.ExecuteDataSet(SQLbyPaging, CommandType.Text, Parameters);
            if (AllCount == -1)
                AllCount = 0;
            if (DAO.ErrorMessage != "")
                throw new Exception(DAO.ErrorMessage + ";SQL=" + SQLbyPaging);
            return result;
            //DAO.
        }

        /// <summary>
        ///     ��ȡ�������¼����
        /// </summary>
        /// <returns></returns>
        public int GetResultDataCount()
        {
            //����һ����ͳ�Ʋ�������������ڼ������Ѿ����ڵ����⡣
            IDataParameter[] countParas = null;
            if (Parameters != null && Parameters.Length > 0)
            {
                countParas = (IDataParameter[]) Parameters.Clone();
                for (var i = 0; i < countParas.Length; i++)
                {
                    countParas[i] = DAO.GetParameter(countParas[i].ParameterName, countParas[i].Value);
                }
            }

            DAO.ConnectionString = ConnectionString;
            var count = DAO.ExecuteScalar(SQLbyCount, CommandType.Text, countParas);
            if (count != null)
                return Convert.ToInt32(count); //(int)count ��Oracle ����ʧ�ܡ�
            throw new Exception(DAO.ErrorMessage);
        }

        /// <summary>
        ///     ������Դ�ķ�ҳ���ݰ󶨵���Ŀ��ؼ��ϣ�֧��GridView
        /// </summary>
        public void BindResultData()
        {
            var BindToControlID = BindToControl;
            if (BindToControlID != null && BindToControlID != "")
            {
                if (DataControlDataBinding != null)
                {
                    DataControlDataBinding(this, new EventArgs());
                }
                //����ķ�ʽ������ؼ����û��ؼ��У������Ҳ�����
                //Control ctr= this.Page.FindControl (BindToControlID); 
                var ctr = FindMyControl(this, BindToControlID);

                if (ctr is GridView)
                {
                    ((GridView) ctr).DataSource = GetDataSource();
                    ctr.DataBind();
                }
                else if (ctr is DataGrid)
                {
                    ((DataGrid) ctr).DataSource = GetDataSource();
                    ctr.DataBind();
                }
                else if (ctr is DataList)
                {
                    ((DataList) ctr).DataSource = GetDataSource();
                    ctr.DataBind();
                }
                else if (ctr is Repeater)
                {
                    ((Repeater) ctr).DataSource = GetDataSource();
                    ctr.DataBind();
                }
                else
                {
                    throw new Exception("�ؼ�" + BindToControlID + "��֧�����ݰ󶨣���ȷ����Ŀ��ؼ���DataGrid,DataList,Repeater���ͣ�");
                }
                if (DataControlDataBound != null)
                {
                    DataControlDataBound(this, new EventArgs());
                }
            }
        }

        //      ���û��ؼ��У���Ȼ�Ҳ�����Ŀ��ؼ�
        private Control FindMyControl(Control sourceControl, string objControlID)
        {
            //������Ȳ���
            foreach (Control ctr in sourceControl.Parent.Controls)
            {
                if (ctr.ID == objControlID)
                    return ctr;
            }
            foreach (Control ctr in sourceControl.Parent.Controls)
            {
                var objCtr = FindMyControl(ctr, objControlID);
                if (objCtr != null)
                    return objCtr;
            }
            return null;
        }

        /// <summary>
        ///     ���°����ݺͼ��㱾�β�ѯ�ļ�¼���������趨��ǰҳ���ڵ�һҳ
        /// </summary>
        public void ReBindResultData()
        {
            CurrentPage = 1;
            AllCount = GetResultDataCount();
            BindResultData();
        }

        #endregion

        #region ���ݷ�ҳ����

        /// <summary>
        ///     ��Ҫ�󶨷�ҳ�Ŀؼ�����DataGrid,DataList,Repeater ��
        /// </summary>
        [DefaultValue(null),
         Category("Data"),
         Description("��Ҫ�󶨷�ҳ�Ŀؼ�����DataGrid,DataList,Repeater ��"),
         TypeConverter(typeof (ControlListIDConverter))]
        public string BindToControl { get; set; }

        /// <summary>
        ///     ���ڷ�ҳ��ѯ��ԭʼ SQL ���
        /// </summary>
        [DefaultValue(null),
         Category("Data"),
         Description("���ڷ�ҳ��ѯ��ԭʼ SQL ���")]
        public string SQL
        {
            get
            {
                if (ViewState[ID + "_SQL"] != null)
                    _SQL = (string) ViewState[ID + "_SQL"];
                return _SQL;
            }
            set
            {
                _SQL = value;
                ViewState[ID + "_SQL"] = value;
            }
        }

        /// <summary>
        ///     ��ҳ��ѯ����,������ʱ����� GetParameter���� ��ӳ�Ա��
        /// </summary>
        [DefaultValue(null),
         Category("Data"),
         Description("��ҳ��ѯ����,������ʱ����� GetParameter���� ��ӳ�Ա��")]
        public IDataParameter[] Parameters
        {
            get
            {
                if (_Parameters != null)
                    return _Parameters;
                if (HttpContext.Current.Session[ID + "_Parameters"] != null)
                {
                    var p0 = (IDataParameter[]) HttpContext.Current.Session[ID + "_Parameters"];
                    var p1 = new IDataParameter[p0.Length];
                    for (var i = 0; i < p0.Length; i++)
                    {
                        p1[i] = GetParameter(p0[i].ParameterName, p0[i].Value); //�����²���
                    }

                    return p1;
                }
                return null;
            }
            set
            {
                _Parameters = value;
                HttpContext.Current.Session[ID + "_Parameters"] = _Parameters;
            }
        }

        /// <summary>
        ///     ���ɵ����ڷ�ҳ��ѯ�� SQL ���
        /// </summary>
        [DefaultValue(null),
         Category("Data"),
         Description("���ɵ����ڷ�ҳ��ѯ�� SQL ���")]
        public string SQLbyPaging
        {
            get
            {
                if (SQL == null) return "";
                SQLPage.DbmsType = DBMSType;
                return SQLPage.MakeSQLStringByPage(SQL, Where, PageSize, CurrentPage, AllCount);
            }
        }

        /// <summary>
        ///     ���ɵ�����ͳ�Ʒ�ҳ��ѯ�ܼ�¼���� SQL ���
        /// </summary>
        [DefaultValue(null),
         Category("Data"),
         Description("���ɵ�����ͳ�Ʒ�ҳ��ѯ�ܼ�¼���� SQL ���")]
        public string SQLbyCount
        {
            get
            {
                if (SQL == null) return "";
                SQLPage.DbmsType = DBMSType;
                return SQLPage.MakeSQLStringByPage(SQL, Where, PageSize, CurrentPage, 0);
            }
        }

        /// <summary>
        ///     ָ�����ڷ�ҳ��ѯ��֧�ֵ����ݿ����ϵͳ��������
        /// </summary>
        [DefaultValue(DBMSType.SqlServer),
         Category("Data"),
         Description("ָ�����ڷ�ҳ��ѯ��֧�ֵ����ݿ����ϵͳ��������")]
        [TypeConverter(typeof (EnumConverter))]
        public DBMSType DBMSType
        {
            get { return _DBMSType; }
            set { _DBMSType = value; }
        }

        /// <summary>
        ///     �Ƿ��Զ������ݿ�ʵ����������ǣ�������DataProvider ���ݷ��ʿ飬�ܹ�����Ļ�ȡ���������ɽ�����ݼ������δ����ȷ���ã�����������ΪTrue ��
        /// </summary>
        [DefaultValue(false),
         Category("Data"),
         Description("�Ƿ��Զ������ݿ�ʵ����������ǣ�������DataProvider ���ݷ��ʿ飬�ܹ�����Ļ�ȡ���������ɽ�����ݼ������δ����ȷ���ã�����������ΪTrue ��")]
        public bool AutoIDB { get; set; }

        private void CheckAutoIDB()
        {
            if (HttpContext.Current == null) //   this.Site !=null && this.Site.DesignMode
            {
                return; //�����ʱ�˳������߼��ж�
            }
            if (AutoIDB) //����Զ�ʵ�������ݿ���ʶ���
            {
                try
                {
                    _ErrorMessage = "";
                    if (DAO == null)
                        DAO = MyDB.GetDBHelper(DBMSType, ConnectionString);
                    AutoIDB = true;
                }
                catch (Exception e)
                {
                    AutoIDB = false;
                    _ErrorMessage = e.Message;
                }
            }
        }

        /// <summary>
        ///     �Ƿ��Զ���Ӧ�ó��������ļ���ȡ���ݷ���������Ϣ��ֻ���Ѿ���ȷ����������Ϣ�ſ��Է���True ��
        /// </summary>
        [DefaultValue(false),
         Category("Data"),
         Description("�Ƿ��Զ���Ӧ�ó��������ļ���ȡ���ݷ��ʺ�����������Ϣ��ֻ���Ѿ���ȷ����������Ϣ�ſ��Է���True ��")]
        public bool AutoConfig { get; set; }

        private void CheckAutoConfig()
        {
            if (HttpContext.Current == null) //   this.Site !=null && this.Site.DesignMode
            {
                return; //�����ʱ�˳������߼��ж�
            }
            if (AutoConfig)
            {
                _ErrorMessage = "";
                var strConn = "";
                //�������ݿ����ϵͳ����
                var strDBMSType = ConfigurationSettings.AppSettings["EngineType"]; //ͳһ�� DBMSType ��ȡ

                if (strDBMSType != null && strDBMSType != "")
                {
                    if (Enum.IsDefined(typeof (DBMSType), strDBMSType))
                        DBMSType = (DBMSType) Enum.Parse(typeof (DBMSType), strDBMSType);
                    else
                        AutoConfig = false;

                    //���������ַ���
                    var ConnStrKey = string.Empty;
                    switch (DBMSType)
                    {
                        case DBMSType.Access:
                            ConnStrKey = "OleDbConnectionString";
                            break;
                        case DBMSType.SqlServer:
                            ConnStrKey = "SqlServerConnectionString";
                            break;
                        case DBMSType.Oracle:
                            ConnStrKey = "OracleConnectionString";
                            break;
                        case DBMSType.MySql:
                            ConnStrKey = "OdbcConnectionString";
                            break;
                        case DBMSType.UNKNOWN:
                            ConnStrKey = "OdbcConnectionString";
                            break;
                    }
                    strConn = ConfigurationSettings.AppSettings[ConnStrKey];
                }
                else
                {
                    //δָ���������һ��connectionStrings ���ýڶ�ȡ
                    if (ConfigurationManager.ConnectionStrings.Count > 0)
                    {
                        DAO = MyDB.GetDBHelper();
                        strConn = DAO.ConnectionString;
                    }
                }

                if (strConn == null || strConn == "")
                    AutoConfig = false;
                else
                    ConnectionString = strConn.Replace("~", Context.Request.PhysicalApplicationPath); //�滻���·��

                if (!AutoConfig) //�����ʱ�����ɴ�����Ϣ����ΪVS2003���ʱ�޷���ȡ������Ϣ
                {
                    _ErrorMessage = "δ����ȷ�������ݷ�����Ϣ�������Ƿ��Ѿ���Ӧ�ó��������ļ��н�������ȷ������";
                    AutoConfig = false;
                }
                else
                    AutoIDB = AutoConfig; //�����ȷ���ã���ô�Զ������ݿ���ʶ���ʵ��
            }
        }

        /// <summary>
        ///     �Ƿ�������ʱ�Զ��󶨷�ҳ���ݣ������� AutoIDB ���Ե���True
        /// </summary>
        [DefaultValue(false),
         Category("Data"),
         Description("�Ƿ�������ʱ�Զ��󶨷�ҳ���ݣ������� AutoIDB ���Ե���True")]
        public bool AutoBindData { get; set; }

        /// <summary>
        ///     ������Ϣ
        /// </summary>
        [DefaultValue(""),
         Category("Data"),
         Description("������Ϣ")]
        public string ErrorMessage
        {
            get { return _ErrorMessage; }
            //set{ _ErrorMessage=value;}
        }

        /// <summary>
        ///     ���ݿ������ַ���
        /// </summary>
        [DefaultValue(""),
         Category("Data"),
         Description("���ݿ������ַ���")]
        public string ConnectionString
        {
            get { return _ConnectionString; }
            set { _ConnectionString = value; }
        }

        /// <summary>
        ///     ָ����ҳ��ѯ�ĸ���������ע��򵥲�ѯ�븴�Ӳ�ѯ�������޶���ʽ��
        /// </summary>
        [DefaultValue(""),
         Category("Data"),
         Description("ָ����ҳ��ѯ�ĸ���������ע��򵥲�ѯ�븴�Ӳ�ѯ�������޶���ʽ��")]
        public string Where
        {
            get
            {
                if (ViewState[ID + "_Where"] != null)
                    _Where = (string) ViewState[ID + "_Where"];
                return _Where;
            }
            set
            {
                _Where = value;
                ViewState[ID + "_Where"] = value;
            }
        }

        /// <summary>
        ///     �����¼����Ϊ0�����������Ƿ���ʾ���ݼܹ��������Ҫ��ʾ�ܹ�����ô��ִ�����ݰ󶨷�����
        /// </summary>
        [DefaultValue(true),
         Category("Data"),
         Description("�����¼����Ϊ0�����������Ƿ���ʾ���ݼܹ��������Ҫ��ʾ�ܹ�����ô��ִ�����ݰ󶨷�����")]
        public bool ShowEmptyData
        {
            get
            {
                if (ViewState[ID + "_ShowEmptyData"] != null)
                    _ShowEmptyData = (bool) ViewState[ID + "_ShowEmptyData"];
                return _ShowEmptyData;
            }
            set
            {
                _ShowEmptyData = value;
                ViewState[ID + "_ShowEmptyData"] = value;
            }
        }

        #endregion

        #region �������صķ���

        /// <summary>
        ///     ���˿ؼ����ָ�ָ�������������
        /// </summary>
        /// <param name="output"> Ҫд������ HTML ��д�� </param>
        protected override void Render(HtmlTextWriter output)
        {
            if (ChangePageProperty)
            {
                ChangePageProperty = false;
                //this.SetPageInfo ();
            }
            SetPageInfo();
            ForeColor = ForeColor;
            EnsureChildControls();

            //�����ͷ��ʽ
            output.Write("<table width='" + Width + "' height='" + Height
                         + "' bgcolor='" + ConvertColorFormat(BackColor)
                         + "' bordercolor='" + ConvertColorFormat(BorderColor)
                         + "' border='" + BorderWidth
                         + "' style='border-style:" + BorderStyle
                         + ";border-collapse:collapse' cellpadding='0'><tr><td><table width='100%' style='color:" +
                         ConvertColorFormat(ForeColor)
                         + " ;font-size:" + FontSize + "; font-family:" + Font.Name + "' class='"
                         + CssClass + "'><tr><td valign='baseline'>"
                         + Text + "</td><td valign='baseline'>");
            //��ӿؼ�
            //1-����ʾ��¼������2-����ʾҳ��ת��3-�Ȳ���ʾ��¼������Ҳ����ʾҳ��ת

            var type = PageToolBarStyle;

            //1��3������ʾ��¼����
            if (type != 1 && type != 3)
            {
                var currSize = PageSize;
                if (PageCount == CurrentPage)
                    currSize = AllCount - PageSize*(CurrentPage - 1);

                output.Write(currSize + "/"); //AllCount-PageSize*(PageNumber-1)
                lblAllCount.RenderControl(output);
                output.Write("����");
            }

            //
            if (UserChangePageSize)
            {
                output.Write("\n");
                dlPageSize.RenderControl(output);
                output.Write("��/ҳ��");
            }
            lblCPA.RenderControl(output);
            output.Write("ҳ</td><td>");
            lnkFirstPage.RenderControl(output);
            output.Write("\n");
            lnkPrePage.RenderControl(output);
            output.Write("\n");
            lnkNextPage.RenderControl(output);
            output.Write("\n");
            lnkLastPage.RenderControl(output);

            //2����3 ����ʾҳ��ת
            if (type != 2 && type != 3)
            {
                output.Write("\n��");
                txtNavePage.RenderControl(output);
                output.Write("ҳ\n");
                lnkGo.RenderControl(output);
            }
            output.Write("</td></tr></table></td></tr></table>");
        }

        /// <summary>
        ///     ��д OnLoad �¼�
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            //			//�����ﴦ���й����ݰ�����
            //			CheckAutoConfig ();
            //			CheckAutoIDB();


            if (Site != null && Site.DesignMode) //�����ʱ
            {
                return;
            }
            if (AutoBindData)
            {
                if (!Page.IsPostBack)
                {
                    AllCount = GetResultDataCount();
                    //�����¼����Ϊ0�����������Ƿ���ʾ���ݼܹ��������Ҫ��ʾ�ܹ�����ô��ִ�����ݰ󶨷�����
                    if (!ShowEmptyData && AllCount == 0)
                        return;

                    BindResultData();

                    //this.SetPageInfo ();
                }
            }
        }


        /// <summary>
        ///     ��д CSS ��ʽ��
        /// </summary>
        public override string CssClass
        {
            get { return base.CssClass; }
            set
            {
                base.CssClass = value;
                foreach (WebControl ctr in Controls)
                {
                    ctr.CssClass = value;
                }
            }
        }

        /// <summary>
        ///     ��дǰ��ɫ
        /// </summary>
        public override Color ForeColor
        {
            get { return base.ForeColor; }
            set
            {
                base.ForeColor = value;
                foreach (WebControl ctr in Controls)
                {
                    ctr.ForeColor = value;
                }
            }
        }

        public override Color BackColor
        {
            get
            {
                if (!hasSetBgColor)
                    return Color.White;
                return base.BackColor;
            }
            set
            {
                hasSetBgColor = true;
                base.BackColor = value;
            }
        }

        /// <summary>
        ///     �����ӿؼ�
        /// </summary>
        protected override void CreateChildControls()
        {
            //base.CreateChildControls ();
            Controls.Clear();
            Controls.Add(lblAllCount);
            Controls.Add(lblCPA);
            Controls.Add(lnkFirstPage);
            Controls.Add(lnkPrePage);
            Controls.Add(lnkNextPage);
            Controls.Add(lnkLastPage);
            Controls.Add(txtNavePage);
            Controls.Add(lnkGo);

            lblAllCount.Text = AllCount.ToString();
            lnkFirstPage.Text = "��ҳ";
            lnkPrePage.Text = "��һҳ";
            lnkNextPage.Text = "��һҳ";
            lnkLastPage.Text = "βҳ";
            txtNavePage.Width = 30;
            lnkGo.Text = "Go";


            if (UserChangePageSize)
            {
                Controls.Add(dlPageSize);
                dlPageSize.AutoPostBack = true;
                dlPageSize.Items.Clear();
                for (var i = 5; i <= 50; i += 5)
                {
                    dlPageSize.Items.Add(i.ToString());
                }
                dlPageSize.SelectedValue = PageSize.ToString();
                dlPageSize.SelectedIndexChanged += dlPageSize_SelectedIndexChanged;
            }

            lnkFirstPage.Click += lnkFirstPage_Click;
            lnkPrePage.Click += lnkPrePage_Click;
            lnkNextPage.Click += lnkNextPage_Click;
            lnkLastPage.Click += lnkLastPage_Click;
            lnkGo.Click += lnkGo_Click;
        }

        #endregion

        #region �ڲ��¼�����

        /// <summary>
        ///     RGB��ɫֵ��Html��ɫֵת��
        /// </summary>
        /// <param name="RGBColor"></param>
        /// <returns></returns>
        private string ConvertColorFormat(Color RGBColor)
        {
            return "RGB(" + RGBColor.R + "," + RGBColor.G + "," + RGBColor.G + ")";
        }

        /// <summary>
        ///     ���÷�ҳ״̬��Ϣ
        /// </summary>
        private void SetPageInfo()
        {
            if (PageIndex == UNKNOW_NUM)
                PageIndex = CurrentPage;

            if (PageIndex > PageCount) PageIndex = PageCount;
            else if (PageIndex == -1) PageIndex = PageCount;
            else if (PageIndex < 1) PageIndex = 1;

            if (AllCount == 0)
                lblCPA.Text = "0/0";
            else
                lblCPA.Text = PageIndex + "/" + PageCount;
            txtNavePage.Text = PageIndex.ToString();
            CurrentPage = PageIndex;

            if (PageCount == 1) lnkGo.Enabled = false;

            if (PageIndex == 0)
            {
                lnkFirstPage.Enabled = false;
                lnkPrePage.Enabled = false;
                lnkLastPage.Enabled = false;
                lnkNextPage.Enabled = false;
                //this.btnNavePage .Enabled =false;
                return;
            }

            if (PageIndex == 1)
            {
                lnkFirstPage.Enabled = false;
                lnkPrePage.Enabled = false;
            }
            else
            {
                lnkFirstPage.Enabled = true;
                lnkPrePage.Enabled = true;
            }
            if (PageIndex < PageCount)
            {
                lnkLastPage.Enabled = true;
                lnkNextPage.Enabled = true;
            }
            else
            {
                lnkLastPage.Enabled = false;
                lnkNextPage.Enabled = false;
            }
        }

        /// <summary>
        ///     ��һҳ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lnkNextPage_Click(object sender, EventArgs e)
        {
            PageIndex = CurrentPage;
            PageIndex++;
            SetPageInfo();
            changeIndex(e);
        }

        /// <summary>
        ///     ��һҳ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lnkPrePage_Click(object sender, EventArgs e)
        {
            PageIndex = CurrentPage;
            PageIndex--;
            SetPageInfo();
            changeIndex(e);
        }

        /// <summary>
        ///     βҳ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lnkLastPage_Click(object sender, EventArgs e)
        {
            PageIndex = -1;
            SetPageInfo();
            changeIndex(e);
        }

        /// <summary>
        ///     ��ҳ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lnkFirstPage_Click(object sender, EventArgs e)
        {
            PageIndex = 1;
            SetPageInfo();
            changeIndex(e);
        }


        /// <summary>
        ///     ��ʼ������������ʽ
        /// </summary>
        private void InitStyle()
        {
            if (css_linkStyle != "")
                lnkFirstPage.Attributes.Add("class", css_linkStyle);
            if (css_linkStyle != "")
                lnkNextPage.Attributes.Add("class", css_linkStyle);
            if (css_linkStyle != "")
                lnkLastPage.Attributes.Add("class", css_linkStyle);
            if (css_linkStyle != "")
                lnkPrePage.Attributes.Add("class", css_linkStyle);
            if (css_linkStyle != "")
                lnkGo.Attributes.Add("class", css_linkStyle);
            if (css_txtStyle != "")
                txtNavePage.Attributes.Add("class", css_txtStyle);
        }

        /// <summary>
        ///     ת��ĳҳ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lnkGo_Click(object sender, EventArgs e)
        {
            try
            {
                PageIndex = Int32.Parse(txtNavePage.Text.Trim());
                SetPageInfo();
                changeIndex(e);
            }
            catch
            {
                Page.ClientScript.RegisterStartupScript(GetType(), "pageErr",
                    "<script language='javascript'>alert('����д����ҳ�룡');</script>");
            }
        }

        /// <summary>
        ///     �ı�ҳ��С
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dlPageSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            PageSize = int.Parse(dlPageSize.SelectedValue);
            CurrentPage = 1;
            PageIndex = CurrentPage;
            SetPageInfo();
            BindResultData();
        }

        #endregion

        #region ��ȡ�󶨿ؼ��б� ��

        /// <summary>
        ///     ��ȡ�󶨿ؼ��б� ��
        /// </summary>
        public class ControlListIDConverter : StringConverter
        {
            /// <summary>
            ///     false
            /// </summary>
            public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
            {
                return false;
            }

            /// <summary>
            ///     true
            /// </summary>
            public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
            {
                return true;
            }

            /// <summary>
            ///     ��ȡ��������ʱ��Ŀ��ؼ���ID
            /// </summary>
            public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
            {
                if (context == null)
                    return null;
                var al = new ArrayList();
                foreach (IComponent ic in context.Container.Components)
                {
                    if (ic is ProPageToolBar)
                        continue;
                    if (ic is DataGrid || ic is Repeater || ic is DataList || ic is GridView)
                        //|| ic is System.Web.UI.WebControls.ListView ������Ҫ3.5���
                    {
                        al.Add(((Control) ic).ID);
                    }
                }
                return new StandardValuesCollection(al);
            }
        }

        public DBMSType DBMSType1
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        public SQLPage SQLPage
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        public DataBoundHandler DataBoundHandler
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        public ClickEventHandler ClickEventHandler
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        //		public class DBMSConverter:TypeConverter
        //		{
        //			public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        //			{
        //				if(sourceType==typeof(string ))
        //				{
        //					return true;
        //				}
        //				return base.CanConvertFrom (context, sourceType);
        //			}
        //
        //			public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        //			{
        //				if( value.GetType() == typeof(string) )
        //				{
        //					DBMSType dbms=DBMSType.UNKNOWN  ;
        //					if(System.Enum.IsDefined (typeof(DBMSType),value) )
        //					{
        //						dbms=(DBMSType)System.Enum.Parse (typeof(DBMSType),value.ToString (),false); 
        //					}
        //					return dbms;
        //				}
        //				else
        //					return base.ConvertFrom(context, culture, value);
        //
        //			}
        //
        //			/// <summary>
        //			/// true
        //			/// </summary>
        //			public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        //			{
        //				return true;
        //			}
        //
        //			public override System.ComponentModel.TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        //			{
        //				if(context==null)
        //					return null;
        //				string[] dbmsTypeNames=System.Enum.GetNames (typeof(DBMSType ));
        //				return new TypeConverter.StandardValuesCollection(dbmsTypeNames);
        // 			}
        //
        //		}

        #endregion
    }
}