@echo off
echo PDF.NET.MSF Nuget ����ļ�ǰ�����ļ� ���������
copy ..\PWMISServiceClient\bin\Debug\PWMIS.EnterpriseFramework.Common.xml  MSF-Client-NuGetPkg\lib\net40\
copy ..\PWMISServiceClient\bin\Debug\PWMIS.EnterpriseFramework.Message.PublishService.xml  MSF-Client-NuGetPkg\lib\net40\
copy ..\PWMISServiceClient\bin\Debug\PWMIS.EnterpriseFramework.Message.SubscriberLib.xml  MSF-Client-NuGetPkg\lib\net40\
copy ..\PWMISServiceClient\bin\Debug\PWMIS.EnterpriseFramework.Service.Basic.xml  MSF-Client-NuGetPkg\lib\net40\
copy ..\PWMISServiceClient\bin\Debug\PWMIS.EnterpriseFramework.Service.Client.xml  MSF-Client-NuGetPkg\lib\net40\

copy ..\PWMISServiceHost\bin\Debug\PWMIS.EnterpriseFramework.Service.Group.*  MSF-Server-Group-NuGetPkg\lib\

copy ..\PWMISServiceHost\bin\Debug\PWMIS.EnterpriseFramework.Service.Runtime.* MSF-Server-NuGetPkg\lib\

copy ..\PWMISServiceHost\bin\Debug\PWMIS.EnterpriseFramework.IOC.xml  PWMIS.EnterpriseFramework-NugetPkg\lib\
copy ..\PWMISServiceHost\bin\Debug\PWMIS.EnterpriseFramework.ModuleRoute.XML  PWMIS.EnterpriseFramework-NugetPkg\lib\

copy ..\PWMISServiceHost\bin\Debug\PWMIS.EnterpriseFramework.Message.PublisherLib.*  MSF-Server-Host-NuGetPkg\lib\

echo �ļ�����ȫ�����
pause






