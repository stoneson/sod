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

using System.Data;

namespace PWMIS.DataForms.Adapter
{
    /// <summary>
    ///     ���ܴ����������ʹ�øö�ǰ����ȷ����Ӧ�����ݱ��������Ͳ���ʱ���������
    /// </summary>
    public class IBCommand
    {
        private string _deleteCmd = string.Empty;
        private string _guidpk = string.Empty;
        private string _insertCmd = string.Empty;
        private string _selectCmd = string.Empty;
        private string _tableName = string.Empty;
        private string _updateCmd = string.Empty;

        /// <summary>
        ///     Ĭ�Ϲ��캯��
        /// </summary>
        public IBCommand()
        {
            InsertedID = 0;
        }

        /// <summary>
        ///     ָ��һ�����ݱ��ʼ������
        /// </summary>
        /// <param name="tableName"></param>
        public IBCommand(string tableName)
        {
            InsertedID = 0;
            _tableName = tableName;
        }

        /// <summary>
        ///     ������������
        /// </summary>
        public string InsertCommand
        {
            get { return _insertCmd; }
            set { _insertCmd = value; }
        }

        /// <summary>
        ///     ������������
        /// </summary>
        public string UpdateCommand
        {
            get { return _updateCmd; }
            set { _updateCmd = value; }
        }

        /// <summary>
        ///     ѡ����������
        /// </summary>
        public string SelectCommand
        {
            get { return _selectCmd; }
            set { _selectCmd = value; }
        }

        /// <summary>
        ///     ɾ����������
        /// </summary>
        public string DeleteCommand
        {
            get { return _deleteCmd; }
            set { _deleteCmd = value; }
        }

        /// <summary>
        ///     ������
        /// </summary>
        public string TableName
        {
            get { return _tableName; }
            set { _tableName = value; }
        }

        /// <summary>
        ///     �����ʶ���������ݿ�������У�����0��ʾ��δ���룬����0��ʾ�Ѿ����������������ʶֵ������-2��ʾ���������͵�������
        /// </summary>
        public int InsertedID { get; set; }

        /// <summary>
        ///     GUID ��������
        /// </summary>
        public string GuidPrimaryKey
        {
            get { return _guidpk; }
            set { _guidpk = value; }
        }

        /// <summary>
        ///     ��Ӧ�Ĳ�ѯ��������
        /// </summary>
        public IDataParameter[] Parameters { get; set; }
    }
}