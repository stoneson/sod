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
 * �޸��ߣ�         ʱ�䣺2012-11-1                
 * �޸�˵�����ռ����ݵ�ʱ�򣬸Ľ���SQLite��֧��
 * ========================================================================
*/
using System;
using System.Data ;
using System.Collections ;
using System.Collections.Generic;
using System.Web;
using System.Web.UI ;
using PWMIS.DataProvider.Data ;
using PWMIS.Common;

namespace PWMIS.DataProvider.Adapter
{
    /// <summary>
    /// �û�ʹ�����ݿؼ����Զ��巽��ί��
    /// </summary>
    /// <param name="dataControl"></param>
    public delegate void  UseDataControl( IDataControl dataControl);

	/// <summary>
	/// ����Web�������ݴ����࣬���������ռ������ݳ־û������浽���ݿ⣩�ȷ��������ʹ����������ʹ�ø����м�ľ�̬������
	/// </summary>
	public class MyWebForm
	{
		private bool _CheckRowUpdateCount=false;//�Ƿ�����½����Ӱ��������������飬��ô��Ӱ����в�����0���׳�����
		private CommonDB _dao=null;
		private static MyWebForm  _instance=null;
		/// <summary>
		/// Ĭ�Ϲ��캯��
		/// </summary>
		public MyWebForm()
		{
			//
			// TODO: �ڴ˴���ӹ��캯���߼�
			//
		}

		/// <summary>
		/// �Ƿ�����½����Ӱ��������������飬��ô��Ӱ����в�����0���׳�����
		/// </summary>
		public bool CheckAffectRowCount
		{
			get
			{
				return _CheckRowUpdateCount;
			}
			set
			{
				_CheckRowUpdateCount=value;
			}
		}

		/// <summary>
		/// ����Web�������ݴ����� �ľ�̬ʵ��
		/// </summary>
		public static  MyWebForm Instance
		{
			get
			{
				if(_instance==null)
					_instance=new MyWebForm ();
				return _instance;
			}
		}

		/// <summary>
		/// ��ȡ�����������ݷ��ʶ���Ĭ��ʹ�þ�̬ CommonDB ���ʵ������
		/// ���ʹ���������в������ʣ������� CommonDB �Ķ�̬ʵ�������磺MyDB.GetDBHelper();
		/// </summary>
		public CommonDB DAO
		{
			get
			{
				if(_dao==null)
					_dao=MyDB.Instance ;
				return _dao;
			}
			set
			{
				_dao=value;
			}
		}

        public IBCommand IBCommand
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }

