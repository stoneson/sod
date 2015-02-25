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
 * �޸��ߣ�         ʱ�䣺2012-11-6                
 * �޸�˵��������Oracle������ǰ׺
 * ========================================================================
*/

using System.Data;
using System.Data.Common;
using System.Data.OracleClient;
using PWMIS.Common;

namespace PWMIS.DataProvider.Data
{
    /// <summary>
    ///     OracleServer ���ݴ���
    /// </summary>
    public sealed class Oracle : AdoHelper
    {
        /// <summary>
        ///     ��ȡ��ǰ���ݿ����͵�ö��
        /// </summary>
        public override DBMSType CurrentDBMSType
        {
            get { return DBMSType.Oracle; }
        }

        public override DbConnectionStringBuilder ConnectionStringBuilder
        {
            get { return new OracleConnectionStringBuilder(ConnectionString); }
        }

        public override string ConnectionUserID
        {
            get { return ((OracleConnectionStringBuilder) ConnectionStringBuilder).UserID; }
        }

        /// <summary>
        ///     ��ȡOracle���ݿ����ǰ׺�ʷ�
        ///     <remarks>������·�˼�.aspx ����</remarks>
        /// </summary>
        public override string GetParameterChar
        {
            get { return ":"; }
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
                conn = new OracleConnection(ConnectionString);
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
            IDbDataAdapter ada = new OracleDataAdapter((OracleCommand) command);
            return ada;
        }

        /// <summary>
        ///     ��ȡһ���²�������
        /// </summary>
        /// <returns>�ض�������Դ�Ĳ�������</returns>
        public override IDataParameter GetParameter()
        {
            return new OracleParameter();
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
            var para = new OracleParameter();
            para.ParameterName = paraName;
            para.DbType = dbType;
            para.Size = size;
            return para;
        }

        public override string GetNativeDbTypeName(IDataParameter para)
        {
            return ((OracleParameter) para).OracleType.ToString();
        }

        /// <summary>
        ///     ���ش� OracleConnection ������Դ�ļܹ���Ϣ��
        /// </summary>
        /// <param name="collectionName">��������</param>
        /// <param name="restrictionValues">����ļܹ���һ������ֵ</param>
        /// <returns>���ݿ�ܹ���Ϣ��</returns>
        public override DataTable GetSchema(string collectionName, string[] restrictionValues)
        {
            using (var conn = (OracleConnection) GetConnection())
            {
                conn.Open();
                if (restrictionValues == null && string.IsNullOrEmpty(collectionName))
                    return conn.GetSchema();
                if (restrictionValues == null && !string.IsNullOrEmpty(collectionName))
                    return conn.GetSchema(collectionName);
                return conn.GetSchema(collectionName, restrictionValues);
            }
        }

        /// <summary>
        ///     Ԥ����SQL��䣬����в��ܰ���"["��"]"���������ţ������Ҫ����ʹ�ò�������ѯ��
        /// </summary>
        /// <param name="SQL"></param>
        /// <returns></returns>
        protected override string PrepareSQL(ref string SQL)
        {
            return SQL.Replace("[", "\"").Replace("]", "\"").Replace("@", ":");
        }
    }
}