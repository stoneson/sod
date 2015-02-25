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
using System.Data.Common;
using System.Data.Odbc;
using PWMIS.Common;

namespace PWMIS.DataProvider.Data
{
    /// <summary>
    ///     ODBC ���ݷ�����
    /// </summary>
    public sealed class Odbc : AdoHelper
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
            get { return new OdbcConnectionStringBuilder(ConnectionString); }
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
        ///     �������Ҵ����ݿ�����
        /// </summary>
        /// <returns>���ݿ�����</returns>
        protected override IDbConnection GetConnection()
        {
            var conn = base.GetConnection();
            if (conn == null)
            {
                conn = new OdbcConnection(ConnectionString);
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
            IDbDataAdapter ada = new OdbcDataAdapter((OdbcCommand) command);
            return ada;
        }

        /// <summary>
        ///     ��ȡһ���²�������
        /// </summary>
        /// <returns>�ض�������Դ�Ĳ�������</returns>
        public override IDataParameter GetParameter()
        {
            return new OdbcParameter();
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
            var para = new OdbcParameter();
            para.ParameterName = paraName;
            para.DbType = dbType;
            para.Size = size;
            return para;
        }

        public override string GetNativeDbTypeName(IDataParameter para)
        {
            return para.DbType.ToString();
        }

        /// <summary>
        ///     ���ش� OdbcConnection ������Դ�ļܹ���Ϣ��
        /// </summary>
        /// <param name="collectionName">��������</param>
        /// <param name="restrictionValues">����ļܹ���һ������ֵ</param>
        /// <returns>���ݿ�ܹ���Ϣ��</returns>
        public override DataTable GetSchema(string collectionName, string[] restrictionValues)
        {
            using (var conn = (OdbcConnection) GetConnection())
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