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
using System.Data;
using System.Data.Common;
using System.Data.OleDb;
using System.IO;
using PWMIS.Common;

namespace PWMIS.DataProvider.Data
{
    /// <summary>
    ///     OleDbServer ���ݴ���
    /// </summary>
    public class OleDb : AdoHelper
    {
        /// <summary>
        ///     ��ȡ��ǰ���ݿ����͵�ö��
        /// </summary>
        public override DBMSType CurrentDBMSType
        {
            get { return DBMSType.UNKNOWN; }
        }

        public override DbConnectionStringBuilder ConnectionStringBuilder
        {
            get { return new OleDbConnectionStringBuilder(ConnectionString); }
        }

        public override string ConnectionUserID
        {
            get
            {
                if (ConnectionStringBuilder.ContainsKey("User ID"))
                    return ConnectionStringBuilder["User ID"].ToString();
                return "";
            }
        }

        /// <summary>
        ///     ��ȡ��ǰ�����ַ����е�����Դ�ַ����������|DataDirectory|������������Դ�ļ���Ӧ�ľ���·����
        /// </summary>
        public string ConnectionDataSource
        {
            get
            {
                if (ConnectionStringBuilder.ContainsKey("Data Source"))
                {
                    var path = ConnectionStringBuilder["Data Source"].ToString();
                    if (path.StartsWith("|DataDirectory|", StringComparison.OrdinalIgnoreCase))
                    {
                        var obj = AppDomain.CurrentDomain.GetData("DataDirectory");
                        if (obj == null)
                            throw new InvalidOperationException("��ǰӦ�ó�����δ���� DataDirectory");
                        var dataPath = obj.ToString();
                        var fileName = path.Substring("|DataDirectory|".Length);
                        var dbFilePath = Path.Combine(dataPath, fileName);
                        return dbFilePath;
                    }
                    return path;
                }
                return null;
            }
        }

        /// <summary>
        ///     �������Ҵ����ݿ�����
        /// </summary>
        /// <returns>���ݿ�����</returns>
        protected override IDbConnection GetConnection()
        {
            var conn = base.GetConnection();
            if (conn == null)
            {
                conn = new OleDbConnection(ConnectionString);
                //conn.Open ();
            }
            return conn;
        }

        /// <summary>
        ///     ��ȡ����������ʵ��
        /// </summary>
        /// <returns>����������</returns>
        protected override IDbDataAdapter GetDataAdapter(IDbCommand command)
        {
            IDbDataAdapter ada = new OleDbDataAdapter((OleDbCommand) command);
            return ada;
        }

        /// <summary>
        ///     ��ȡһ���²�������
        /// </summary>
        /// <returns>�ض�������Դ�Ĳ�������</returns>
        public override IDataParameter GetParameter()
        {
            return new OleDbParameter();
        }

        public override string GetNativeDbTypeName(IDataParameter para)
        {
            return ((OleDbParameter) para).OleDbType.ToString();
        }

        /// <summary>
        ///     ��ȡһ���²�������
        /// </summary>
        /// <param name="paraName">������</param>
        /// <param name="dbType">������������</param>
        /// <param name="size">������С</param>
        /// <returns>�ض�������Դ�Ĳ�������</returns>
        public override IDataParameter GetParameter(string paraName, DbType dbType, int size)
        {
            var para = new OleDbParameter();
            para.ParameterName = paraName;
            para.DbType = dbType;
            para.Size = size;
            return para;
        }

        /// <summary>
        ///     ���ش� OleDbConnection ������Դ�ļܹ���Ϣ��
        /// </summary>
        /// <param name="collectionName">��������</param>
        /// <param name="restrictionValues">����ļܹ���һ������ֵ</param>
        /// <returns>���ݿ�ܹ���Ϣ��</returns>
        public override DataTable GetSchema(string collectionName, string[] restrictionValues)
        {
            using (var conn = (OleDbConnection) GetConnection())
            {
                conn.Open();
                if (restrictionValues == null && string.IsNullOrEmpty(collectionName))
                    return conn.GetSchema();
                if (restrictionValues == null && !string.IsNullOrEmpty(collectionName))
                    return conn.GetSchema(collectionName);
                return conn.GetSchema(collectionName, restrictionValues);
            }
        }
    }
}