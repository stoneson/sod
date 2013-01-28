﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Changes to this file will be lost if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
namespace FileSystem
{
	using PermissionControlSystem;
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using UserSystem;

	public interface IFileSystem  : IResource
	{
		bool IsDir { get;set; }

		DateTime CreateTime { get;set; }

		DateTime UpdateTime { get;set; }

		User CreatedWithUser { get;set; }

		User UpdatedWithUser { get;set; }

		/// <summary>
		/// 创建文件或者目录
		/// </summary>
		bool Create();

		/// <summary>
		/// 读取文件或者目录的信息（不是内容）
		/// </summary>
		bool Read();

		/// <summary>
		/// 修改文件或者目录的内容
		/// </summary>
		bool Update();

		/// <summary>
		/// 删除文件或者目录
		/// </summary>
		bool Delete();

	}
}