        public MyDB MyDB
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }

        public  IDataControl IDataControl
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }

		/// <summary>
		/// ��������ϵ����ܿؼ���ֵ
		/// </summary>
		/// <param name="Controls">����ؼ�����</param>
		public static void ClearIBData(ControlCollection Controls)
		{
            //ʹ������ί��
            UseDataControl clearData = delegate(IDataControl dataControl) 
            { 
                dataControl.SetValue(""); 
            };
            DoDataControls(Controls, clearData);

		}

        /// <summary>
        /// ʹ���Զ���ķ�������ؼ�������ÿһ�����ܴ������ݿؼ���ʹ�û����ض�������ؼ����ϡ�
        /// </summary>
        /// <param name="controls">���������ؼ��Ŀؼ�����</param>
        /// <param name="useMethod">�Զ���ķ���</param>
        public static void DoDataControls(ControlCollection controls,UseDataControl useMethod)
        {
            foreach (IDataControl item in GetIBControls(controls))
                useMethod(item);
        }

        /// <summary>
        /// �ӿؼ����ϵ�ÿ��Ԫ�ؼ�����Ԫ����Ѱ�����е��������ݿؼ����������ܿؼ��б�
        /// </summary>
        /// <param name="controls">�ؼ����ϣ���ҳ����������</param>
        /// <returns>���ܿؼ��б�</returns>
        public static List<IDataControl>  GetIBControls(ControlCollection controls)
        {
            List<IDataControl> IBControls = new List<IDataControl>();
            findIBControls(IBControls, controls);
            return IBControls;
        }

		#region ҳ�������ռ�

		/// <summary>
		/// Ѱ�����ܿؼ�������ŵ������б���
		/// </summary>
		/// <param name="arrIBs">��ſؼ�������</param>
		/// <param name="controls">ҪѰ�ҵ�ԭ�ؼ�����</param>
		private static void findIBControls(List<IDataControl > arrIBs,ControlCollection controls)
		{
			foreach(Control ctr in controls)
			{
				if(ctr is  IDataControl )
				{
					arrIBs.Add (ctr as IDataControl );
				}
				if(ctr.HasControls() )
				{
					findIBControls(arrIBs,ctr.Controls );
				}
			}
		
		}

		/// <summary>
		/// ��ȡѡ���ɾ����ѯ��SQL���
		/// </summary>
		/// <param name="IBControls">�Ѿ��ռ��Ŀؼ�����</param>
		/// <returns> ArrayList �еĳ�ԱΪ IBCommand ���󣬰��������CRUD SQL</returns>
        public static List<IBCommand> GetSelectAndDeleteCommand(List<IDataControl> IBControls)
		{
            List<IBCommand> IBCommands = new List<IBCommand>();
			//��ȡ���е�CRUD ��䡣
			while(IBControls.Count >0)
			{
				string strTableName="";
				string strSelect="";
				string strDelete="";
				string strFields="";
				//string strValues="";
				
				string strCondition="";
				int nullCount=0;
				

				for(int i=0;i<IBControls.Count ;i++)// object objCtr in IBControls)
				{
					object objCtr=IBControls[i];
					if(objCtr!=null)
					{
						 IDataControl ibCtr =objCtr as  IDataControl;
						//ֻ�з�ֻ���Ŀؼ��ſ��Ը������ݵ����ݿ�
						if(ibCtr!=null )
						{
							if(strTableName=="")
							{
								strTableName=ibCtr.LinkObject;
								strSelect="SELECT ";
								strDelete="DELETE FROM "+strTableName;
								
							}
							//�ҵ���ǰ����ı�ֻ�����ֶ�Ҳ���Դ���
							if(strTableName==ibCtr.LinkObject && ibCtr.LinkObject!="" )
							{
								string cValue=ibCtr.GetValue ().ToString ().Replace ("'","''");
								if(ibCtr.PrimaryKey )
								{
									if(cValue!="")
									{
										//��ֹSQLע��ʽ����
										cValue=(ibCtr.SysTypeCode==System.TypeCode.String || ibCtr.SysTypeCode==System.TypeCode.DateTime  ? "'"+ cValue +"'":Convert.ToDouble (cValue).ToString ());
									}
									strCondition+=" And "+ibCtr.LinkProperty+"="+cValue;
								}
                                string temp = ibCtr.LinkProperty + ",";
                                if (temp.Trim() != "," && strFields.IndexOf(temp) == -1)
                                    strFields += temp;

                                IBControls[i] = null;
							}
                            if(ibCtr.LinkObject=="" || ibCtr.LinkProperty =="")
                                IBControls[i] = null;
							
						}
					}
					else
						nullCount++;
					
				}//end for

                if (strFields == "")
                    break;

				strSelect+=strFields.TrimEnd (',')+" FROM "+strTableName+" WHERE 1=1 "+strCondition;
				strDelete+=" WHERE 1=1 "+strCondition;

				IBCommand ibcmd=new IBCommand (strTableName);
				ibcmd.SelectCommand =strSelect ;
				ibcmd.DeleteCommand =strDelete ;
				
				IBCommands.Add (ibcmd);

                if (nullCount >= IBControls.Count - 1)
                    break;
			}//end while

			return IBCommands;
		}

		/// <summary>
		/// ��ȡѡ���ɾ����ѯ��SQL���
		/// </summary>
		/// <param name="Controls">Ҫ�ռ��Ŀؼ�����</param>
		/// <returns> ArrayList �еĳ�ԱΪ IBCommand ���󣬰��������CRUD SQL</returns>
		public static List<IBCommand> GetSelectAndDeleteCommand(ControlCollection Controls)
		{
            List<IDataControl> IBControls = new List<IDataControl>();
			findIBControls(IBControls ,Controls );
			return GetSelectAndDeleteCommand(IBControls);
		}

		/// <summary>
		/// �ռ������е����ܿؼ�����ϳ��ܹ�ֱ���������ݿ����͸��� ��ѯ�� SQL���
		/// һ�������п���ͬʱ������������ݲ���
        /// ����ؼ���������������Ϊֻ������ô�ÿؼ���ֵ������µ����ݿ⣻����ÿؼ���������������Ϊ��������ô������佫����������
		/// ��̫�� 2008.1.15
		/// </summary>
		/// <returns>
		/// ArrayList �еĳ�ԱΪ IBCommand ���󣬰��������CRUD SQL
		///</returns>
        public static List<IBCommand> GetIBFormData(ControlCollection Controls,CommonDB DB)
		{
            List<IDataControl> IBControls = new List<IDataControl>();
			findIBControls(IBControls ,Controls );

            List<IBCommand> IBCommands = new List<IBCommand>();
			//��ȡ���е�CRUD ��䡣
			while(IBControls.Count >0)
			{
				string strTableName="";
				string strInsert="";
				string strFields="";
				string strValues="";
				string strUpdate="";
				string strCondition="";
				int nullCount=0;
				int ID=-1;

				for(int i=0;i<IBControls.Count ;i++)// object objCtr in IBControls)
				{
					object objCtr=IBControls[i];
					if(objCtr!=null)
					{
						 IDataControl ibCtr =objCtr as  IDataControl;
						//ֻ�з�ֻ���Ŀؼ��ſ��Ը������ݵ����ݿ�
						if(ibCtr!=null )
						{
							if(strTableName=="" && ibCtr.LinkObject!="")
							{
								strTableName=ibCtr.LinkObject;
								strInsert="INSERT INTO "+strTableName+"(";
								strUpdate="UPDATE "+strTableName+" SET ";
							}
							//�ҵ���ǰ����ı�ֻ�з�ֻ�����ֶο��Ը���
							if(strTableName==ibCtr.LinkObject && ibCtr.LinkProperty!="" )
							{
								string cValue=ibCtr.GetValue ().ToString ().Replace ("'","''");
								
								//dth,2008.4.11 �����ַ�������Ϊ�յ����
								//��ֹSQLע��ʽ����
								//�����Ƿ�Ϊ�ն������ַ������Ͳ���
								if(ibCtr.SysTypeCode==System.TypeCode.String || ibCtr.SysTypeCode==System.TypeCode.Empty)
								{
									cValue="'"+ cValue +"'";
								}
								else
								{
									if(cValue!="")
									{
										if(ibCtr.SysTypeCode==System.TypeCode.Boolean )
											cValue=(cValue.ToUpper ()=="TRUE"?"1":"0");
										else if(ibCtr.SysTypeCode==System.TypeCode.DateTime )
                                        {
                                            if (DB.CurrentDBMSType == DBMSType.SQLite)
                                                cValue = "'" + DateTime.Parse(cValue).ToString("s") + "'";
                                            else
                                                cValue = "'" + cValue + "'";//SQL SERVER ���ڸ�ʽ
                                        
                                        }
										else if(ibCtr.SysTypeCode==System.TypeCode.DBNull )
										{
											cValue="NULL";
										}
										else if(ibCtr.SysTypeCode==System.TypeCode.Object )
										{
											//Object ��ǲ����κδ����������ʹ�����ֵ��һ��ȡ����ֵ
											
										}
										else if(!(ibCtr.SysTypeCode==System.TypeCode.String || ibCtr.SysTypeCode==System.TypeCode.Empty))
										{
											//��������ַ�����ô��ͼ��������ת��
											cValue=Convert.ToDouble (cValue).ToString ();
										}

									}
								}
								
								//��ֻ�������ݲŸ���
								if(cValue!="")
								{
                                    //2010.1.25 ȡ�� ibCtr.PrimaryKey ���ܸ��µ����ƣ����������GUID�����п��Ը���
                                    //����������У����ø��еĿؼ�����Ϊ ֻ�����Լ��ɡ�
                                    if (!ibCtr.ReadOnly) 
									{
										strFields+=ibCtr.LinkProperty +",";
										strValues+=cValue+",";
										strUpdate+=ibCtr.LinkProperty+"="+cValue+",";
									}
                                    if (ibCtr.PrimaryKey) //ֻҪ����������Ϊ���µ�����
                                    {
                                        strCondition += " And " + ibCtr.LinkProperty + "=" + cValue;
                                        if (ibCtr.SysTypeCode == System.TypeCode.Int32)
                                            ID = int.Parse(cValue);
                                        else
                                            ID = -2;//���������Ƿ�������
                                       
                                    }
								}
								

							}
							IBControls[i]=null;
						}
					}
					else
						nullCount++;
					
				}//end for

				if(nullCount>=IBControls.Count-1)
					break;

				strInsert+=strFields.TrimEnd (',')+") VALUES ("+strValues.TrimEnd (',')+")";
				strUpdate=strUpdate.TrimEnd (',')+" WHERE 1=1 "+strCondition;

				IBCommand ibcmd=new IBCommand (strTableName);
				ibcmd.InsertCommand=strInsert ;
				ibcmd.UpdateCommand =strUpdate ;
                //if( ID>0) 
                    ibcmd.InsertedID =ID; 
				IBCommands.Add (ibcmd);
			}//end while

			return IBCommands;
			
		}

		#endregion

		#region ��������Լ��־û�����

	
        /// <summary>
        /// �Զ����´�������
        /// </summary>
        /// <param name="Controls">�ؼ�����</param>
        /// <returns></returns>
        public List<IBCommand> AutoUpdateIBFormData(ControlCollection Controls)
		{
			List<IBCommand > ibCommandList=GetIBFormData(Controls,this.DAO);
			int result=0;
			foreach(object item in ibCommandList)
			{
				IBCommand command=(IBCommand)item;
				if(command.InsertedID >0)
					result=DAO.ExecuteNonQuery (command.UpdateCommand );
                else if (command.InsertedID ==-2)
                {
                    result = DAO.ExecuteNonQuery(command.UpdateCommand);
                    if (result <= 0)
                        result = DAO.ExecuteNonQuery(command.InsertCommand);
                }
				else
				{
					object id=0;
					result=DAO.ExecuteInsertQuery (command.InsertCommand,ref id );
					command.InsertedID=Convert.ToInt32 (id);
				}
				if(result<=0 && _CheckRowUpdateCount)
					throw new Exception ("�ڸ��±�"+command.TableName +"��δȡ����Ӱ������������ݴ�����Ϣ��"+DAO.ErrorMessage );
					
			}
			return ibCommandList;
		}

        /// <summary>
        /// �Զ����º���GUID�������ַ��������Ĵ������ݣ�ע�ÿؼ���������PrimaryKey����
        /// </summary>
        /// <param name="Controls">�ؼ�����</param>
        /// <param name="guidControl">Gudi���ַ��������ؼ�</param>
        /// <returns>�����Ƿ�ɹ�</returns>
        public bool  AutoUpdateIBFormData(ControlCollection Controls,  IDataControl guidControl)
        {
            object  guidObj=guidControl.GetValue();
            if (guidObj == null || guidObj.ToString() == "")
                throw new Exception("GUID �������ַ��������и������ݲ���Ϊ�գ�");
            if (guidControl.ReadOnly  )
                throw new Exception("GUID �������ַ��������и�������ʱ��������Ϊֻ����");
            if (!guidControl.PrimaryKey )
                throw new Exception("GUID �������ַ��������и�������ʱ��������PrimaryKey���ԣ�");

            string guidText = guidObj.ToString();
            List<IBCommand> ibCommandList = GetIBFormData(Controls,this.DAO);
            int result = 0;
            foreach (IBCommand command in ibCommandList)
            {
                if (command.TableName == guidControl.LinkObject)
                {
                    string sql = "select " + guidControl.LinkProperty + " from " + guidControl.LinkObject + " where " + guidControl.LinkProperty + "='" + guidText + "'";
                    object guidInDb = DAO.ExecuteScalar(sql);
                    if (guidInDb != null && guidInDb.ToString() == guidText)
                    {
                        //�����ݿ����иü�¼
                        result = DAO.ExecuteNonQuery(command.UpdateCommand);
                    }
                    else
                    {
                        result = DAO.ExecuteNonQuery(command.InsertCommand );
                    }
                    return result>0 ;
                }

            }
            return false;
        }

		/// <summary>
		/// �Զ�������ܴ���ؼ�������
		/// </summary>
		/// <param name="Controls">Ҫ���Ĵ���ؼ�����</param>
		public void AutoSelectIBForm(ControlCollection Controls)
		{
            List<IDataControl> IBControls = new List<IDataControl>();
			findIBControls(IBControls ,Controls );

            List<IDataControl> IBControls2 = new List<IDataControl>();
            foreach (IDataControl obj in IBControls)
			{
				IBControls2.Add (obj);
			}
			//IBControls2 ���ᱻ���
			List<IBCommand> ibCommandList=GetSelectAndDeleteCommand(IBControls2);

            foreach (IBCommand command in ibCommandList)
			{
				IDataReader reader=DAO.ExecuteDataReaderWithSingleRow (command.SelectCommand );
				if(reader!=null && reader.Read ())
				{
					foreach(object obj in IBControls)
					{
						 IDataControl ibCtr =obj as  IDataControl;
						if(ibCtr.LinkObject ==command.TableName )
						{
							for(int i=0;i<reader.FieldCount ;i++)
							{
								if(reader.GetName(i)==ibCtr.LinkProperty )
								{
									ibCtr.SetValue (reader[i]);
									break;
								}
							}
						}
					}
				}
				//Ӧ���ڴ˴��ر��Ķ����������������г���Command����æ������
				reader.Close ();
			}
		}

        /// <summary>
        /// �����ݼ�DataSet������ݵ����ݿؼ����棬DataSet�еı����Ʊ�������ݿؼ���LinkObjectƥ�䣨�����ִ�Сд��
        /// </summary>
        /// <param name="Controls">Ҫ���Ĵ���ؼ�����</param>
        /// <param name="dsSource">�ṩ����Դ�����ݼ�</param>
        public void AutoSelectIBForm(ControlCollection Controls,DataSet dsSource)
        {
            List<IDataControl> IBControls = new List<IDataControl>();
            findIBControls(IBControls, Controls);

            foreach (DataTable dt in dsSource .Tables )
            {
                string tableName=dt.TableName;
                foreach (object obj in IBControls)
                    {
                         IDataControl ibCtr = obj as  IDataControl;
                        if (string.Compare( ibCtr.LinkObject, tableName,true )==0)
                        {
                            for (int i = 0; i < dt.Columns.Count ; i++)
                            {
                                if (string.Compare (dt.Columns[i].ColumnName,ibCtr.LinkProperty,true )==0 )
                                {
                                    ibCtr.SetValue(dt.Rows[0][i]);
                                    break;
                                }
                            }
                        }
                    }
            }

        }

        /// <summary>
        /// ��ʵ����������ݵ�ҳ��ؼ�
        /// </summary>
        /// <param name="Controls"></param>
        /// <param name="entity"></param>
        public void AutoSelectIBForm(ControlCollection Controls,  IEntity entity)
        {
            List<IDataControl> IBControls = new List<IDataControl>();
            findIBControls(IBControls, Controls);

            foreach (object obj in IBControls)
            {
                 IDataControl ibCtr = obj as  IDataControl;
                foreach (string key in entity.PropertyNames )
                {
                    if (string.Compare(key, ibCtr.LinkProperty, true) == 0)
                    {
                        ibCtr.SetValue(entity.PropertyList(key));
                        break;
                    }
                }
                
            }

        }


		/// <summary>
        /// �Զ�ɾ�����ܴ���ؼ��ĳ־û�����
		/// </summary>
        /// <param name="Controls">Ҫ����Ĵ���ؼ�����</param>
		/// <returns>������Ӱ��ļ�¼����</returns>
		public int AutoDeleteIBForm(ControlCollection Controls)
		{
            List<IDataControl> IBControls = new List<IDataControl>();
			findIBControls(IBControls ,Controls );

            List<IDataControl> IBControls2 = new List<IDataControl>();
            foreach (IDataControl obj in IBControls)
			{
				IBControls2.Add (obj);
			}
			//IBControls2 ���ᱻ���
			List<IBCommand> ibCommandList=GetSelectAndDeleteCommand(IBControls2);
            int count = 0;
            foreach (IBCommand command in ibCommandList)
			{
                count+=DAO.ExecuteNonQuery(command.DeleteCommand);
			}
            return count;
			
		}

		#endregion
	}
}
